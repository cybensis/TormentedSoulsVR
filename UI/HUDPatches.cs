﻿using HarmonyLib;
using Rewired;
using UnityEngine;
using Valve.VR;
using UnityEngine.Events;


using System.Collections.Generic;
using UnityEngine.EventSystems;
using TS.Gameplay.Menu;
using System.Diagnostics;
using TS.Items;

namespace TormentedSoulsVR.UI
{
    [HarmonyPatch]
    internal class HUDPatches
    {
        private const int NUM_OF_3D_OBJ_CHILDREN = 6;
        public static Canvas hudCanvas;

        private static bool buttonAlreadyPressed = false;

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

            }
        }


        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(GameplayMenuManager), "IsMouseInput")]
        //private static void FixHUDPosidtion(GameplayMenuManager __instance)
        //{
        //    UnityEngine.Debug.LogWarning(new StackTrace().GetFrame(1).GetMethod().Name);
        //    UnityEngine.Debug.LogWarning(new StackTrace().GetFrame(2).GetMethod().Name);
        //    //UnityEngine.Debug.LogWarning(new StackTrace().GetFrame(3).GetMethod().Name);
        //}


        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(ViewExaminateMenu), "Open")]
        //private static void Disable(GameplayMenuFakeCursor __instance)
        //{
           
        //}


        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameplayMenuFakeCursor), "UpdatePosition")]
        private static bool FixHUDPoswidwtion(GameplayMenuFakeCursor __instance, Vector3 deltaPosition)
        {
            if (__instance.transform.root.name == "MainCanvas")
                return true;
            else if (UserInputManager.InputEnabled)
                __instance.transform.localPosition = new Vector3(Mathf.Clamp(deltaPosition.x, -2000, 2000), Mathf.Clamp(deltaPosition.y, -700, 1000), 0);
            return false;
        }





        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameplayMenuFakeCursor), "Update")]
        private static bool FixHUDPoswidwwtiown(GameplayMenuFakeCursor __instance)
        {

            if (!__instance.panelEnabled)
                return false;
            Vector3 fwd = __instance.transform.TransformDirection(Vector3.forward);
            RaycastHit hit;
            IFakeClick fakeClick = null;
            IFakeClick fakeClick2 = __instance.lastElement;
            if (Physics.Raycast(__instance.transform.position, fwd, out hit, 1) && hit.collider.GetComponent<IFakeClick>() != null) { 
                fakeClick = hit.collider.GetComponent<IFakeClick>();
                if (__instance.lastElement != null)
                    __instance.lastElement.MouseExit(null);
                __instance.lastElement = fakeClick;
                fakeClick.MouseEnter(null);
                __instance.SetCursorType(GameplayMenuFakeCursor.PointerType.Magnify);


            }
            else if (__instance.lastElement != null)
            {
                __instance.SetCursorType(GameplayMenuFakeCursor.PointerType.Normal);
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



        //[HarmonyPostfix]
        //[HarmonyPatch(typeof(Gui3DObject), "SetItem")]
        //private static void FixInspectObject(Gui3DObject __instance) {
        //    //UnityEngine.Debug.LogWarning(__instance.m_currentGo);
        //    __instance.transform.GetChild(1).localScale = new Vector3(2222, 2222, 2222);
        //    if (__instance.transform.GetChild(1).GetChild(0) != null)
        //        __instance.transform.GetChild(1).GetChild(0).localScale = new Vector3(1, 1, 1);

        //}

        private static bool itemResized = false;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Gui3DObject), "OnItemInReady")]
        private static void kk(Gui3DObject __instance)
        {
            GameObject currentObject = __instance.m_currentGo;
            if (currentObject != null && !itemResized )
            {
                itemResized = true;
                currentObject.transform.localScale *= 225;
                currentObject.transform.parent.localPosition = new Vector3(0, 100, 125);
            }
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(Gui3DObject), "Close")]
        private static void ResetResizeVar(Gui3DObject __instance)
        {
            itemResized = false;
        }


        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(GM_StateNormal), "OnItemOnGridSelected")]
        //private static void ResetResizeddVar(GM_StateNormal __instance, ButtonItemGameplayMenuBehaviour itemSelected)
        //{
        //    UnityEngine.Debug.LogError(itemSelected);
        //    UnityEngine.Debug.LogError(new StackTrace().GetFrame(1).GetMethod().Name);
        //    UnityEngine.Debug.LogError(new StackTrace().GetFrame(2).GetMethod().Name);
        //}

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ViewItemGridBehaviour), "SetItemMenu")]
        private static void ResetResdfizeddVar(ViewItemGridBehaviour __instance, ButtonItemGameplayMenuBehaviour ItemButtonBehaviour)
        {
            UnityEngine.Debug.LogError(ItemButtonBehaviour);
            UnityEngine.Debug.LogWarning(new StackTrace().GetFrame(1).GetMethod().Name + " " + new StackTrace().GetFrame(2).GetMethod().Name);
        }

        // LOOK INTO GM_StateNormal.OnItemOnGridSelected
        //private void OnItemOnGridSelected(ButtonItemGameplayMenuBehaviour itemSelected)
        //{
        //    if (isCombining)
        //    {
        //        if (currentSelectedGrid != itemSelected)
        //        {
        //            currentSelectedGrid?.DeselectButton();
        //        }
        //        currentSelectedGrid = itemSelected;
        //        base.m_itemOnExaminatePanel = itemSelected.ItemData;
        //        CollectableGuiItem itemOnGridSelected = m_itemOnGridSelected;
        //        CollectableGuiItem itemOnExaminatePanel = base.m_itemOnExaminatePanel;
        //        if (CollectableGuiItem.ItemsCanBeConsumedBetweenThem(itemOnExaminatePanel, itemOnGridSelected))
        //        {
        //            ConsumeItemsBetweenThemOnGrid(itemOnExaminatePanel, itemOnGridSelected);
        //            isCombining = false;
        //            DeselectView();
        //        }
        //        else
        //        {
        //            OnMergeItems();
        //        }
        //        isItemGrabbed = false;
        //        m_menuView.Cursor.ResetDefaultSprite();
        //        return;
        //    }
        //    if (m_isUsingItem)
        //    {
        //        itemSelected.DeselectButton();
        //        return;
        //    }
        //    if (currentSelectedGrid != itemSelected)
        //    {
        //        currentSelectedGrid?.DeselectButton();
        //    }
        //    currentSelectedGrid = itemSelected;
        //    CollectableGuiItem collectableGuiItem = (m_itemOnGridSelected = itemSelected.ItemData);
        //    List<ItemMenuOptionBehaviour.ButtonDefinition> list = new List<ItemMenuOptionBehaviour.ButtonDefinition>();
        //    if (collectableGuiItem.IsItem || collectableGuiItem.IsResource)
        //    {
        //        if (collectableGuiItem.IsWeapon && collectableGuiItem.GetSaveData() is CollectableItemSaveData collectableItemSaveData && !isPuzzleZoomMenu)
        //        {
        //            if (collectableItemSaveData.IsEquiped)
        //            {
        //                list.Add(new ItemMenuOptionBehaviour.ButtonDefinition("Unequip_Gui", "Unequip"));
        //            }
        //            else
        //            {
        //                list.Add(new ItemMenuOptionBehaviour.ButtonDefinition("equip", "Equip"));
        //            }
        //        }
        //        if (collectableGuiItem.IsHealth && collectableGuiItem.GetSaveData() is CollectableItemSaveData)
        //        {
        //            _ = (CollectableItemSaveData)collectableGuiItem.GetSaveData();
        //            list.Add(new ItemMenuOptionBehaviour.ButtonDefinition("heal", "Heal"));
        //        }
        //        if (collectableGuiItem.IsUsable && m_enabledUseItemOnWorld)
        //        {
        //            list.Add(new ItemMenuOptionBehaviour.ButtonDefinition("use_item", "Grab"));
        //        }
        //    }
        //    list.Add(new ItemMenuOptionBehaviour.ButtonDefinition("examinate_item", "Examinate"));
        //    if (!collectableGuiItem.IsArchive)
        //    {
        //        list.Add(new ItemMenuOptionBehaviour.ButtonDefinition("combine_item", "Combine"));
        //    }
        //    if (collectableGuiItem.IsSkin)
        //    {
        //        list.Add(new ItemMenuOptionBehaviour.ButtonDefinition("change_clothes", "Change Clothes"));
        //    }
        //    m_menuView.currentOption = itemSelected.navButton;
        //    m_menuView.ItemGrid.MenuItemOption.navItemReference.resolveDestination = (NavItemButton)m_menuView.currentOption;
        //    NavItemButton currentOption = m_menuView.ItemGrid.MenuItemOption.OpenMenu(list, OnMenuOptionSelected, "SubMenuItem");
        //    m_menuView.currentOption = currentOption;
        //    LeanTween.value(m_menuView.currentOption.GetGameObject(), 0f, 0f, 0.05f).setOnComplete(m_menuView.PlaceCursorInCurrentOption);
        //}






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
            __instance.Cursor.MouseOff = false;
            //__instance.Cursor.transform.localPosition;
            Vector3 newPos = __instance.Cursor.transform.localPosition;
            newPos.x += SteamVR_Actions._default.LeftJoystick.axis.x * (600 * Time.deltaTime);
            newPos.y += SteamVR_Actions._default.LeftJoystick.axis.y * (600 * Time.deltaTime);
            __instance.Cursor.UpdatePosition(newPos);
            //return false;
            //if (__instance.notInGrid)
            //{
            //}
            //__instance.DPADInput.x = (SteamVR_Actions._default.LeftJoystick.axis.x > 0.7) ? 1 : ((SteamVR_Actions._default.LeftJoystick.axis.x < -0.7) ? -1 : 0);
            //__instance.DPADInput.y = (SteamVR_Actions._default.LeftJoystick.axis.y > 0.7) ? 1 : ((SteamVR_Actions._default.LeftJoystick.axis.y < -0.7) ? -1 : 0);
            //bool flag2 = __instance.lastDPADInput != __instance.DPADInput;
            //if (flag2)
            //{
            //    if (__instance.DPADInput.sqrMagnitude != 0f)
            //    {
            //        __instance.Cursor.MouseOff = true;
            //    }
            //    __instance.DigitalMovement();
            //}
            __result = __instance.Cursor.MousePosition;
            //if (__instance.notInGrid) {
            //    __instance.Cursor.MouseOff = false;
            //    //__instance.Cursor.transform.localPosition;
            //    Vector3 newPos = __instance.Cursor.transform.localPosition;
            //    newPos.x += SteamVR_Actions._default.LeftJoystick.axis.x * (400 * Time.deltaTime);
            //    newPos.y += SteamVR_Actions._default.LeftJoystick.axis.y * (400 * Time.deltaTime);
            //    __instance.Cursor.UpdatePosition(newPos );
            //    return false;
            //}
            //__instance.DPADInput.x = (SteamVR_Actions._default.LeftJoystick.axis.x > 0.7) ? 1 : ((SteamVR_Actions._default.LeftJoystick.axis.x < -0.7) ? -1 : 0);
            //__instance.DPADInput.y = (SteamVR_Actions._default.LeftJoystick.axis.y > 0.7) ? 1 : ((SteamVR_Actions._default.LeftJoystick.axis.y < -0.7) ? -1 : 0);
            //bool flag2 = __instance.lastDPADInput != __instance.DPADInput;
            //if (flag2)
            //{
            //    if (__instance.DPADInput.sqrMagnitude != 0f)
            //    {
            //        __instance.Cursor.MouseOff = true;
            //    }
            //    __instance.DigitalMovement();
            //}
            //__result = __instance.Cursor.MousePosition;
            if (!buttonAlreadyPressed && SteamVR_Actions._default.ButtonA.stateDown && __instance.currentOption != null)
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
                        if (optionButton != null)
                            optionButton.OnMouseClick();
                        break;
                }

            }
            else if (SteamVR_Actions._default.ButtonA.stateUp)
                buttonAlreadyPressed = false;


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
