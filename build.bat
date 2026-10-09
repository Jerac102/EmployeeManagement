@echo off
setlocal

set CONFIGURATION=Release
set ROOT=%~dp0
set OUTPUT=%ROOT%bin

echo Cleaning output...
if exist "%OUTPUT%" rmdir /s /q "%OUTPUT%"

echo.
echo Publishing EmployeeManagement.UI...
dotnet publish "%ROOT%src\EmployeeManagement.UI\EmployeeManagement.UI.csproj" ^
    -c %CONFIGURATION% ^
    -o "%OUTPUT%\EmployeeManagement.UI"

if errorlevel 1 goto :error

echo.
echo Publishing EmployeeManagement.Api...
dotnet publish "%ROOT%src\EmployeeManagement.Api\EmployeeManagement.Api.csproj" ^
    -c %CONFIGURATION% ^
    -o "%OUTPUT%\EmployeeManagement.Api"

if errorlevel 1 goto :error

echo.
echo Publishing EmployeeManagement.Web...
dotnet publish "%ROOT%src\EmployeeManagement.Web\EmployeeManagement.Web.csproj" ^
    -c %CONFIGURATION% ^
    -o "%OUTPUT%\EmployeeManagement.Web"

if errorlevel 1 goto :error

echo.
echo ========================================
echo Publish completed successfully.
echo Output: %OUTPUT%
echo ========================================
exit /b 0

:error
echo.
echo ========================================
echo Publish FAILED.
echo ========================================
exit /b 1