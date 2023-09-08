using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi_Client.Responses
{
    internal class ClientResponse<T>
    {
        public ClientResponse()
        {

        }
        public ClientResponse(T data)
        {
            Data = data;
        }

        public T Data;
        public bool Success { get; set; } = true;
        public bool ServerIsRunning { get; set; } = true;
        public List<string> Errors { get; set; } = new List<string>();
    }
}
