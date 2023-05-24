using HarmonyLib;
using Rewired;
using UnityEngine;
using Valve.VR;
using static UnityEngine.UIElements.UIRAtlasAllocator;
using System;

namespace TormentedSoulsVR.UI
{
    [HarmonyPatch]
    internal class HUDPatches
    {
        public static Canvas hudCanvas;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameplayMenuManager), "InitialSetup")]
        private static void FixHUDPosition(GameplayMenuManager __instance) { 
            if (CamFix.camRoot != null)
            {
                __instance.transform.parent = CamFix.camRoot.transform;
                __instance.transform.localScale = new Vector3(0.0003f, 0.0003f, 0.0003f);
                __instance.transform.localPosition = new Vector3(-0.1f, 1.575f, 0.5f);
                __instance.transform.localRotation = Quaternion.identity;
                __instance.transform.rotation = Quaternion.identity;
                __instance.m_normalMenuView.Gui3DObject_Animator.gameObject.SetActive(false);
                hudCanvas = __instance.m_normalMenuView.m_generalCanvas.GetComponent<Canvas>();
                hudCanvas.renderMode = RenderMode.WorldSpace;
                hudCanvas.transform.localPosition = Vector3.zero;

            }
        }



        // This is called by GM_StateOptionsMenu I think, a lot of the input handling and method calling seems to come from the GM_ classes

   

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameplayMenuGeneralView), "UpdateCursorPosition")]
        private static bool AllowHUDMowvement(GameplayMenuGeneralView __instance, ref Vector3 __result)
        {
            __instance.DPADInput.x = (SteamVR_Actions._default.LeftJoystick.axis.x > 0.7) ? 1 : ((SteamVR_Actions._default.LeftJoystick.axis.x < -0.7) ? -1 : 0);
            __instance.DPADInput.y = (SteamVR_Actions._default.LeftJoystick.axis.y > 0.7) ? 1 : ((SteamVR_Actions._default.LeftJoystick.axis.y < -0.7) ? -1 : 0);
            bool flag2 = __instance.lastDPADInput != __instance.DPADInput;
            if (flag2)
            {
                if (__instance.DPADInput.sqrMagnitude != 0f)
                {
                    __instance.Cursor.MouseOff = true;
                }
                __instance.DigitalMovement();
            }
            __result = __instance.Cursor.MousePosition;
            if (SteamVR_Actions._default.ButtonA.stateDown)
            {
                NavItemButton curSelection = (NavItemButton)__instance.currentOption;
                ButtonType curSelectionType = curSelection.buttonType;
                GameObject curGameObject = curSelection.GetGameObject();

                switch (curSelectionType)
                {
                    case ButtonType.ItemGridButton:
                        curGameObject.GetComponent<ButtonItemGameplayMenuBehaviour>().EnablePressedMenu(true);
                        // Use itemGridButtonComponent as needed
                        break;
                    case ButtonType.SubmenuButton:
                        curGameObject.GetComponent<ItemOptionMenuButtonBehaviour>().FakeOnClick();
                        // Use submenuButtonComponent as needed
                        break;
                    case ButtonType.TopPanelButton:
                        curGameObject.GetComponent<TopPanelButton>().OnMouseClick();
                        // Use topPanelButtonComponent as needed
                        break;
                    case ButtonType.DoneButton:
                        var doneButtonComponent = curGameObject.GetComponent<DoneButton>();
                        // Use doneButtonComponent as needed
                        break;
                    case ButtonType.OptionMenuButton:
                        var optionMenuButtonComponent = curGameObject.GetComponent<OptionMenuButton>();
                        // Use optionMenuButtonComponent as needed
                        break;
                    case ButtonType.MainMenuButton:
                        var mainMenuButtonComponent = curGameObject.GetComponent<MainMenuButton>();
                        // Use mainMenuButtonComponent as needed
                        break;
                    case ButtonType.GenericButton:
                        var genericButtonComponent = curGameObject.GetComponent<GenericButton>();
                        // Use genericButtonComponent as needed
                        break;
                }
            }
            return false;
        }






        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(GameplayMenuGeneralView), "DigitalMovement")]
        ////private static bool AllowHUDMowvement(GameplayMenuGeneralView __instance, ref Vector3 __result) {
        //private static void AllowHUDMowvement(GameplayMenuGeneralView __instance)
        //{
        //    int rows = __instance.GetRows();
        //    int num = ((NavItemButton)__instance.m_currentOption).gridID / 3 + 1;
        //    float num2 = 4f;
        //    float to = 1f - Math.Max((float)num - num2, 0f) / Mathf.Max(1f, (float)rows - num2);
        //    Debug.LogWarning(to + "      " + num);
        //    // to should be 1 but if the inv window has so many items you need to scroll down it will go to 0, num should be the column

        //}

        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(GameplayMenuGeneralView), "PlaceCursorInCurrentOption")]
        ////private static bool AllowHUDMowvement(GameplayMenuGeneralView __instance, ref Vector3 __result) {
        //private static void AllowHUDModwvement(GameplayMenuGeneralView __instance) {
        //    Debug.LogError(__instance.m_menuManager.IsMouseInput() + "     " + __instance.m_currentOption.GetTransform().position);
        //    // mouseinput should always be false and the position is pretty much always (0.3, 1.5, -3.8) to (0.4, 1.5, -3.8) but thats a world position 
        //}

    }
}
