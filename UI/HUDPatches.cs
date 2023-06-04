using HarmonyLib;
using Rewired;
using UnityEngine;
using Valve.VR;
using UnityEngine.Events;


using System.Collections.Generic;
using UnityEngine.EventSystems;
using TS.Gameplay.Menu;
using System.Diagnostics;
using TS.Items;
using UnityEngine.SceneManagement;
using FlowDirector.Nodes.Gameplay.Menu;

namespace TormentedSoulsVR.UI
{
    [HarmonyPatch]
    internal class HUDPatches
    {
        private const int NUM_OF_3D_OBJ_CHILDREN = 6;


        private static float raycastLength = 5;
        public static Canvas hudCanvas;

        private static bool buttonAlreadyPressed = false;
        private static bool leftJoyUsedLast = false;

        public static Vector3 HUD_POSITION = new Vector3(0f, 1.575f, 0.35f);

        public static float targetHUDYRot = 0;


        // When the inv or pause menu is opened set its position and rotation relative to the VR cameras
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameplayMenuGeneralView), "Open")]
        private static void SetHUDRotationOnOpen(GameplayMenuGeneralView __instance)
        {
            if (!CamFix.inCinematic && CamFix.menus != null) {
                //targetHUDYRot =  CamFix.vrCamera.transform.localEulerAngles.y - CamFix.camHolder.transform.localEulerAngles.y;
                targetHUDYRot = CamFix.vrCamera.transform.localEulerAngles.y;
                CamFix.menus.transform.position = CamFix.vrCamera.transform.position + (CamFix.vrCamera.transform.forward * 0.35f);
            }
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameplayMenuGeneralView), "Close")]
        private static void SetHUDRotationOnClose(GameplayMenuGeneralView __instance)
        {
            targetHUDYRot = 0f;
            if (!CamFix.inCinematic && CamFix.menus != null)
                CamFix.menus.transform.localPosition = HUD_POSITION;
        }

        // 
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameplayMenuManager), "InitialSetup")]
        private static void FixHUDPosition(GameplayMenuManager __instance) { 
            if (CamFix.camRoot != null)
            {

                __instance.transform.parent = CamFix.camRoot.transform;
                if (CamFix.camRoot.transform.childCount >= 3)
                    UnityEngine.Object.Destroy(CamFix.camRoot.transform.GetChild(1).gameObject);
                CamFix.menus = __instance;
                RectTransform mainCanvas = (RectTransform)__instance.m_normalMenuView.canvas_Animator.transform;
                mainCanvas.localScale = new Vector3(0.6667f, 0.6667f, 0.6667f);
                mainCanvas.sizeDelta = new Vector2(3840, 2160);
                mainCanvas.anchoredPosition3D = new Vector3(0, 0, 0);

                __instance.transform.localScale = new Vector3(0.0003f, 0.0003f, 0.0003f);
                // In the intro the HUD needs to be at a lower pos
                if (SceneManager.GetActiveScene().name == "IntroScene")
                    __instance.transform.localPosition = new Vector3(0,0,0.3f);
                else
                    __instance.transform.localPosition = HUD_POSITION;
                __instance.transform.localRotation = Quaternion.identity;
                __instance.transform.rotation = Quaternion.identity;
                hudCanvas = __instance.m_normalMenuView.m_generalCanvas.GetComponent<Canvas>();
                hudCanvas.renderMode = RenderMode.WorldSpace;
                hudCanvas.transform.localPosition = Vector3.zero;

                // This is for the 3D inspector window which has a bunch of lights which completely illuminate the room so disable them
                if (__instance.m_normalMenuView.Gui3DObject_Animator.transform.childCount == NUM_OF_3D_OBJ_CHILDREN) {
                    __instance.m_normalMenuView.Gui3DObject_Animator.transform.GetChild(2).gameObject.SetActive(false);
                    __instance.m_normalMenuView.Gui3DObject_Animator.transform.GetChild(3).gameObject.SetActive(false);
                    __instance.m_normalMenuView.Gui3DObject_Animator.transform.GetChild(4).gameObject.SetActive(false);
                    __instance.m_normalMenuView.Gui3DObject_Animator.transform.GetChild(5).gameObject.SetActive(false);
                }

                Canvas altCanvas = __instance.transform.GetChild(4).GetComponent<Canvas>();
                altCanvas.renderMode = RenderMode.WorldSpace;
                altCanvas.transform.localScale = Vector3.one;
                altCanvas.transform.localPosition = Vector3.zero;

                Canvas shadowCanvas = __instance.transform.GetChild(5).GetComponent<Canvas>();
                shadowCanvas.renderMode = RenderMode.WorldSpace;
                shadowCanvas.transform.localScale = Vector3.one;
                shadowCanvas.transform.localPosition = Vector3.zero;


                // Disable the renderer for the small cube in the 3D inspect menu
                __instance.transform.GetChild(2).GetChild(1).GetComponent<MeshRenderer>().enabled = false;
            }
        }





        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameplayMenuFakeCursor), "UpdatePosition")]
        private static bool FixFakeCursorMovement(GameplayMenuFakeCursor __instance, Vector3 deltaPosition)
        {
            if (__instance.transform.root.name == "MainCanvas")
                return true;
            else if (UserInputManager.InputEnabled)
                __instance.transform.localPosition = new Vector3(Mathf.Clamp(deltaPosition.x, -2000, 2000), Mathf.Clamp(deltaPosition.y, -700, 1000), 0);
            return false;
        }





        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameplayMenuFakeCursor), "Update")]
        private static bool EnableRaycastForFakeCursor(GameplayMenuFakeCursor __instance)
        {

            if (!__instance.panelEnabled && !leftJoyUsedLast)
                return false;
            Vector3 fwd = __instance.transform.TransformDirection(Vector3.forward);
            RaycastHit hit;
            IFakeClick fakeClick = null;
            IFakeClick fakeClick2 = __instance.lastElement;
            if (Physics.Raycast(__instance.transform.position, fwd, out hit, raycastLength) && hit.collider.GetComponent<IFakeClick>() != null) { 
                fakeClick = hit.collider.GetComponent<IFakeClick>();
                if (__instance.lastElement != null)
                    __instance.lastElement.MouseExit(null);
                __instance.lastElement = fakeClick;
                fakeClick.MouseEnter(null);
                //__instance.SetCursorType(GameplayMenuFakeCursor.PointerType.Magnify);


            }
            else if (__instance.lastElement != null)
            {
                //__instance.SetCursorType(GameplayMenuFakeCursor.PointerType.Normal);
                __instance.lastElement.MouseExit(null);
                __instance.lastElement = null;
            }

            if (fakeClick == null && fakeClick2 != null)
                __instance.MouseExitIFakeClick(fakeClick2);
            else if (fakeClick2 == null && fakeClick != null)
                __instance.MouseEnterIFakeClick(fakeClick);
            else if (fakeClick != null && fakeClick != fakeClick2)
                __instance.MouseEnterIFakeClick(fakeClick, fakeClick2);

            return false;
        }




        private static bool itemResized = false;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Gui3DObject), "OnItemInReady")]
        private static void ResizeInspectedObject(Gui3DObject __instance)
        {
            GameObject currentObject = __instance.m_currentGo;
            if (currentObject != null && !itemResized )
            {
                itemResized = true;
                currentObject.transform.localScale *= 225;
                currentObject.transform.parent.localPosition = new Vector3(0, 100, 160);
            }
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(Gui3DObject), "Close")]
        private static void ResetResizeVar(Gui3DObject __instance)
        {
            itemResized = false;
        }


 







        [HarmonyPostfix]
        [HarmonyPatch(typeof(ImageViewRotation), "Start")]
        private static void DisableInspectCanvasItem(ImageViewRotation __instance)
        {
            if (__instance.transform.parent.name == "ObjectView")
                __instance.GetComponent<UnityEngine.UI.RawImage>().enabled = false;
                //__instance.transform.parent.gameObject.SetActive(false);
        }



        // This is called by GM_StateOptionsMenu I think, a lot of the input handling and method calling seems to come from the GM_ classes
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameplayMenuGeneralView), "UpdateCursorPosition")]
        private static bool AllowHUDMowvement(GameplayMenuGeneralView __instance, ref Vector3 __result)
        {


            //__instance.Cursor.transform.localPosition;
            Vector3 newPos = __instance.Cursor.transform.localPosition;
            float xAxis = SteamVR_Actions._default.RightJoystick.axis.x;
            float yAxis = SteamVR_Actions._default.RightJoystick.axis.y;
            if (__instance.isExaminateItemPanelOpen) {
                xAxis = SteamVR_Actions._default.LeftJoystick.axis.x;
                yAxis = SteamVR_Actions._default.LeftJoystick.axis.y;
            }
            newPos.x += xAxis * (600 * Time.deltaTime);
            newPos.y += yAxis * (600 * Time.deltaTime);
            if (xAxis != 0 || yAxis != 0)
            {
                leftJoyUsedLast = true;
                __instance.Cursor.UpdatePosition(newPos);
                __instance.Cursor.MouseOff = false;
            }
            else { 
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
                    leftJoyUsedLast = false;
                }
            }

            __result = __instance.Cursor.MousePosition;
            if (!leftJoyUsedLast && !buttonAlreadyPressed && SteamVR_Actions._default.ButtonA.stateDown && __instance.currentOption != null)
            {
                buttonAlreadyPressed = true;
                NavItemButton curSelection = (NavItemButton)__instance.currentOption;
                ButtonType curSelectionType = curSelection.buttonType;
                GameObject curGameObject = curSelection.GetGameObject();

                switch (curSelectionType)
                {
                    case ButtonType.ItemGridButton:
                        curGameObject.GetComponent<ButtonItemGameplayMenuBehaviour>().EnablePressedMenu(true);
                        break;
                    case ButtonType.SubmenuButton:
                        curGameObject.GetComponent<ItemOptionMenuButtonBehaviour>().FakeOnClick();
                        break;
                    case ButtonType.TopPanelButton:
                        curGameObject.GetComponent<TopPanelButton>().OnMouseClick();
                        break;
                    //case ButtonType.DoneButton:
                    //    var doneButtonComponent = curGameObject.GetComponent<DoneButton>();
                    //    break;
                    case ButtonType.OptionMenuButton:
                        var optionMenuButtonComponent = curGameObject.GetComponent<OptionMenuButton>();
                        break;
                    //case ButtonType.MainMenuButton:
                    //    var mainMenuButtonComponent = curGameObject.GetComponent<MainMenuButton>();
                    //    // Use mainMenuButtonComponent as needed
                    //    break;
                    case ButtonType.GenericButton:
                        var genericButtonComponent = curGameObject.GetComponent<GenericButton>();
                        // Use genericButtonComponent as needed
                        break;
                    default:
                        OptionMenuButton optionButton = curGameObject.GetComponent<OptionMenuButton>();
                        if (optionButton != null) { 
                            optionButton.OnMouseClick();
                            break;
                        }
                        MenuImageButtonBehaviour menuButton  = curGameObject.GetComponent<MenuImageButtonBehaviour>();
                        if (menuButton != null)
                        {
                            menuButton.OnMouseClick();
                            break;
                        }
                        break;
                        
                }

            }
            else if (SteamVR_Actions._default.ButtonA.stateUp)
                buttonAlreadyPressed = false;


            return false;
        }






    }
}
