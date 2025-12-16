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

## Build and Run
### Build the project
```bash
podman build -t sample-app .
```
### Run the project
```bash
podman run -d --name sample-app-container -p 8080:80 sample-app
```
OR
```bash
podman run -d --name sample-app-container \
  -p 8080:80 \
  -e ReportConfig__IntervalMinutes=15 \
  -e ReportConfig__OutputLocation=c://Reports \
  sample-app
```
### Run with Docker Compose
```bash
podman-compose up -d
```

### Run with Helm
```bash
helm install sample-app ./helm/sample-app
```
Or upgrade an existing release:
```bash
helm upgrade sample-app ./helm/sample-app
```
Check deployment status:
```bash
helm list
kubectl get pods
kubectl get svc
kubectl get deployments
```


