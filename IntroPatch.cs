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
            CamFix.camHolder.transform.localPosition = CamFix.vrCamera.transform.position * -1;
            CamFix.menus.transform.localPosition = new Vector3(0,0,0.3f);

        }



        [HarmonyPrefix]
        [HarmonyPatch(typeof(UnityEngine.Playables.PlayableDirector), "Play", new Type[] { })]
        private static void PositionIntroCinematic(UnityEngine.Playables.PlayableDirector __instance)
        {
            if (SceneManager.GetActiveScene().name == "IntroScene") { 
                Canvas cinematicCanvas = __instance.transform.parent.GetChild(1).GetComponent<Canvas>();
                if (cinematicCanvas != null)
                {
                    cinematicCanvas.renderMode = RenderMode.WorldSpace;
                    cinematicCanvas.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
                    cinematicCanvas.transform.position = new Vector3(0f, 0f, 2.1f);
                }
            }
            else { 
                CamFix.inCinematic = true;
                CamFix.camHolder.transform.localPosition = CamFix.vrCamera.transform.localPosition * -1;
            }

        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(UnityEngine.Playables.PlayableDirector), "SendOnPlayableDirectorStop", new Type[] { })]
        private static void ResetVarOnCinematicEnd(UnityEngine.Playables.PlayableDirector __instance)
        {
            CamFix.inCinematic = false;
        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(ItemObsClick), "Setup")]
        private static void DisableColliderMesh(ItemObsClick __instance)
        {
            if (__instance.name == "DireccionCollider" || __instance.name == "AbrirCollider" || __instance.name == "TextoCollider" || __instance.name == "FotoCollider")
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
                    CamFix.inIntro = true;
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
                CamFix.inIntro = false;
                CamFix.headsetPos = CamFix.vrCamera.transform.localPosition;
            }
        }
    }
}
