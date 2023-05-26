using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using TS.Gameplay.Menu;
using UnityEngine;
using Valve.VR;

namespace TormentedSoulsVR.UI
{
    [HarmonyPatch]
    internal class MainMenuPatches
    {
        private const float NEXT_SELECTION_DELAY = 0.3f;

        private static NavItemButton currentButton;
        private static ItemOptionMenuButtonBehaviour currentButtonBehaviour;
        private static float timeSinceMoved = NEXT_SELECTION_DELAY;
        private static bool returningFromOptionsMenu = false;

        private static BaseMenuPanelController selectedOptionsPanel;
        private static OptionMenuButtonBaseBehaviour currentOptionsButton;
        private static int currentOptionsIndex = 0;

        // Sets up the main menu screen to select the first available button
        [HarmonyPostfix]
        [HarmonyPatch(typeof(InitMenuController), "InitialSetup")]
        private static void IntialiseMainMenuSelection(InitMenuController __instance)
        {
            currentButton = (NavItemButton)__instance.navItems[0];
            currentButtonBehaviour = currentButton.GetTransform().GetComponent<ItemOptionMenuButtonBehaviour>();
            currentButtonBehaviour.SelectButton(true);
            if (CamFix.camHolder != null)
                CamFix.camHolder.transform.localPosition = Vector3.zero;
        }

        // Handles the main menu button movement
        [HarmonyPostfix]
        [HarmonyPatch(typeof(InitMenuManager), "Update")]
        private static void HandleMainMenuMovement(InitMenuManager __instance)
        {
            timeSinceMoved += Time.deltaTime;
            if (timeSinceMoved < NEXT_SELECTION_DELAY)
                return;
            //Debug.LogWarning(1);
            if (currentOptionsButton != null) {
                if (SteamVR_Actions._default.LeftJoystick.axis.y > 0.8)
                {
                    currentOptionsButton.m_baseButton.OnExitClick(1);
                    currentOptionsIndex = (currentOptionsIndex - 1 < 0) ? selectedOptionsPanel.m_options.Count - 1 : currentOptionsIndex - 1;
                    currentOptionsButton = selectedOptionsPanel.m_options[currentOptionsIndex];
                    currentOptionsButton.m_baseButton.OnEnterClick(1);
                    timeSinceMoved = 0;
                }
                else if (SteamVR_Actions._default.LeftJoystick.axis.y < -0.8)
                {
                    currentOptionsButton.m_baseButton.OnExitClick(1);
                    currentOptionsIndex += 1;
                    currentOptionsIndex %= selectedOptionsPanel.m_options.Count;
                    currentOptionsButton = selectedOptionsPanel.m_options[currentOptionsIndex];
                    currentOptionsButton.m_baseButton.OnEnterClick(1);
                    timeSinceMoved = 0;
                }
                else if (SteamVR_Actions._default.LeftJoystick.axis.x > 0.8) {
                    currentOptionsButton.OnButtonPressed(1);
                    timeSinceMoved = 0;
                }
                else if (SteamVR_Actions._default.LeftJoystick.axis.x < -0.8)
                {
                    currentOptionsButton.OnButtonPressed(3);
                    timeSinceMoved = 0;
                }
            }
            else if (currentButton != null)
            {
                if (SteamVR_Actions._default.LeftJoystick.axis.y > 0.7)
                {
                    currentButtonBehaviour.SelectButton(false);
                    currentButton = (NavItemButton)currentButton.navigationReferences.up;
                    currentButtonBehaviour = currentButton.GetTransform().GetComponent<ItemOptionMenuButtonBehaviour>();
                    currentButtonBehaviour.SelectButton(true);
                    timeSinceMoved = 0;
                }
                else if (SteamVR_Actions._default.LeftJoystick.axis.y < -0.7)
                {
                    currentButtonBehaviour.SelectButton(false);
                    currentButton = (NavItemButton)currentButton.navigationReferences.down;
                    currentButtonBehaviour = currentButton.GetTransform().GetComponent<ItemOptionMenuButtonBehaviour>();
                    currentButtonBehaviour.SelectButton(true);
                    timeSinceMoved = 0;

                }
                if (SteamVR_Actions._default.ButtonA.stateDown)
                {
                    currentButtonBehaviour.FakeOnClick();
                    timeSinceMoved = 0;
                }
            }
            else
            {
                InitMenuController menuController = (InitMenuController)__instance.menuRefs[0].menuController;
                currentButton = (NavItemButton)menuController.navItems[0];
                currentButtonBehaviour = currentButton.GetTransform().GetComponent<ItemOptionMenuButtonBehaviour>();
                currentButtonBehaviour.SelectButton(true);

            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GM_StateOptionsMenu), "Update")]
        private static void HandleMainMwenuMovement(GM_StateOptionsMenu __instance)
        {
            if (__instance.m_panelCurrent.name != "OptionsMenu") 
                return;
            timeSinceMoved += Time.deltaTime;
            if (timeSinceMoved < NEXT_SELECTION_DELAY)
                return;
            //Debug.LogWarning(1);
            if (currentOptionsButton != null)
            {
                if (SteamVR_Actions._default.LeftJoystick.axis.y > 0.8)
                {
                    currentOptionsButton.m_baseButton.OnExitClick(1);
                    currentOptionsIndex = (currentOptionsIndex - 1 < 0) ? selectedOptionsPanel.m_options.Count - 1 : currentOptionsIndex - 1;
                    currentOptionsButton = selectedOptionsPanel.m_options[currentOptionsIndex];
                    currentOptionsButton.m_baseButton.OnEnterClick(1);
                    timeSinceMoved = 0;
                }
                else if (SteamVR_Actions._default.LeftJoystick.axis.y < -0.8)
                {
                    currentOptionsButton.m_baseButton.OnExitClick(1);
                    currentOptionsIndex += 1;
                    currentOptionsIndex %= selectedOptionsPanel.m_options.Count;
                    currentOptionsButton = selectedOptionsPanel.m_options[currentOptionsIndex];
                    currentOptionsButton.m_baseButton.OnEnterClick(1);
                    timeSinceMoved = 0;
                }
                else if (SteamVR_Actions._default.LeftJoystick.axis.x > 0.8)
                {
                    currentOptionsButton.OnButtonPressed(1);
                    timeSinceMoved = 0;
                }
                else if (SteamVR_Actions._default.LeftJoystick.axis.x < -0.8)
                {
                    currentOptionsButton.OnButtonPressed(3);
                    timeSinceMoved = 0;
                }
            }
            else if (currentButton != null)
            {
                if (SteamVR_Actions._default.LeftJoystick.axis.y > 0.7)
                {
                    currentButtonBehaviour.SelectButton(false);
                    currentButton = (NavItemButton)currentButton.navigationReferences.up;
                    currentButtonBehaviour = currentButton.GetTransform().GetComponent<ItemOptionMenuButtonBehaviour>();
                    currentButtonBehaviour.SelectButton(true);
                    timeSinceMoved = 0;
                }
                else if (SteamVR_Actions._default.LeftJoystick.axis.y < -0.7)
                {
                    currentButtonBehaviour.SelectButton(false);
                    currentButton = (NavItemButton)currentButton.navigationReferences.down;
                    currentButtonBehaviour = currentButton.GetTransform().GetComponent<ItemOptionMenuButtonBehaviour>();
                    currentButtonBehaviour.SelectButton(true);
                    timeSinceMoved = 0;

                }
                if (SteamVR_Actions._default.ButtonA.stateDown)
                {
                    currentButtonBehaviour.FakeOnClick();
                    timeSinceMoved = 0;
                }
            }
            else
            {
                OptionsMenuController menuController = __instance.optionsView;
                currentButton = (NavItemButton)menuController.NavMenuItems[0];
                currentButtonBehaviour = currentButton.GetTransform().GetComponent<ItemOptionMenuButtonBehaviour>();
                currentButtonBehaviour.SelectButton(true);

            }
            if (SteamVR_Actions._default.ButtonB.stateDown) {
                __instance.optionsView.OnPressedBackOnPanel(null);
                timeSinceMoved = 0; 
            }

        }







