using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi_Client.Models;

namespace WebApi_Client.Logger
{
    internal class ConsoleLogger : ILogger
    {
        public string InputMessage()
        {
            return Console.ReadLine();
        }

        public void DisplayMessage(string message)
        {
            Console.Write(message);
        }
    }
}
