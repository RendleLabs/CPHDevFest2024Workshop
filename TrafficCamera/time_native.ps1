# dotnet publish -c Release -o ./native ./src/TrafficCamera/

$sw = [System.Diagnostics.Stopwatch]::StartNew()

./native/TrafficCamera.exe ./traffic.txt

$sw.Stop()
$sw.Elapsed
