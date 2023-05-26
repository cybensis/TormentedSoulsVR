using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TormentedSoulsVR.VRBody
{
    internal class ArmIK
    {
        // Patch to this
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerWeaponBehaviour), "InitialSetup")]
        private static void FixHUDPoswwidwtion(PlayerWeaponBehaviour __instance)
        {
            //RIGHT HAND = __instance.RightHandWeaponPoint.transform.parent
            // From right hand go up 5 parents to reach shoulder
            // 1st and third parent are some weird duplicate of forearm and upper arm

            // Same for left hand = __instance.LeftHandWeaponPoint.transform.parent
        
        }
    }
}
