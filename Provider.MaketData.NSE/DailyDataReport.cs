using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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

        protected string HostNameNSEArchive { get; set; }
        protected string HostNameNSEIndia { get; set; }

        public DailyDataReport(string downloadDirectory, int downloadIntervalinMilliSeconds) {
            DownloadDirectory = downloadDirectory;
            DownloadInterval = downloadIntervalinMilliSeconds;
            SchemeName = "https";
            HostNameNSEArchive = "nsearchives.nseindia.com";
            HostNameNSEIndia = "www.nseindia.com";
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
                    case DailyReportType.DailyVolatility:
                        DownloadHelper.WriteContentToFile(DownloadDailyVolatilityReport(reportDate).Result, DownloadDirectory, "daily-volatility", string.Format("{0}.csv", reportDate.ToString("yyyyMMdd")));
                        break;
                    case DailyReportType.CorpAction:
                        DownloadHelper.WriteContentToFile(DownloadCorpActions(reportDate,DateTime.Now).Result, DownloadDirectory, "consolidated-reports", string.Format("corp-actions-{0}-{1}.csv", reportDate.ToString("yyyyMMdd"), DateTime.Now.ToString("yyyyMMdd")));
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
            ValidateForWeekEnd(reportDate);
            return DownloadHelper.GetDownloadedContent(CreateUriForArchiveReports(string.Format(@"/products/content/sec_bhavdata_full_{0}.csv", reportDate.ToString("ddMMyyyy"))));

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
            return DownloadHelper.GetDownloadedContent(CreateUriForArchiveReports(string.Format(@"/archives/equities/mkt/MA{0}.csv", reportDate.ToString("ddMMyy"))));

        }

        /// <summary>
        /// Downloads EOD Market Volatility Data
        /// </summary>
        /// <param name="reportDate"></param>
        /// <returns></returns>
        /// <!--
        /// https://nsearchives.nseindia.com/archives/nsccl/volt/CMVOLT_01122023.CSV
        /// Date	Symbol	Underlying Close Price (A)	Underlying Previous Day Close Price (B)	Underlying Log Returns (C) = LN(A/B)	Previous Day Underlying Volatility (D)	Current Day Underlying Daily Volatility (E) = Sqrt(0.995*D*D + 0.005*C*C)	Underlying Annualised Volatility (F) = E*Sqrt(365)
        /// 1-Dec-23	20MICRONS	176.5	183.6	-0.0394	0.0289	0.029	0.554
        /// 1-Dec-23	21STCENMGM	24.3	23.85	0.0187	0.0164	0.0164	0.3133
        /// -->
        protected Task<string> DownloadDailyVolatilityReport(DateTime reportDate) {
            ValidateForWeekEnd(reportDate);
            return DownloadHelper.GetDownloadedContent(CreateUriForArchiveReports(string.Format(@"/archives/nsccl/volt/CMVOLT_{0}.CSV", reportDate.ToString("ddMMyyyy"))));

        }


        /// <summary>
        /// Corporate Actions such as Stock Split and Bonus
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="tillDate"></param>
        /// <returns></returns>
        /// <!--
        /// https://www.nseindia.com/api/corporates-corporateActions?index=equities&from_date=02-12-2022&to_date=02-12-2023&csv=true
        /// https://www.nseindia.com/api/corporates-corporateActions?index=equities&from_date=01-12-2023&to_date=02-12-2023&csv=true
        /// SYMBOL	COMPANY NAME	SERIES	PURPOSE	FACE VALUE	EX-DATE	RECORD DATE	BOOK CLOSURE START DATE	BOOK CLOSURE END DATE
        /// DHANUKA Dhanuka Agritech Limited    EQ Buy Back	2	1-Jan-19	2-Jan-19	-	-
        /// JWL Jupiter Wagons Limited  EQ Extra Ordinary General Meeting	10	1-Jan-19	-	3-Jan-19	7-Jan-19
        /// -->
        protected Task<string> DownloadCorpActions(DateTime fromDate, DateTime tillDate) {
            return DownloadHelper.GetDownloadedContent(CreateUriForNSEIndiaReports(@"/api/corporates-corporateActions", string.Format(@"index=equities&from_date={0}&to_date={1}&csv=true", fromDate.ToString("dd-MM-yyyy"), tillDate.ToString("dd-MM-yyyy"))));

        }

        #region Helper Functions

        protected string CreateUriForArchiveReports(string path) {
            return new UriBuilder(SchemeName, HostNameNSEArchive) {Path = path}.ToString();
        }

        protected string CreateUriForNSEIndiaReports(string path, string query) {
            return new UriBuilder(SchemeName, HostNameNSEIndia) { Path = path, Query = query}.ToString();
        }

        protected void ValidateForWeekEnd(DateTime reportDate) {
            if (reportDate.DayOfWeek == DayOfWeek.Saturday || reportDate.DayOfWeek == DayOfWeek.Sunday) {
                string errorLog = string.Format("Skipping report of weekend - {0}", reportDate.ToShortDateString());
                Log.WriteLog(errorLog);
                throw new FileNotFoundException(errorLog);
            }

        }
#endregion

    }
}
