@echo off

REM delete deploy directory
if exist deploy rd /q /s deploy
mkdir deploy

REM compile web service
pushd dotnet\Secullum.Internationalization.WebService
call dotnet restore
call dotnet publish -c Release
popd

REM copy files
xcopy /y /e "dotnet\Secullum.Internationalization.WebService\bin\Release\netcoreapp3.1\publish\*" "deploy\"
