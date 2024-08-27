dotnet build --no-restore -c Release ./src/TrafficCamera/

$sw = [System.Diagnostics.Stopwatch]::StartNew()

./src/TrafficCamera/bin/Release/net8.0/TrafficCamera.exe ./traffic.txt

$sw.Stop()
$sw.Elapsed
