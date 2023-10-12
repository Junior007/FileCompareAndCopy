namespace FileCompareAndCopy.Commands.Request
{
    internal class RequestExistsInDataBase : IRequest
    {
        public FileData FileData { get;  }

        public RequestExistsInDataBase(FileData fileData)
        {
            FileData = fileData;
        }
    }
}
