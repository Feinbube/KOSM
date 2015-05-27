call "Configure KSP Drive and Directory.bat"

set BACKUPDIR=%KSPDIR%\KSP_BACKUP_BY_KOSM

mkdir "%BACKUPDIR%"
mkdir "%BACKUPDIR%\GameData"
mkdir "%BACKUPDIR%\GameData\Squad"
mkdir "%BACKUPDIR%\GameData\Squad\PartList"
mkdir "%BACKUPDIR%\GameData\Squad\Contracts"
mkdir "%BACKUPDIR%\GameData\Squad\Agencies"
mkdir "%BACKUPDIR%\Ships"
mkdir "%BACKUPDIR%\Ships\SPH"
mkdir "%BACKUPDIR%\Ships\VAB"

move "%KSPDIR%\GameData\Squad\Flags" "%BACKUPDIR%\GameData\Squad\Flags"
move "%KSPDIR%\GameData\Squad\Parts" "%BACKUPDIR%\GameData\Squad\Parts"
move "%KSPDIR%\GameData\Squad\Spaces" "%BACKUPDIR%\GameData\Squad\Spaces"
move "%KSPDIR%\GameData\Squad\Sounds" "%BACKUPDIR%\GameData\Squad\Sounds"
move "%KSPDIR%\GameData\Squad\Contracts\Icons" "%BACKUPDIR%\GameData\Squad\Contracts\Icons"
move "%KSPDIR%\GameData\Squad\PartList\SimpleIcons" "%BACKUPDIR%\GameData\Squad\PartList\SimpleIcons"

move "%KSPDIR%\Ships\SPH\*.*" "%BACKUPDIR%\Ships\SPH"
move "%KSPDIR%\Ships\VAB\*.*" "%BACKUPDIR%\Ships\VAB"
move "%KSPDIR%\GameData\Squad\Agencies\*.dds" "%BACKUPDIR%\GameData\Squad\Agencies"
move "%KSPDIR%\GameData\Squad\Agencies\*.truecolor" "%BACKUPDIR%\GameData\Squad\Agencies"
