using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provider.MaketData.NSE
{
    internal static class Log
    {

        public static void WriteLog(string logLine) {
            Console.WriteLine("{0}\t- {1}", DateTime.Now.ToLongTimeString(), logLine);
        }
    }
}
