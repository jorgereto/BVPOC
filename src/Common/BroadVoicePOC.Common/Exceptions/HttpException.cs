using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BroadVoicePOC.Common.Exceptions
{
    public class HttpException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public HttpException(HttpStatusCode statuscode) : base(String.Format("Service returned {0}", statuscode))
        {
            StatusCode = statuscode;
        }

        public HttpException(HttpStatusCode statuscode, string message) : base(String.Format("Service returned {0}: {1}", statuscode, message))
        {
            StatusCode = statuscode;
        }
    }
}
