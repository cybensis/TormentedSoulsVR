using HarmonyLib;
using TormentedSoulsVR.VRBody;
using UnityEngine;

namespace TormentedSoulsVR.body
{
    [HarmonyPatch]
    internal class BodyPatches
    {

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerStepsReceiver), "Setup")]
        private static void AddVRCrouch(PlayerStepsReceiver __instance)
        {
            GameObject hips = __instance.leftFoot.transform.parent.parent.parent.parent.gameObject;
            if (CamFix.crouchInstance == null) { 
                CamFix.crouchInstance = hips.AddComponent<VRCrouch>();
                CamFix.crouchInstance.SetBodyRoot(__instance.transform);
            }
        }

    }
}
