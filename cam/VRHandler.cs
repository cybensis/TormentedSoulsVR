﻿using HarmonyLib;
using JetBrains.Annotations;
using Rewired;
using System.Reflection;
using TormentedSoulsVR.UI;
using Unity.XR.OpenVR;
using UnityEngine;
using UnityEngine.XR.Management;
using Valve.VR;

namespace TormentedSoulsVR.cam
{
    internal class VRHandler : MonoBehaviour
    {
        public float heightOffset = 1.575f;
        private void Awake()
        {

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            SteamVR_Actions.PreInitialize();


            SteamVR_Settings.instance.pauseGameWhenDashboardVisible = true;

            var generalSettings = ScriptableObject.CreateInstance<XRGeneralSettings>();
            var managerSettings = ScriptableObject.CreateInstance<XRManagerSettings>();
            var xrLoader = ScriptableObject.CreateInstance<OpenVRLoader>();


            var settings = OpenVRSettings.GetSettings();
            settings.StereoRenderingMode = OpenVRSettings.StereoRenderingModes.MultiPass;
            generalSettings.Manager = managerSettings;

            managerSettings.loaders.Clear();
            managerSettings.loaders.Add(xrLoader);
            managerSettings.InitializeLoaderSync(); ;

            XRGeneralSettings.AttemptInitializeXRSDKOnLoad();
            XRGeneralSettings.AttemptStartXRSDKOnBeforeSplashScreen();

            SteamVR.Initialize();

            // When returning from the game to the main menu, it deletes the controller scheme so we have to reset it here
            Controllers.ResetControllerVars();
            Controllers.Init();


            SteamVR_Actions._default.RightHandPose.AddOnUpdateListener(SteamVR_Input_Sources.Any, UpdateRightHand);
            SteamVR_Actions._default.LeftHandPose.AddOnUpdateListener(SteamVR_Input_Sources.Any, UpdateLeftHand);
            SteamVR_Actions._default.Start.AddOnStateDownListener(SetHeight, SteamVR_Input_Sources.Any);
        }

        private void Update()
        {
            Controllers.Update();
            if (CamFix.inCinematic)
            {
                CamFix.camRoot.transform.position = Camera.main.transform.position;
                CamFix.camHolder.transform.localPosition = CamFix.headsetPos * -1;
                CamFix.camRoot.transform.rotation = Quaternion.Euler(0, Camera.main.transform.localEulerAngles.y, 0);
                //CamFix.menus.transform.localPosition = Quaternion.identity;

            }
            // Do this so camera is black during loading screens
            else if (CamFix.player != null && !CamFix.player.m_playerManager.m_cameraManager.m_cameraInitialized)
            {
                CamFix.camRoot.transform.position = new Vector3(100, 100, 100);
            }
            else if (CamFix.player != null)
            {
                Vector3 newPos = CamFix.headsetPos * -1;
                //newPos.y += CamFix.player.m_capsuleCollider.height;
                newPos.y += heightOffset;
                newPos.z += 0.05f;
                CamFix.camHolder.transform.localPosition = newPos;
                CamFix.camRoot.transform.position = CamFix.player.transform.position;
                //CamFix.camRoot.transform.rotation = CamFix.player.transform.rotation;
                CamFix.menus.transform.localRotation = Quaternion.Euler(0, HUDPatches.targetHUDYRot, 0);
            }
        }

        public void SetMenuPosOnSave()
        {
            CamFix.menus.transform.localRotation = Quaternion.Euler(40, 0, 0);
            CamFix.menus.transform.localPosition = new Vector3(0, -0.2f, 0.2f);
        }

        public void SetMenuPosOnExitSave()
        {
            CamFix.menus.transform.localRotation = Quaternion.Euler(0, 0, 0);
            CamFix.menus.transform.localPosition = HUDPatches.HUD_POSITION;
        }


        private static void UpdateRightHand(SteamVR_Action_Pose fromAction, SteamVR_Input_Sources fromSource)
        {
            if (CameraManager.RightHand)
            {
                CameraManager.RightHand.transform.localPosition = fromAction.localPosition;
                CameraManager.RightHand.transform.localRotation = fromAction.localRotation;

            }

        }

        private static void UpdateLeftHand(SteamVR_Action_Pose fromAction, SteamVR_Input_Sources fromSource)
        {
            if (CameraManager.LeftHand)
            {
                CameraManager.LeftHand.transform.localPosition = fromAction.localPosition;
                CameraManager.LeftHand.transform.localRotation = fromAction.localRotation;
            }
        }


        public static void SetHeight(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            CamFix.headsetPos = CamFix.vrCamera.transform.localPosition;
            if (!CamFix.inCinematic && CamFix.menus != null)
                CamFix.menus.transform.position = CamFix.vrCamera.transform.position + (CamFix.vrCamera.transform.forward * 0.35f);
            //CamFix.camHolder.transform.localRotation = Quaternion.Euler(0,CamFix.vrCamera.transform.localEulerAngles.y * -1,0);
            CamFix.crouchInstance.InitHeight();

        }
    }
}
