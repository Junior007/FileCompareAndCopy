using FileCompareAndCopy.Commands.Request;
using FileCompareAndCopy.Commands.Response;

namespace FileCompareAndCopy.Commands.Handler
{
    internal class HandlerCopyFile : ICommand
    {
        private IDatabase _dataBase;

        public HandlerCopyFile(IDatabase dataBase)
        {
            _dataBase = dataBase;
        }

        public IResponse Execute(IRequest request)
        {
            if (request is RequestCopyFile requestCopyFile)
            {
                CopyFile(requestCopyFile.From, requestCopyFile.To);
            }

            return new ResponseDefault(true);
        }

        private void CopyFile(string from, string to)
        {
            List<FileData> fileDatas = new List<FileData>();
            ICommand commandExistsInDataBase = new HandlerExistsInDataBase(_dataBase);

            GetFileInfos(from, fileDatas);

            foreach (var fileData in fileDatas)
            {
                string message = string.Empty;
                IRequest requestExistsInDataBase = new RequestExistsInDataBase(fileData);
 

                if (!fileData.IsImageVideoFile)
                {
                    message = "No image/video file";
                }
                else if (!(commandExistsInDataBase.Execute(requestExistsInDataBase) as ResponseDefault).Success)
                {

                    try
                    {
                        fileData.CopyTo(to);
                        ICommand saveFile = new HandlerSaveFileDatas(_dataBase);
                        saveFile.Execute(new RequestSaveFileData(new List<FileData>() { fileData }));
                    }
                    catch (Exception ex)
                    {
                        message = ex.Message;
                    }
                }
                if (!string.IsNullOrEmpty(message))
                {
                    ICommand handlerError = new HandlerInsertFileError(_dataBase);
                    handlerError.Execute(new RequestInsertErrorFile(fileData, message));
                }
            }
        }
        private void GetFileInfos(string from, List<FileData> fileInfos)
        {
            if (from.Contains("$RECYCLE.BIN") || from.Contains("System Volume Information"))
                return;
            DirectoryInfo dirInfo = new DirectoryInfo(from);

            fileInfos.AddRange(dirInfo.GetFiles().Select(x => new FileData(x, from)));

            string[] subDirs = Directory.GetDirectories(from);

            foreach (string subDir in subDirs)
            {
                GetFileInfos(subDir, fileInfos);
            }
        }
    }
}
