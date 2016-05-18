mkdir "generated"

doxygen.exe main.cfg

echo Building pdf

generated\latex\make.bat &