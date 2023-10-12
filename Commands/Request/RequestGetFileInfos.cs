namespace FileCompareAndCopy.Commands.Request
{
    internal class RequestGetFileInfos : IRequest
    {
        internal string From { get; }
        public RequestGetFileInfos(string from)
        {
            From = from;
        }
    }
}
