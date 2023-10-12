using ExifLib;
using FileCompareAndCopy.Commands.Handler;
using FileCompareAndCopy.Commands.Request;
using FileCompareAndCopy.Commands.Response;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;
using System.Xml.Linq;

namespace FileCompareAndCopy
{
    internal class ActionDo
    {
        internal static bool MakeDatabase(string path, IDatabase dataBase)
        {
            ICommand handlerGetFileInfos = new HandlerGetFileInfos();
            IRequest requestGetFileInfos = new RequestGetFileInfos(path);
            ResponseGetFileInfos response = (ResponseGetFileInfos)handlerGetFileInfos.Execute(requestGetFileInfos);

            List<FileData> fileInfosDir = response.FileInfos;
            SaveFileData(fileInfosDir, dataBase);
            return true;
        }
        internal static void UpdateNewFullName(string to, IDatabase dataBase)
        {
            ICommand updateNewFullName = new HandlerUpdateNewFullName(dataBase);
            updateNewFullName.Execute(new RequestUpdateNewFullName(to));
        }

        internal static void SaveFileData(List<FileData> fileInfosDir, IDatabase dataBase)
        {
            ICommand saveFile = new HandlerSaveFileDatas(dataBase);
            saveFile.Execute(new RequestSaveFileData(fileInfosDir));
        }

        internal static void CopyFile(string from, string to, IDatabase dataBase)
        {

            ICommand handlerCopyFile = new HandlerCopyFile(dataBase);
            IRequest requestCopyFile = new RequestCopyFile(from, to);

            handlerCopyFile.Execute(requestCopyFile);
        }
        internal static void UpdateMetaData(IDatabase dataBase)
        {
            ICommand handlerUpdateMetaData = new HandlerUpdateMetaData(dataBase);
            IRequest requestUpdateMetaData = new RequestUpdateMetaData();

            handlerUpdateMetaData.Execute(requestUpdateMetaData);
        }
    }
}
