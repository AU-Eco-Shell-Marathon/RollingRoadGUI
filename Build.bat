@echo off

echo Building Code

cd src
call Build.bat > NUL
cd ..

cd docs
call Build.bat
cd ..


echo Creating zip file with build and docs

mkdir temp > NUL

mkdir "temp\bin" > NUL
copy "src\RollingRoad.WinApplication\bin\x64\Release\" "temp\bin" > NUL
copy "src\ShortcutTemplate.lnk" "temp\RollingRoad.lnk" > NUL

mkdir "temp\docs" > NUL
copy "docs\Architecture.pdf" "temp\docs\Architecture.pdf" > NUL
copy "docs\Requirements.pdf" "temp\docs\Requirements.pdf" > NUL
copy "docs\Technical.pdf" "temp\docs\Technical.pdf" > NUL


del RollingRoadGUI_x64.zip > nul
powershell.exe -nologo -noprofile -command "& { Add-Type -A 'System.IO.Compression.FileSystem'; [IO.Compression.ZipFile]::CreateFromDirectory('temp', 'RollingRoadGUI_x64.zip'); }"  > NUL
rd /s /q temp  > NUL

echo Done