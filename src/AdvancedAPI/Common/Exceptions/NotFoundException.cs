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
        public NotFoundException(string message, ApiResultStatusCode apiStatusCode = ApiResultStatusCode.None, HttpStatusCode httpStatusCode = HttpStatusCode.NotFound, object additionalData = null, Exception exception = null)
            : base(message, apiStatusCode, httpStatusCode, additionalData, exception)
        {
        }
    }
}
