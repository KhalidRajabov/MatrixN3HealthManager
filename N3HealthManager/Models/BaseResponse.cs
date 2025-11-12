using System.Net;

namespace N3HealthManager.Models
{
    public class BaseResponse
    {
        /// <summary>
        /// servis uğurlu olduqda 200 digər bütün hallarda 0 dan fərqli olacaq
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// servis uğurlu olduqda məlumat, digər bütün hallanda null olacaq
        /// </summary>
        public dynamic Data { get; set; }
        /// <summary>
        /// xəta məlumatları
        /// </summary>
        public Exceptions Exception { get; set; }
        /// <summary>
        /// mesag strin ve objectde ola biler
        /// </summary>
        public dynamic ErrorMessage { get; set; }

        /// <summary>
        /// </summary>
        public BaseResponse()
        {
            Exception = null;
            Data = null;
        }


        public BaseResponse(HttpStatusCode code)
        {
            Code = (int)code;
        }


        public BaseResponse(HttpStatusCode code, dynamic data)
        {
            Code = (int)code;
            Data = data;
        }

        public BaseResponse(string errorMessage)
        {
            Code = 400;
            ErrorMessage = errorMessage;
        }

        public BaseResponse(HttpStatusCode code, Exceptions exceptions)
        {
            Code = (int)code;
            Exception = exceptions;
        }
        public BaseResponse(HttpStatusCode code, ExceptionStatus status, string message)
        {
            Code = (int)code;
            Exception = new Exceptions
            {
                Status = status.ToString(),
            };
            ErrorMessage = message;
        }


        public BaseResponse(HttpStatusCode statusCode, ExceptionStatus status, string messageCode, Func<string, string> function)
        {
            Code = (int)statusCode;
            Exception = new Exceptions
            {
                Status = status.ToString(),
                Code = messageCode,
                ErrorMessage = function(messageCode)
            };
        }


        public BaseResponse(HttpStatusCode statusCode, ExceptionStatus status, string messageCode, Func<ExceptionStatus, string, string> function)
        {
            Code = (int)statusCode;
            Exception = new Exceptions
            {
                Status = status.ToString(),
                Code = messageCode,
                ErrorMessage = function(status, messageCode)
            };
        }
    }
    /// <summary>
    /// xeta melumatlar 
    /// </summary>
    public class Exceptions
    {
        public string Status { get; set; }
        public string Code { get; set; }
        public dynamic ErrorMessage { get; set; }
        public dynamic ValidationError { get; set; }
        public string[] ErrorParams { get; set; }
    }
    public enum ExceptionStatus
    {
        info,
        error,
        warn
    }
}
