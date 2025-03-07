@setlocal
@pushd %~dp0

@set _C=Debug
:parse_args
@if /i "%1"=="release" set _C=Release
@if /i "%1"=="inc" set _INC=1
@if /i "%1"=="clean" set _CLEAN=1
@if not "%1"=="" shift & goto parse_args

:: Clean

@if "%_INC%"=="" call :clean
@if NOT "%_CLEAN%"=="" goto :end

@echo UI.wixext build %_C%

:: Build
msbuild -Restore -p:Configuration=%_C% || exit /b

:: Test
:: dotnet test -c %_C% --no-build test\WixToolsetTest.UI || exit /b

:: Pack
msbuild -t:Pack -p:Configuration=%_C% -p:NoBuild=true wixext\WixToolset.UI.wixext.csproj || exit /b

@goto :end

:clean
@rd /s/q "..\..\..\build\UI.wixext" 2> nul
@del "..\..\..\build\artifacts\WixToolset.UI.wixext.*.nupkg" 2> nul
@rd /s/q "%USERPROFILE%\.nuget\packages\wixtoolset.ui.wixext" 2> nul
@exit /b

:end
@popd
@endlocal
