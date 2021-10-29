using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exceptions
{
    public class AppException : Exception
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public int Code { get; set; }
        public object AdditionalData { get; set; }
        
        public AppException(string message,
            int code = -1,
            HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError,
            object additionalData = null,
            Exception exception = null)
            : base(message, exception)
        {
            Code = code;
            HttpStatusCode = httpStatusCode;
            AdditionalData = additionalData;
        }
    }
}
