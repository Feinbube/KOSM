set KSDRIVE=C:
set KSPDIR=%KSDRIVE%"\Program Files (x86)\Steam\steamapps\common\Kerbal Space Program"

xcopy .\Release %KSPDIR%\GameData\KOSM /D /E /C /R /I /K /Y
%KSDRIVE%
cd %KSPDIR%
call ksp.exe