namespace XtraUpload.Domain
{
    public class ErrorContent
    {
        public ErrorContent(ErrorContent error)
        {
            Message = error.Message;
            ErrorType = error.ErrorType;
        }

        public ErrorContent(string message, ErrorOrigin errorType)
        {
            Message = message;
            ErrorType = errorType;
        }

        public string Message { get; private set; }
        public ErrorOrigin ErrorType { get; private set; }

        public override string ToString()
        {
            return Message + " originated from: " + ErrorType;
        }
    }
}
