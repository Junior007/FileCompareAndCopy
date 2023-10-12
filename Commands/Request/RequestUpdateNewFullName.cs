namespace FileCompareAndCopy.Commands.Request
{
    internal class RequestUpdateNewFullName : IRequest
    {
        public string Path { get;  }
        public RequestUpdateNewFullName(string path)
        {
            Path = path;
        }
    }
}
