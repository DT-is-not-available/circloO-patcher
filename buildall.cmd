@echo off

REM # im sure theres a better way to do this with dotnet
REM # but i simply dont have the time to mess around with it rn

REM # so uh TODO: clean the build process up rather than using this
REM # hacked together solution i came up with in 5 mins

cd UndertaleModTool/UndertaleModLib
dotnet publish
cd ../../src
dotnet publish
cd ..