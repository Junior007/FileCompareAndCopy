
namespace FileCompareAndCopy.Commands.Response
{
    internal class ResponseGetFileInfos : IResponse
    {
        public List<FileData> FileInfos  { get; }

        public ResponseGetFileInfos(List<FileData> fileInfos)
        {
            FileInfos = fileInfos;
        }
    }
}
