@echo off
echo Cleaning previous build...
if exist obj rmdir /s /q obj
if exist bin rmdir /s /q bin
if exist publish rmdir /s /q publish

echo Restoring packages...
dotnet restore AstralView.csproj
if errorlevel 1 goto error

echo Publishing self-contained executable...
dotnet publish AstralView.csproj --configuration Release --runtime win-x64 --self-contained true --output ./publish
if errorlevel 1 goto error

echo Copying Tools folder...
if not exist publish\Tools mkdir publish\Tools
xcopy /E /I /Y Tools publish\Tools

echo.
echo Build completed successfully!
echo Output is in the 'publish' folder
goto end

:error
echo.
echo Build failed! Check the errors above.
exit /b 1

:end
