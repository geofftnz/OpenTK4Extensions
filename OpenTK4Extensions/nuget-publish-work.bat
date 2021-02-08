@echo off
cd bin/Release
dir /o-D /b *.nupkg > nupkg.txt
set /p NUPKG=<nupkg.txt
nuget add %NUPKG% -Source c:\\nuget
echo "added %NUPKG%"
