using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Roulette.Data.Models.Response
{
    public class Response<T>
    {
        public Response()
        {
            StatusCode = (int)HttpStatusCode.OK;
        }

        public Response(T response)
        {
            Data = response;
            StatusCode = (int)HttpStatusCode.OK;
        }

        public T Data { get; init; }
        public string Message { get; init; }
        public int StatusCode { get; private set; }

        public void ChangeStatusCode(HttpStatusCode statusCode, string message = null)
        {
            StatusCode = (int)statusCode;
        }
        public HttpStatusCode GetNormalizedStatusCode()
        {
            return (HttpStatusCode)StatusCode;
        }
    }
}
