using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TormentedSoulsVR.VRBody;
using UnityEngine;

namespace TormentedSoulsVR.weapons
{
    [HarmonyPatch]
    internal class weaponPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ShootgunAimToBrother), "LateUpdate")]
        private static bool DisableWeaponRotationBlocking(PlayerStepsReceiver __instance)
        {
            // LateUpdate in this class makes it so the weapon is always pointing forward so the tip can never go up or down,
            // so replace the LateUpdate function so it does nothing
            return false;
        }



        [HarmonyPrefix]
        [HarmonyPatch(typeof(NailerPullerWeaponBehaviour), "OnTriggerEnter")]
        private static bool DisableNormalCrowbarAttackBehaviour(NailerPullerWeaponBehaviour __instance)
        {
            // LateUpdate in this class makes it so the weapon is always pointing forward so the tip can never go up or down,
            // so replace the LateUpdate function so it does nothing
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(WeaponBase), "PositionParts")]
        private static bool AddVRCrouch(WeaponBase __instance, Transform mountPointLeft, Transform mountPointRight)
        {
            if (__instance.name == "Lighter_equip(Clone)")
                return true;
            if ((bool)__instance.LeftHandMesh)
            {
                __instance.SetHandBone(mountPointRight, __instance.LeftHandMesh.transform);
            }
            if ((bool)__instance.RightHandMesh)
            {
                __instance.SetHandBone(mountPointRight, __instance.RightHandMesh.transform);
            }
            if (__instance.name == "ShotGunEquip(Clone)")
            {
                __instance.LeftHandMesh.transform.localPosition = new Vector3(0.0777f, -0.0276f, 0.058f);
                __instance.LeftHandMesh.transform.localRotation = Quaternion.Euler(345, 340, 18);

                __instance.RightHandMesh.transform.localPosition = new Vector3(-0.1251f, 0.2308f, -0.0144f);
                __instance.RightHandMesh.transform.localRotation = Quaternion.Euler(354, 358, 4);
            }
            else if (__instance.name == "ElectricalStun_Equip(Clone)")
                __instance.LeftHandMesh.transform.localPosition = new Vector3(0.04f, 0.02f, 0.03f);
            else if (__instance.name == "NailerEquip(Clone)")
            {
                __instance.LeftHandMesh.transform.localPosition = new Vector3(0.06f, 0.01f, 0f);
                __instance.LeftHandMesh.transform.localRotation = Quaternion.Euler(0, 345, 0);

            }
            else if (__instance.name == "Crowbar_Equip(Clone)") { 
                if ((__instance.m_weaponBehaviour as NailerPullerWeaponBehaviour).ColliderEventSender.gameObject.GetComponent<VRCrowbar>() == null)
                    (__instance.m_weaponBehaviour as NailerPullerWeaponBehaviour).ColliderEventSender.gameObject.AddComponent<VRCrowbar>();
                (__instance.m_weaponBehaviour as NailerPullerWeaponBehaviour).ColliderEventSender.transform.parent = (__instance.m_weaponBehaviour as NailerPullerWeaponBehaviour).ColliderEventSender.transform.parent.parent;
            }
            return false;
        }
    }
}
