IF EXIST "generated" (
    rmdir "generated" /s /q
)

mkdir "generated"

doxygen.exe main.cfg

generated\latex\make.bat &
generated\latex\make.bat &
generated\latex\make.bat &

copy generated\latex\refman.pdf code_documentation.pdf

pause