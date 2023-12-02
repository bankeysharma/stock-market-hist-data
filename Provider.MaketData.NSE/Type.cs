using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provider.MaketData.NSE {
    public enum DailyReportType {
        /// <summary>
        /// Default value that do not map to any report
        /// </summary>
        None,
        /// <summary>
        /// EOD Seucirty Data such as Price, Volume, Traded Qty, and Deliveries
        /// </summary>
        BhavCopyFull,
        /// <summary>
        /// 
        /// </summary>
        MarketActivity,
        /// <summary>
        /// 
        /// </summary>
        DailyVolatility,
        /// <summary>
        /// 
        /// </summary>
        CorpAction
    }
}
