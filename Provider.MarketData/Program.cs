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

        private const string V = @"D:\local-store\workspace\stock-market-hist-data-store";

        static void Main(string[] args) {

        /* 
         * https://nsearchives.nseindia.com/products/content/sec_bhavdata_full_08092023.csv
         * https://nsearchives.nseindia.com/products/content/sec_bhavdata_full_24112023.csv
        */

            if(args.Length == 0) throw new ArgumentNullException("args");

            string downloadDirectory = V;

            DailyDataReport reportProvider = new DailyDataReport(downloadDirectory, 1*1000);

            //reportProvider.DownloadDailyDataReport(DailyReportType.BhavCopyFull, new DateTime(2023, 11, 20), new DateTime(2023, 11, 24));

            DateTime reportDate = DateTime.ParseExact(args[0],"MM/dd/yyyy", null);

            reportProvider.DownloadDailyDataReport(DailyReportType.BhavCopyFull, reportDate);

            Console.WriteLine("Completed!");
        }
    }
}
