namespace ShootingAcademy.Models.Exceptions
{
    public class BaseException : ApplicationException
    {
        public bool IsError { get; set; }

        public int Code { get; set; }

        public bool Show { get; set; }

        public BaseException(string message) : base(message)
        {
            Code = 400;
            IsError = true;
            Show = true;
        }

        public BaseException(string message, int code, bool isError = true, bool show = true) : base(message)
        {
            Code = code;
            IsError = isError;
            Show = show;
        }

        public BaseError GetModel()
        {
            return new BaseError()
            {
                Message = Message,
                Code = Code,
                Error = IsError,
                Show = Show
            };
        }
    }
}
