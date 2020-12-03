using System;
using System.Collections.Generic;
using System.Text;

namespace Roulette.Data.Models.Response
{
    public class Response<T>
    {
        public Response() { }

        public Response(T response)
        {
            Data = response;
        }

        public T Data { get; set; }
        public string Message { get; set; }
    }
}
