namespace FileCompareAndCopy.Commands.Request
{
    internal class RequestCopyFile : IRequest
    {
        public string From { get; }
        public string To { get; }
        public RequestCopyFile(string from, string to)
        {
            To = to;
            From = from;
        }
    }
}