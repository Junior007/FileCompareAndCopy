namespace FileCompareAndCopy.Commands.Response
{

    internal class ResponseDefault : IResponse
    {

        public bool Success { get; }

        public ResponseDefault(bool success)
        {

            Success = success;

        }
    }
}
