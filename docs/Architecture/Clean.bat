del /s *.aux
del /s *.bbl
del /s *.bcf
del /s *.blg
del /s *.glg
del /s *.glo
del /s *.gls
del /s *.idx
del /s *.ilg
del /s *.ind
del /s *.ist
del /s *.lof
del /s *.log
del /s *.lol
del /s *.lot
del /s *.nlo
del /s *.nls
del /s *.out
del /s *.tdo
del /s *.toc
del /s *.xml
del /s *.lox

for /f "usebackq delims=" %%d in (`"dir /ad/b/s | sort /R"`) do rd "%%d"

del Architecture.pdf