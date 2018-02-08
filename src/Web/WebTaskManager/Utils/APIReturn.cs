using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebTaskManager.Utils
{
    public class APIReturn : JsonResult
    {

        public int Code { get; set; }

        public string Message { get; set; }

        public Object Data { get; set; }

        public bool Success { get { return this.Code == 0; } }
        public APIReturn(int code, string message, object data) : base(new { code = code, message = message, data = data, success = code == 0 })
        {
            this.Code = code;
            this.Message = message;
            this.Data = data;
        }

        public static APIReturn BuildSuccess(string message = null, object data = null)
        {
            return new APIReturn(0, message, data);
        }

        public static APIReturn BuildFail(string message)
        {
            return BuildFial(99, message);
        }

        public static APIReturn BuildFial(int code, string message)
        {
            return new APIReturn(code, message, null);
        }

    }
}
