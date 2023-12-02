# .\download-nse-data.ps1 -reportName "BhavCopyFull" -startDate "01/01/2023" -endDate "01/10/2023"
# .\download-nse-data.ps1 -reportName "MarketActivity" -startDate "01/01/2023" -endDate "01/10/2023"
# .\download-nse-data.ps1 -reportName "DailyVolatility" -startDate "01/01/2023" -endDate "01/10/2023"


param(
    [string]$exePath = "D:\local-store\workspace\stock-market-hist-data\Provider.MarketData\bin\Debug\Provider.MarketData.exe",
    [string]$reportName,
    [string]$startDate,
    [string]$endDate
)

# Validate that both start and end dates are provided
if (-not $startDate -or -not $endDate) {
    Write-Host "Please provide both start and end dates."
    exit 1
}

# Convert start and end dates to DateTime objects
$startDateTime = Get-Date $startDate -ErrorAction Stop
$endDateTime = Get-Date $endDate -ErrorAction Stop

# Loop through dates from start to end, calling the executable for each date
$currentDate = $startDateTime
while ($currentDate -le $endDateTime) {
    $formattedDate = $currentDate.ToString("MM/dd/yyyy")
    Write-Host "Processing date: $formattedDate"
    & $exePath $reportName $formattedDate

    Start-Sleep -Milliseconds 500

    # Increment the current date by one day
    $currentDate = $currentDate.AddDays(1)
}
