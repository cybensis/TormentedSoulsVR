using TormentedSoulsVR.cam;
using UnityEngine;
using Valve.VR;
using Valve.VR.Extras;

namespace TormentedSoulsVR;

public static class CameraManager
{
    public static SteamVR_LaserPointer laserPointer;

    public static void Setup()
    {
        SpawnHands();
        if (RightHand)
            RightHand.transform.parent = CamFix.camHolder.transform;
        if (LeftHand)
            LeftHand.transform.parent = CamFix.camHolder.transform;
    }

    public static void SpawnHands()
    {
        if (!RightHand)
        {
            //RightHand = GameObject.Instantiate(AssetLoader.RightHandBase, Vector3.zero, Quaternion.identity);
            RightHand = new GameObject("RightHand");
            RightHand.AddComponent<SteamVR_Behaviour_Pose>();
            //RightHand.AddComponent<SteamVR_Skeleton_Poser>();
            RightHand.transform.parent = CamFix.camHolder.transform;

        }
        if (!LeftHand)
        {
            //LeftHand = GameObject.Instantiate(AssetLoader.LeftHandBase, Vector3.zero, Quaternion.identity);
            LeftHand = new GameObject("LeftHand");
            LeftHand.AddComponent<SteamVR_Behaviour_Pose>();
            LeftHand.transform.parent = CamFix.camHolder.transform;
        }
    }



    // VR Origin and body stuff
    public static GameObject LeftHand = null;
    public static GameObject RightHand = null;


}
