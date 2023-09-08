using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi_Client.Models;

namespace WebApi_Client.Logger
{
    internal interface ILogger
    {
        string InputMessage();
        void DisplayMessage(string message);
    }
}