            // Sets up the main menu screen to select the first available button
        [HarmonyPostfix]
        [HarmonyPatch(typeof(OptionsMenuController), "OnEnter")]
        private static void InitOptionsMenuSelection(OptionsMenuController __instance)
        {
            currentButtonBehaviour.SelectButton(false);
            currentButton = (NavItemButton)__instance.NavMenuItems[0];
            currentButtonBehaviour = currentButton.GetTransform().GetComponent<ItemOptionMenuButtonBehaviour>();
            currentButtonBehaviour.SelectButton(true);
        }


        // Sets up the main menu screen to select the first available button
        [HarmonyPrefix]
        [HarmonyPatch(typeof(OptionsMenuController), "OnPressedBackOnPanel")]
        private static bool www(OptionsMenuController __instance, OptionMenuPanelData panel)
        {
            if (timeSinceMoved < NEXT_SELECTION_DELAY)
                return false;
            if (currentOptionsButton == null)
                __instance.BackButtonPressed();
            else {
                selectedOptionsPanel = null;
                currentOptionsButton = null;
                __instance.currMenu.panelController.CloseWithAnimation(null);
                timeSinceMoved = 0;
            }
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(OptionsMenuController), "BackButtonPressed")]
        private static void wdww(OptionsMenuController __instance)
        {
            currentButtonBehaviour.SelectButton(false);
            selectedOptionsPanel = null;
            currentOptionsButton = null;
            currentButton = null;
            timeSinceMoved = 0;
        }




        [HarmonyPostfix]
        [HarmonyPatch(typeof(OptionsMenuController), "OptionPressed")]
        private static void InitChosenOptionsMenu(OptionsMenuController __instance, OptionMenuPanelData menuPressed)
        {
            if (!returningFromOptionsMenu)
            {
                selectedOptionsPanel = menuPressed.panelController;
                currentOptionsButton = selectedOptionsPanel.m_options[0];
                currentOptionsIndex = 0;
                currentOptionsButton.m_baseButton.OnEnterClick(1);
            }
            else
                returningFromOptionsMenu = false;
            
        }





        [HarmonyPostfix]
        [HarmonyPatch(typeof(SaveScreenView), "OpenNormal")]
        private static void InitChosenOwwptiddonsMenu(SaveScreenView __instance)
        {
            currentButtonBehaviour.SelectButton(false) ;
            currentButton = (NavItemButton)__instance.mainMenuNavItems[0];
            currentButtonBehaviour = currentButton.GetTransform().GetComponent<ItemOptionMenuButtonBehaviour>();
            currentButtonBehaviour.SelectButton(true);

        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SaveScreenView), "CloseNormal")]
        private static void d(SaveScreenView __instance)
        {
            currentButtonBehaviour.SelectButton(false);
            currentButton = null;
            timeSinceMoved = 0;

        }

    }
}
