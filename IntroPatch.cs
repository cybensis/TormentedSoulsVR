using HarmonyLib;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TormentedSoulsVR
{
    [HarmonyPatch]
    internal class IntroPatch
    {



        [HarmonyPostfix]
        [HarmonyPatch(typeof(ActorEvent_PuzzleIllumination), "Start")]
        private static void PositionIntroCanvasAndCam(ActorEvent_PuzzleIllumination __instance)
        {
            if (__instance.gameObject.name != "IntroScene_Actor_GameObject")
                return;
            GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (GameObject gameObj in rootObjects)
            {
                if (gameObj.name == "Canvas")
                {
                    Canvas canvas = gameObj.GetComponent<Canvas>();
                    canvas.renderMode = RenderMode.WorldSpace;
                    gameObj.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                    gameObj.transform.position = new Vector3(0, 0, 10);
                    break;
                }
            }

            CamFix.vrCamera.backgroundColor = new Color(0.1284f, 0.1792f, 0.1333f, 1);
            CamFix.camHolder.transform.localPosition = new Vector3(0, 0, 0.1f);

        }



        [HarmonyPrefix]
        [HarmonyPatch(typeof(UnityEngine.Playables.PlayableDirector), "Play", new Type[] { })]
        private static void PositionIntroCinematic(UnityEngine.Playables.PlayableDirector __instance)
        {
            Debug.LogWarning("bbbbbbbbbbbbbb");
            Canvas cinematicCanvas = __instance.transform.parent.GetChild(1).GetComponent<Canvas>();
            if (cinematicCanvas != null)
            {
                cinematicCanvas.renderMode = RenderMode.WorldSpace;
                cinematicCanvas.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
                cinematicCanvas.transform.position = new Vector3(0f, 1.5f, 2.1f);
            }
            else { 
                CamFix.inCinematic = true;
                CamFix.camHolder.transform.localPosition = CamFix.vrCamera.transform.localPosition * -1;
            }

        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(UnityEngine.Playables.PlayableDirector), "SendOnPlayableDirectorStop", new Type[] { })]
        private static void PositionIntroCfdinematic(UnityEngine.Playables.PlayableDirector __instance)
        {

            Debug.LogWarning("eeeeeeeeeeeeeeee");
            CamFix.inCinematic = false;

        }
        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(UnityEngine.Playables.PlayableDirector), "ClearReferenceValue")]
        //private static void PositionIntroffCfdinematic(UnityEngine.Playables.PlayableDirector __instance)
        //{

        //    Debug.LogWarning("1122222222222222");

        //}

        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(UnityEngine.Playables.PlayableDirector), "Pause")]
        //private static void PositionIntroffCfdifnematic(UnityEngine.Playables.PlayableDirector __instance)
        //{

        //    Debug.LogWarning("3333333333333333333");

        //}


        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(SceneFlowDirector), "UnregisterActor")]
        //private static void PositionIntroffCfdifnewwmatic(SceneFlowDirector __instance)
        //{

        //    Debug.LogWarning("4444444444444");

        //}



        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(HintableBehaviour), "OnDisable")]
        //private static void PositionwIntroCfdinematic(HintableBehaviour __instance)
        //{

        //    Debug.LogWarning("111111111");

        //}


        [HarmonyPostfix]
        [HarmonyPatch(typeof(ItemObsClick), "Setup")]
        private static void DisableColliderMesh(ItemObsClick __instance)
        {
            if (__instance.name == "DireccionCollider" || __instance.name == "AbrirCollider")
            {
                UnityEngine.MeshFilter filter = __instance.GetComponent<MeshFilter>();
                // The colliders for the intro letter are visible for some reason so hide them with this
                filter.mesh.subMeshCount = 0;
                if (CamFix.camRoot.transform.childCount >= 2)
                {
                    Transform gui3dObject = CamFix.camRoot.transform.GetChild(1).GetChild(2);
                    // Re-enable the 3D object inspector lights and reflection probe just for this scene
                    gui3dObject.GetChild(2).gameObject.SetActive(true);
                    gui3dObject.GetChild(2).GetComponent<UnityEngine.Light>().intensity = 1.25f;
                    gui3dObject.GetChild(5).gameObject.SetActive(true);
                }
            }
        }



        [HarmonyPrefix]
        [HarmonyPatch(typeof(ItemObservationBase), "OnLetterCommentaryClick")]
        private static void SetCameraBackgroundToBlackAgain(ItemObservationBase __instance, string id)
        {
            if (id == "maintext") { 
                CamFix.vrCamera.backgroundColor = Color.black;
                Transform gui3dObject = CamFix.camRoot.transform.GetChild(1).GetChild(2);
                gui3dObject.GetChild(2).gameObject.SetActive(false);
                gui3dObject.GetChild(5).gameObject.SetActive(false);
            }
        }
    }
}
