cd src
call Clean.bat
cd ..

cd docs
call Clean.bat
cd ..

rd /s /q temp
del RollingRoadGUI_x64.zip > nul