using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartWaste.Model.Error
{
    public class HttpError
    {
        public HttpError(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }
    }
}
