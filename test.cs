using HarmonyLib;
using Rewired;
using System.Reflection;
using Unity.XR.OpenVR;
using UnityEngine;
using UnityEngine.XR.Management;
using Valve.VR;

namespace TormentedSoulsVR
{
    internal class test : MonoBehaviour
    {
        private void Awake()
        {
            PlayerPrefs.SetInt("XBOX_EN", 1);

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

        }

        private void Update() {
            Controllers.Update();
            if (CamFix.inCinematic && CamFix.camRoot != null)
            {
                CamFix.camRoot.transform.position = Camera.main.transform.position + CamFix.vrCamera.transform.localPosition * -1;
                //CamFix.camHolder.transform.localPosition = CamFix.vrCamera.transform.localPosition * -1;
                CamFix.camRoot.transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
            }
            else if (PlayerController.instance != null) {
                Vector3 newPos = CamFix.vrCamera.transform.localPosition * -1;
                newPos.y += 1.575f;
                newPos.z += 0.05f;
                CamFix.camHolder.transform.localPosition = newPos;
                CamFix.camRoot.transform.position = PlayerController.instance.transform.position;
                CamFix.camRoot.transform.rotation = PlayerController.instance.transform.rotation;
            }
        }
    }
}
