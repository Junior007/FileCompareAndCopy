namespace FileCompareAndCopy.Commands.Request
{
    internal class RequestInsertErrorFile : IRequest
    {
        public readonly FileData FileData;
        public readonly string Message;
        public RequestInsertErrorFile(FileData fileData, string message)
        {
            FileData = fileData;
            Message = message;
        }
    }
}
