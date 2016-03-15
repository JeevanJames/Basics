@ECHO OFF

REM Updates static and dynamic dependencies for a solution.
REM This file must be copied to the directory where the solution file resides, typically the root directory.
REM The NuGet command-line tool should be available on the path for this to work.

REM Look for the solution file in the current directory and set it to the SolutionFile variable.
REM If more than one solution files are found, use the last one.
SET SolutionFile=
FOR %%S IN (*.SLN) DO SET SolutionFile="%%S"
IF NOT EXIST "%SolutionFile%" GOTO MissingSolution

REM Update static NuGet dependencies.
ECHO Updating solution dependencies for %SolutionFile%
NUGET UPDATE "%SolutionFile%"
ECHO.

REM If a subdirectory named 'drops' exists, then we have dynamic packages. Update them as well.
IF NOT EXIST drops GOTO NoDrops
ECHO Updating drop dependencies
NUGET INSTALL drops\packages.config -SolutionDirectory drops
NUGET UPDATE drops\Drops.sln

GOTO End

:MissingSolution
ECHO Cannot find any solution files (.sln) in the current directory.
GOTO End

:NoDrops
ECHO drops folder not found. Skipping drops updates.
GOTO End

:End
PAUSE
