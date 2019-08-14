@echo off
cls

"tools\nuget\NuGet.exe" pack AsyncSuffix.nuspec
move .\*.nupkg .\.deploy\