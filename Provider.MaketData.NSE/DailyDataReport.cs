using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Provider.MaketData.NSE.HttpClientHelper;

namespace Provider.MaketData.NSE {
    public class DailyDataReport {
        protected string DownloadDirectory { get; set; }
        protected int DownloadInterval { get; set; }

        protected string SchemeName { get; set; }

        protected string HostName { get; set; }

        public DailyDataReport(string downloadDirectory, int downloadIntervalinMilliSeconds) {
            DownloadDirectory = downloadDirectory;
            DownloadInterval = downloadIntervalinMilliSeconds;
            SchemeName = "https";
            HostName = "nsearchives.nseindia.com";
        }

        ///// <summary>
        ///// Download EOD security data of given report-type of the provided report date
        ///// </summary>
        ///// <param name="report"></param>
        ///// <param name="reportStartDate"></param>
        ///// <param name="reportEndDate"></param>
        //public void DownloadDailyDataReport(DailyReportType report, DateTime reportStartDate, DateTime reportEndDate) {
        //    if (reportStartDate == null || reportStartDate == DateTime.MinValue || reportEndDate == null || reportEndDate == DateTime.MinValue) throw new ArgumentNullException();
        //    if (reportStartDate > reportEndDate) throw new ArgumentOutOfRangeException();

        //    int failedAttempts = 0;

        //    while (reportStartDate <= reportEndDate && failedAttempts < 3) {
        //        try {

        //            DownloadDailyDataReport(report, reportStartDate);

        //            Log.WriteLog(string.Format("Sleeping for {0} milliseconds...", this.DownloadInterval));
        //            Thread.Sleep(this.DownloadInterval);
        //            if (failedAttempts != 0) failedAttempts = 0;

        //        } catch (Exception ex) {
        //            failedAttempts++;
        //            Log.WriteLog(ex.ToString());
        //        }

        //        reportStartDate = reportStartDate.AddDays(1);

        //    }
        //}

        /// <summary>
        /// Download EOD security data of given report-type of the provided report date
        /// </summary>
        /// <param name="report"></param>
        /// <param name="reportDate"></param>
        public bool DownloadDailyDataReport(DailyReportType report, DateTime reportDate) {

            if (reportDate == null || reportDate == DateTime.MinValue) throw new ArgumentNullException(nameof(reportDate));

            Log.WriteLog(string.Format("Initiating download of {0} for {1}", report.ToString(), reportDate.ToShortDateString()));

            try {
                switch (report) {
                    case DailyReportType.BhavCopyFull:
                        DownloadHelper.WriteContentToFile(DownloadBhavCopyFull(reportDate).Result, DownloadDirectory, "bhav-copy-full", string.Format("{0}.csv", reportDate.ToString("yyyyMMdd")));
                        break;
                    case DailyReportType.MarketActivity:
                        DownloadHelper.WriteContentToFile(DownloadMarketActivityReport(reportDate).Result, DownloadDirectory, "market-activity", string.Format("{0}.csv", reportDate.ToString("yyyyMMdd")));
                        break;
                }
            } catch (FileNotFoundException) {
                return false;

            }

            Log.WriteLog(string.Format("Successfully downloaded {0} for {1}", report.ToString(), reportDate.ToShortDateString()));

            return true;
        }


        /// <summary>
        /// Downloads EOD Seucirty Data such as Price, Volume, Traded Qty, and Deliveries
        /// </summary>
        /// <param name="reportDate"></param>
        /// <!-- 
        /// https://nsearchives.nseindia.com/products/content/sec_bhavdata_full_24112023.csv
        /// SYMBOL SERIES    DATE1 PREV_CLOSE	 OPEN_PRICE	 HIGH_PRICE	 LOW_PRICE	 LAST_PRICE	 CLOSE_PRICE	 AVG_PRICE	 TTL_TRD_QNTY	 TURNOVER_LACS	 NO_OF_TRADES	 DELIV_QTY	 DELIV_PER
        /// CIPLA	 EQ	 08-Sep-2023	1249.3	1249.5	1262	1239	1243	1244.6	1247.17	787710	9824.12	54343	339736	43.13
        /// -->
        protected Task<string> DownloadBhavCopyFull(DateTime reportDate) {
            
            if (reportDate.DayOfWeek == DayOfWeek.Saturday || reportDate.DayOfWeek == DayOfWeek.Sunday) {
                string errorLog = string.Format("Skipping Bhav Copy, its weekend - {0}", reportDate.ToShortDateString());
                Log.WriteLog(errorLog);
                throw new FileNotFoundException(errorLog);
            }

            return DownloadHelper.GetDownloadedContent(CreateUriForReport(string.Format(@"/products/content/sec_bhavdata_full_{0}.csv", reportDate.ToString("ddMMyyyy"))));

        }


        /// <summary>
        /// Downloads EOD Market Activity Data such as top 25 securities, volatility, traded qty
        /// </summary>
        /// <param name="reportDate"></param>
        /// <returns></returns>
        /// <!--
        /// https://nsearchives.nseindia.com/archives/equities/mkt/MA011223.csv
        /// INDEX PREVIOUS CLOSE	OPEN	HIGH	LOW	CLOSE	GAIN/LOSS
        /// Nifty 50	19727.05	19774.8	19867.15	19727.05	19819.95	92.9
        /// -->
        protected Task<string> DownloadMarketActivityReport(DateTime reportDate) {
            return DownloadHelper.GetDownloadedContent(CreateUriForReport(string.Format(@"/archives/equities/mkt/MA{0}.csv", reportDate.ToString("ddMMyy"))));

        }

        #region Helper Functions

        protected string CreateUriForReport(string path) {
            return new UriBuilder(SchemeName, HostName) {Path = path}.ToString();
        }
#endregion

    }
}
