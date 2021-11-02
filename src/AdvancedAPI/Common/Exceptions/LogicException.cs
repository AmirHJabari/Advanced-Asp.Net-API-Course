using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exceptions
{
    public class LogicException : AppException
    {
        public LogicException(string message, int apiStatusCode = -1, HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError, object additionalData = null, Exception exception = null) 
            : base(message, apiStatusCode, httpStatusCode, additionalData, exception)
        {
        }
    }
}
