using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace eshop_api.Exceptions
{
    public class ApiError
    {
        public string Title { get; }
        public int StatusCode { get; }
        public string Detail { get; }

        public ApiError(int code, string title, string detail)
        {
            StatusCode = code;
            Title = title;
            Detail = detail;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
