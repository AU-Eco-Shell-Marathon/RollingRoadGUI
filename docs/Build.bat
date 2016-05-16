@echo off

ECHO Building Architecture
cd Architecture
call Build.bat > NUL
cd ..

ECHO Building Requirements
cd Requirements
call Build.bat > NUL
cd ..

ECHO Building Technical (May take a couple of minutes)
cd Technical
call Build.bat > NUL
cd ..

ECHO Copying files
copy "Architecture\Architecture.pdf" "./Architecture.pdf" > NUL
copy "Requirements\Requirements.pdf" "./Requirements.pdf" > NUL
copy "Technical\Technical.pdf" "./Technical.pdf" > NUL