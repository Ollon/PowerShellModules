@echo off

tools\RepositoryUtility.exe --online

echo f| xcopy "tools\Packages.props" Packages.props /y

pause