using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuthTest.Utils
{
    public class BusinessLogicMessage<T> : BusinessLogicMessage
    {
        public T? Result { get; set; }

        public BusinessLogicMessage(HttpStatusCode statusCode, string message, T? result = default)
        {
            Result = result;
            StatusCode = statusCode;
            Message = message;
        }

        public BusinessLogicMessage(BusinessLogicMessage passedMessage)
        {
            Result = default(T);
            StatusCode = passedMessage.StatusCode;
            Message = passedMessage.Message;
        }

        public override string ToString()
        {
            if (this.Result == null)
            {
                BusinessLogicMessage businessLogicMessage = new BusinessLogicMessage(this.StatusCode, this.Message);
                return businessLogicMessage.ToString();
            }
            else
            {
                return base.ToString();
            }
        }
    }

    public class BusinessLogicMessage
    {
        public static BusinessLogicMessage NotAuthenticated = new(HttpStatusCode.Unauthorized, "User is not authenticated.");
        public bool GetValid() => StatusCode == HttpStatusCode.OK;

        public BusinessLogicMessage()
        {
            StatusCode = HttpStatusCode.OK;
            Message = string.Empty;
        }

        public BusinessLogicMessage(HttpStatusCode statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }

        public string Status => StatusCode.ToString();
        internal HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
    }
}
