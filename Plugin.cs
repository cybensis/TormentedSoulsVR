using BepInEx;
using HarmonyLib;
using System.Reflection;
using Unity.XR.OpenVR;
using UnityEngine;
using UnityEngine.XR.Management;
using Valve.VR;

namespace TormentedSoulsVR;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    private void Awake()
    {
        // Plugin startup logic
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        //PlayerPrefs.SetInt("XBOX_EN", 1);

        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

        //SteamVR_Actions.PreInitialize();

        //SteamVR_Settings.instance.pauseGameWhenDashboardVisible = true;

        //var generalSettings = ScriptableObject.CreateInstance<XRGeneralSettings>();
        //var managerSettings = ScriptableObject.CreateInstance<XRManagerSettings>();
        //var xrLoader = ScriptableObject.CreateInstance<OpenVRLoader>();


        //var settings = OpenVRSettings.GetSettings();
        //settings.StereoRenderingMode = OpenVRSettings.StereoRenderingModes.MultiPass;
        //generalSettings.Manager = managerSettings;

        //managerSettings.loaders.Clear();
        //managerSettings.loaders.Add(xrLoader);
        //managerSettings.InitializeLoaderSync(); ;

        //XRGeneralSettings.AttemptInitializeXRSDKOnLoad();
        //XRGeneralSettings.AttemptStartXRSDKOnBeforeSplashScreen();

        //SteamVR.Initialize();
    }
}
