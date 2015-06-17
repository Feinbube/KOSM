#!/usr/bin/env bash

source ./Configure_KSP_Drive_and_Directory.sh

BACKUPDIR=$KSPDIR/KSP_BACKUP_BY_KOSM

mkdir "$BACKUPDIR" 
mkdir "$BACKUPDIR/GameData"
mkdir "$BACKUPDIR/GameData/Squad"
mkdir "$BACKUPDIR/GameData/Squad/PartList"
mkdir "$BACKUPDIR/GameData/Squad/Contracts"
mkdir "$BACKUPDIR/GameData/Squad/Agencies"
mkdir "$BACKUPDIR/Ships"
mkdir "$BACKUPDIR/Ships/SPH"
mkdir "$BACKUPDIR/Ships/VAB"

mv "$KSPDIR/GameData/Squad/Flags" "$BACKUPDIR/GameData/Squad"
mv "$KSPDIR/GameData/Squad/Parts" "$BACKUPDIR/GameData/Squad"
mv "$KSPDIR/GameData/Squad/Spaces" "$BACKUPDIR/GameData/Squad"
mv "$KSPDIR/GameData/Squad/Sounds" "$BACKUPDIR/GameData/Squad"
mv "$KSPDIR/GameData/Squad/Contracts/Icons" "$BACKUPDIR/GameData/Squad/Contracts"
mv "$KSPDIR/GameData/Squad/PartList/SimpleIcons" "$BACKUPDIR/GameData/Squad/PartList"

mv "$KSPDIR/Ships/SPH/"* "$BACKUPDIR/Ships/SPH"
mv "$KSPDIR/Ships/VAB/"* "$BACKUPDIR/Ships/VAB"
mv "$KSPDIR/GameData/Squad/Agencies/"*.dds "$BACKUPDIR/GameData/Squad/Agencies"
mv "$KSPDIR/GameData/Squad/Agencies/"*.truecolor "$BACKUPDIR/GameData/Squad/Agencies"