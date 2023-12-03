# stock-market-hist-data

You can download historical end-of-day equities data from the National Stock Exchange (NSE) India. This tool leverages HTTP requests to download certain end-of-day reports such as BhavCopy, Daily Volatility, and Market Activity.

## How to use

The Provider.MarketData project has a PowerShell script `download-nse-data.ps1` available under its `\Scripts` directory. You can use the script to download reports for a given date or range of dates. Supported commands are shown below in the example section. The PowerShell script may require signing (based on your local machines' policies), instructions to sign the script are also available in the `.text` file under the same location. 

## Examples
1. BhavCopy and Security Deliverable Data:

`.\download-nse-data.ps1 -reportName "BhavCopyFull" -startDate "01/01/2023" -endDate "12/01/2023"`

> SYMBOL SERIES    DATE1 PREV_CLOSE	 OPEN_PRICE	 HIGH_PRICE	 LOW_PRICE	 LAST_PRICE	 CLOSE_PRICE	 AVG_PRICE	 TTL_TRD_QNTY	 TURNOVER_LACS	 NO_OF_TRADES	 DELIV_QTY	 DELIV_PER
> CIPLA	 EQ	 08-Sep-2023	1249.3	1249.5	1262	1239	1243	1244.6	1247.17	787710	9824.12	54343	339736	43.13

2. Market Activity Report

`.\download-nse-data.ps1 -reportName "MarketActivity" -startDate "01/01/2023" -endDate "12/01/2023"`

> INDEX PREVIOUS CLOSE	OPEN	HIGH	LOW	CLOSE	GAIN/LOSS 
> Nifty 50	19727.05	19774.8	19867.15	19727.05	19819.95	92.9

3. Daily Volatility Report

`.\download-nse-data.ps1 -reportName "DailyVolatility" -startDate "01/01/2023" -endDate "12/01/2023"`
   
