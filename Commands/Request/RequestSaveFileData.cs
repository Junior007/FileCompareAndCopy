namespace FileCompareAndCopy.Commands.Request
{
    internal class RequestSaveFileData : IRequest
    {
        public readonly List<FileData> FilesData;
        public RequestSaveFileData(List<FileData> fileInfosDir)
        {
            FilesData = fileInfosDir;
        }

    }
}
