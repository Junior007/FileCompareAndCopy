using FileCompareAndCopy.Commands.Request;
using FileCompareAndCopy.Commands.Response;

namespace FileCompareAndCopy.Commands.Handler
{
    internal class HandlerGetFileInfos : ICommand
    {
        public IResponse Execute(IRequest request)
        {
            List<FileData> fileInfos =new List<FileData>();
            if (request is RequestGetFileInfos requestGetFileInfos)
            {
                GetFileInfos(requestGetFileInfos.From, fileInfos);
            }
            return new ResponseGetFileInfos(fileInfos);
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
