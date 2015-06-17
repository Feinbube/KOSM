using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[KSPAddon(KSPAddon.Startup.MainMenu, true)]
public class KOSMModule : MonoBehaviour
{
    public KOSMModule()
    {
        KOSM.Immortal.AddImmortal<KOSM.KOSMBehaviour>();
    }
}
