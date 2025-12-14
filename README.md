# PowerTtraders.PowerPosition.IntradayReport
This project generates intraday reports for Power Traders, providing insights into trading positions and performance throughout the trading day. It is built using .NET 10.0 and leverages C# 14.0 features for enhanced functionality and performance.
## Features
- Real-time data processing for intraday trading positions
- etc.

## Configuration
- Interval settings for data refresh
- Output localation 

## Requirements
- .NET 10.0 SDK
- Externnal libraries
	- `PowerService.dll` must be in `{ROOT}\src\EnternalLibraries` folder. (excluded)
	- Nice to have if this library is as a nuget package in your nuget repository

## Informatioin about code challenge 
- Used BackgroundService instead of ServiceBase
- can add more logging
- Can add more configuration options
- Can Add more unit tests & integration tests
