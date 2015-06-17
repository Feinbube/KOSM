#!/usr/bin/env bash

source ./Configure_KSP_Drive_and_Directory.sh

rm -rf "$KSPDIR/GameData/KOSM"
rm -rf "$KSPDIR/GameData/KOSMTest/Parts" 

cp -R ./Release "$KSPDIR/GameData/KOSM"
cp -R ./Tests/saves "$KSPDIR/saves"

mkdir "$KSPDIR/GameData/KOSMTest" 2>/dev/null
cp -R ./Tests/Parts "$KSPDIR/GameData/KOSMTest/Parts"

open "$KSPDIR/KSP.app"