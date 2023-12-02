using Provider.MaketData.NSE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace nse_hist_data_downloader_lib
{
    internal class Program {

        private const string _DownloadDirectory = @"D:\local-store\workspace\stock-market-hist-data-store";

        static void Main(string[] args) {

        /* 
         * https://nsearchives.nseindia.com/products/content/sec_bhavdata_full_08092023.csv
         * https://nsearchives.nseindia.com/products/content/sec_bhavdata_full_24112023.csv
        */

            if(args.Length < 2) throw new ArgumentNullException("retuire report name and report date as arguments");

            DateTime reportDate = DateTime.ParseExact(args[1], "MM/dd/yyyy", null);

            DailyReportType reportType;

            if (!Enum.TryParse<DailyReportType>(args[0], out reportType)) {
                Console.WriteLine(string.Format("Invalid report name: {0}", args[0]));
                return; 
            }

            bool downloadStatus = new DailyDataReport(_DownloadDirectory, 1*1000).DownloadDailyDataReport(reportType, reportDate);
            Console.WriteLine(string.Format("{0}", downloadStatus ? "Download successful!" : "Download failed!"));
        }
    }
}
