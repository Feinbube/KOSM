call "Configure KSP Drive and Directory.bat"

xcopy .\Release "%KSPDIR%\GameData\KOSM" /D /E /C /R /I /K /Y
xcopy .\Tests\saves "%KSPDIR%\saves" /D /E /C /R /I /K /Y

xcopy .\Tests\Parts "%KSPDIR%\GameData\KOSMTest\Parts" /D /E /C /R /I /K /Y

%KSDRIVE%
cd %KSPDIR%
call ksp.exe