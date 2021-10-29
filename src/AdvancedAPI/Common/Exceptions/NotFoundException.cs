using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exceptions
{
    public class NotFoundException : AppException
    {
        public NotFoundException(string message, int code = -1, HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError, object additionalData = null, Exception exception = null)
            : base(message, code, httpStatusCode, additionalData, exception)
        {
        }
    }
}
