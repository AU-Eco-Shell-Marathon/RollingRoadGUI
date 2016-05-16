IF EXIST "generated" (
    rmdir "generated" /s /q
)

mkdir "generated"

doxygen.exe main.cfg > NUL 2>&1 3>&1

generated\latex\make.bat &
generated\latex\make.bat &
generated\latex\make.bat &