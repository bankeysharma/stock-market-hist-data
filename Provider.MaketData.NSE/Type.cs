using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provider.MaketData.NSE {
    public enum DailyReportType {
        /// <summary>
        /// EOD Seucirty Data such as Price, Volume, Traded Qty, and Deliveries
        /// </summary>
        None,
        BhavCopyFull,
        MarketActivity
    }
}
