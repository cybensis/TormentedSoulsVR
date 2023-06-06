using HarmonyLib;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TormentedSoulsVR.cam
{
    [HarmonyPatch]
    internal class InteractablePatches
    {

        // Need to change the menu position and rotation for a few interactables
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerDetector), "ForcePlayerExitFromCollider")]
        private static void PlaceCamAndUIOnHintInteract(PlayerDetector __instance)
        {
            CamFix.inCinematic = true;
            CamFix.headsetPos = CamFix.vrCamera.transform.localPosition;
            CamFix.menus.transform.localPosition = new Vector3(0, 0, 0.3f);
            try {
                if (__instance.transform.parent.parent.name == "CashRegisterTrigger_GameObject")
                {
                    CamFix.menus.transform.localPosition = new Vector3(0, -0.16f, 0.4f);
                    CamFix.menus.transform.localRotation = Quaternion.Euler(60, 0, 0);
                }
                else if (__instance.transform.parent.parent.name == "Room2B_Past_ContainerHintable")
                {
                    CamFix.menus.transform.localPosition = new Vector3(0, -0.2f, 0.3f);
                    CamFix.menus.transform.localRotation = Quaternion.Euler(60, 0, 0);
                }
                else if (__instance.transform.parent.parent.name == "MonkeysPuzzleHintable_GameObject")
                {
                    CamFix.menus.transform.localPosition = new Vector3(0.05f, -0.3f, 0.35f);
                    CamFix.menus.transform.localRotation = Quaternion.Euler(60, 0, 0);
                }
                else if (__instance.transform.parent.parent.name == "ComputerPuzzleHintable_GameObject")
                {
                    CamFix.menus.transform.localPosition = new Vector3(-0.0779f, -0.25f, 0.3065f);
                    CamFix.menus.transform.localRotation = Quaternion.Euler(75, 0, 0);
                }
                else if (__instance.transform.parent.parent.name == "MusicPanelHint")
                {
                    CamFix.menus.transform.localPosition = new Vector3(0f, -0.3f, 0.3f);
                    CamFix.menus.transform.localRotation = Quaternion.Euler(85, 0, 0);
                }
                else if (__instance.transform.parent.parent.parent.parent.name == "TapeRecorder")
                    CamFix.vrHandlerInstance.SetMenuPosOnSave();
            }
            catch (Exception e) { 
                CamFix.menus.transform.localRotation = Quaternion.identity;
            }
            
        }



        // Need to manually set the virtual camera positions for a lot of interactable stuff
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ActorEvent_VirtualCamera), "Start")]
        private static void SetVirtualCamPosition(ActorEvent_VirtualCamera __instance)
        {
            string camName = __instance.name;
            if (camName == "ShelfCamera")                                                                                           // Shelf at the start of the game
            { 
                __instance.virtualCamera.transform.position = new Vector3(1.918f, 1.6789f, -21.771f);
                __instance.virtualCamera.transform.parent.GetChild(1).position = new Vector3(2.512f, -0.9286f, -21.9002f);
            }
            else if (camName == "PadlockCameraActor")                                                                               // Padlock on the shelf at the start of the game
                __instance.virtualCamera.transform.position = new Vector3(2.1541f, 1.6139f, -22.03f);
            else if (camName == "Corridor_1C_DLCPuzzle__VirtualCamera0")                                                            // DLC puzzle
            {
                __instance.virtualCamera.transform.position = new Vector3(-4.215f, 2.77f, -19.071f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(287, 357, 244);
            }
            else if (camName == "DoorCameraActor")                                                                                  // Door in the starting room you need to use the wrench on
                __instance.virtualCamera.transform.position = new Vector3(-0.753f, 0.9859f, -20.391f);
            else if (camName == "Xray_A_VirtualCamera0_GameObject")                                                                 // The X-ray room save
            {
                __instance.virtualCamera.transform.position = new Vector3(-16.006f, 1.084f, 2.493f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0f, 194f, 0f);
            }
            else if (camName == "MotorPuzzleCamera_GameObject")                                                                     // The motor thing in engine room
            {
                __instance.virtualCamera.transform.position = new Vector3(-5.0709f, 2.1617f, 0.4292f);
                __instance.virtualCamera.transform.rotation = Quaternion.Euler(0, 8, 0);
            }
            else if (camName == "ValvePuzzleNewCamera_GameObject")                                                                  // The engine room gas box door
            {
                __instance.virtualCamera.transform.position = new Vector3(-1.3899f, 1.546f, 1.6511f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0, 273, 0);
            }
            else if (camName == "ValvePuzzleCloserCamera_GameObject")                                                               // The engine room gas box inside
            {
                __instance.virtualCamera.transform.position = new Vector3(-1.1139f, 1.5912f, 2.1472f);
                __instance.virtualCamera.transform.rotation = Quaternion.Euler(0, 90, 0);
            }
            else if (camName == "MirrorCamera_GameObject" && SceneManager.GetActiveScene().name == "Bathroom_E")                    // The maternity ward underground mirror
            {
                __instance.virtualCamera.transform.position = new Vector3(-1.026f, 1.3075f, -0.2503f);
                __instance.virtualCamera.transform.parent.GetChild(1).position = new Vector3(-1.0235f, 1.234f, -0.081f);
            }
            else if (camName == "MirrorCamera_GameObject")                                                                          // The maternity ward underground mirror
                __instance.virtualCamera.transform.position = new Vector3(3.1799f, -0.448f, -3.2055f);
            else if (camName == "BabyDoll_Camera")                                                                                  // The mirror world baby
                __instance.virtualCamera.transform.position = new Vector3(1.8144f, 1.47f, -6.0777f);
            else if (camName == "HandCamera")                                                                                       // The woman in the maternity ward
            {
                __instance.virtualCamera.transform.position = new Vector3(1.5594f, 1.3408f, -6.218f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0, 243, 0);
            }
            else if (camName == "DoorPuzzleCamera")                                                                                 // Waiting room door
            {
                __instance.virtualCamera.transform.position = new Vector3(-3.0185f, 1.1519f, 0.4228f);
                __instance.virtualCamera.transform.parent.GetChild(1).position = new Vector3(-3.0752f, 1.0959f, 0.2848f);
            }
            else if (camName == "FridgeCamera")                                                                                     // Fridge in the kitchen
            {
                __instance.virtualCamera.transform.position = new Vector3(-0.4357f, 1.7708f, -2.708f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0f, 192f, 0f);
            }
            else if (camName == "HeartBeatPuzzleCamera_GameObject") // Fridge in the kitchen
                __instance.virtualCamera.transform.position = new Vector3(-2.488f, 4.612f, 20.489f);
            else if (camName == "Corridor2dHeartBeatPuzzleCamera_GameObject")                                                       // The door with the knocking puzzle
            {
                __instance.virtualCamera.transform.position = new Vector3(-6.031f, 1.313f, 5.683f);
                __instance.virtualCamera.transform.parent.GetChild(1).position = new Vector3(1.502f, 1.186f, 6.1762f);
            }
            else if (camName == "HydraulicElevatorPuzzleCamera_GameObject")                                                         // Library hydraulic lift puzzle
            {
                __instance.virtualCamera.transform.position = new Vector3(-14.896f, 1.673f, -3.2214f);
                __instance.virtualCamera.transform.parent.GetChild(1).position = new Vector3(-14.509f, 1.558f, -3.2696f);
            }
            else if (camName == "FuseboxReception_Camera_GameObject" && SceneManager.GetActiveScene().name == "Corridor_2A")        // Reception area fuse box
            {
                __instance.virtualCamera.transform.position = new Vector3(-6.282f, 1.7057f, -7.1045f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0f, 289f, 0f);
            }
            else if (camName == "FuseboxReception_Camera_GameObject")                                                               // Reception area fuse box
            {
                __instance.virtualCamera.transform.position = new Vector3(2.873f, 2.121f, 1.834f);
                __instance.virtualCamera.transform.parent.GetChild(1).position = new Vector3(2.578f, 1.978f, 1.28f);
            }
            else if (camName == "Corridor1C_VirtualCamera0_GameObject")                                                             // Alien/human door puzzle
                __instance.virtualCamera.transform.position = new Vector3(-1.06f, 0.939f, -11.307f);
            else if (camName == "Archives_VirtualCamera0")                                                                          // Tetris door puzzle
            {
                __instance.virtualCamera.transform.position = new Vector3(-13.3091f, 0.865f, 5.8446f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(354f, 257f, 268f);
            }
            else if (camName == "Corridor_1A_TrapdoorCamera_GameObject")                                                            // Trapdoor with rope on it
                __instance.virtualCamera.transform.position = new Vector3(2.6804f, 0.24f, 8.1234f);
            else if (camName == "PresentAcidPuzzleCamera_GameObject" || camName == "AcidPuzzleCamera_GameObject")                   // Lock acid puzzle
            {
                __instance.virtualCamera.transform.position = new Vector3(-2.778f, 0.947f, -12.967f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0f, 212f, 0f);
            }
            else if (camName == "MirrorRoom_BP_VirtualCamera1_GameObject")                                                          // Statue with buttons tape world
            {
                __instance.virtualCamera.transform.position = new Vector3(-10.952f, 0.975f, 4.4793f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0f, 282f, 0f);
            }
            else if (camName == "MirrorRoom_B_VirtualCamera1_GameObject")                                                          // Statue with buttons
            {
                __instance.virtualCamera.transform.position = new Vector3(-11.4067f, 0.972f, 4.5898f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0f, 277f, 0f);
            }
            else if (camName == "VCRPuzzleCamera_GameObject")                                                                       // VCR
            {
                __instance.virtualCamera.transform.position = new Vector3(-10.1856f, 1.268f, -0.2953f);
                __instance.virtualCamera.transform.parent.GetChild(1).position = new Vector3(-12.0406f, 0.998f, -0.4713f);
            }
            else if (camName == "CashRegisterVirtualCamera_GameObject")                                                             // Cash register puzzle
            {
                __instance.virtualCamera.transform.position = new Vector3(-5.3728f, 1.7362f, 3.4011f);
                __instance.virtualCamera.transform.parent.GetChild(1).position = new Vector3(-5.8083f, 1.5742f, 2.19f);
            }
            else if (camName == "Corridor_2A_VirtualCamera0_GameObject")                                                            // Number door puzzle
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(20f, 269f, 270f);
            else if (camName == "Room2B_Past_ContainerCamera")                                                                      // Mirror world chest
                __instance.virtualCamera.transform.position = new Vector3(2.1819f, 1.041f, 3.412f);
            else if (camName == "Room2DM_GuillotineCamera")                                                                         // Monkey guillotine
            {
                __instance.virtualCamera.transform.position = new Vector3(-0.529f, 0.86f, 0.01f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0f, 194f, 0f);
            }
            else if (camName == "ManiquiCamera_GameObject")                                                                         // manequin 
            {
                __instance.virtualCamera.transform.position = new Vector3(-0.899f, 1.128f, -2.99f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0f, 251f, 0f);
            }
            else if (camName == "PuzzleMonkeyCamera_GameObject")                                                                    // Monkey puzzle 
            {
                __instance.virtualCamera.transform.position = new Vector3(-0.31f, 1.596f, 1.783f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0f, 269f, 0f);
            }
            else if (camName == "ItemGateCamera_GameObject")                                                                        // Monkey puzzle stapler
                __instance.virtualCamera.transform.parent.GetChild(1).position = new Vector3(-1.3038f, 0.941f, 1.47f);
            else if (camName == "BatteryPuzzleCamera_GameObject")                                                                   // Battery puzzle
                __instance.virtualCamera.transform.position = new Vector3(-1.264f, 1.403f, -1.311f);
            else if (camName == "SpendingMachineCamera_GameObject")                                                                 // Vending machine puzzle
            {
                __instance.virtualCamera.transform.position = new Vector3(-14.735f, 0.841f, -4.26f);
                __instance.virtualCamera.transform.parent.GetChild(1).position = new Vector3(-4.5007f, 0.867f, -4.4545f);
            }
            else if (camName == "Laundry_VirtualCamera0_GameObject")                                                                // Laundry puzzle
            {
                __instance.virtualCamera.transform.position = new Vector3(-9.334f, 0.391f, 0.835f);
                __instance.virtualCamera.transform.parent.GetChild(1).position = new Vector3(-8.351f, 0.272f, 1.0719f);
            }
            else if (camName == "ComputerCamera_GameObject")                                                                        // Computer puzzle
            {
                __instance.virtualCamera.transform.position = new Vector3(-5.778f, -0.5393f, 2.999f);
                __instance.virtualCamera.transform.parent.GetChild(1).position = new Vector3(-5.9819f, -1.3474f, 22.3859f);
            }
            else if (camName == "FloopyDriveCamera_GameObject")                                                                     // Floppy disk door lock
            {
                __instance.virtualCamera.transform.position = new Vector3(-12.032f, 1.177f, -0.9275f);
                __instance.virtualCamera.transform.parent.GetChild(1).position = new Vector3(-12.2442f, 1.13f, 1.1859f);
            }
            else if (camName == "PhonographVirtualCamera_GameObject")                                                              // Phonograph puzzle
            {
                __instance.virtualCamera.transform.position = new Vector3(-2.3891f, 1.1993f, -5.7073f);
                __instance.virtualCamera.transform.parent.GetChild(1).position = new Vector3(-1.9388f, 1.0263f, -6.0423f);
            }
            else if (camName == "RealRustyHatch_Camera_GameObject")                                                                // Mirror world rusty hatch
                __instance.virtualCamera.transform.position = new Vector3(2.1491f, 1.358f, -12.1265f);
            else if (camName == "BustPuzzleVirtualCamera_GameObject")                                                              // Bust eye puzzle
                __instance.virtualCamera.transform.position = new Vector3(-0.2085f, 1.3743f, 0.0758f);
            else if (camName == "Corridor1D_ClockVirtualCamera_GameObject")                                                        // Clock puzzle
            {
                __instance.virtualCamera.transform.position = new Vector3(-6.0596f, 1.586f, -0.4758f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0, 283, 0);
            }
            else if (camName == "Panel Camera" && SceneManager.GetActiveScene().name == "Office")                                   // Music puzzle
            {
                __instance.virtualCamera.transform.position = new Vector3(-2.484f, 1.6373f, 1.968f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0, 266, 0);
            }
            else if (camName == "SecurityPanel_VirtualCamera_GameObject")                                                            // Downstairs security panel
            {
                __instance.virtualCamera.transform.position = new Vector3(-2.209f, -3.777f, 23.113f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0, 183, 0);
            }
            else if (camName == "Mausoleo_Camera_GameObject")                                                                       // Statue hand puzzle
            {
                __instance.virtualCamera.transform.position = new Vector3(-0.0026f, 0.6112f, -13.6073f);
                __instance.virtualCamera.transform.parent.GetChild(1).position = new Vector3(-0.1834f, 1.0076f, -14.2398f);
            }
            else if (camName == "SewerRoom_MirrorCamera")                                                                           // Sewer bedroom mirror puzzle
            {
                __instance.virtualCamera.transform.position = new Vector3(-3.8403f, 1.992f, 0.7181f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0, 341, 0);
            }
            else if (camName == "TvPuzzleSafeCamera_GameObject")                                                                    // Sewer bedroom safe puzzle
                __instance.virtualCamera.transform.position = new Vector3(-2.6361f, 1.87f, -1.6686f);
            else if (camName == "TvPuzzleMirrorCamera_GameObject")                                                                    // Sewer bedroom tv puzzle
                __instance.virtualCamera.transform.position = new Vector3(-1.842f, 1.645f, -2.076f);
            else if (camName == "ClosetPuzzle_VirtualCamera")                                                                       // Childrens room puzzle
            {
                __instance.virtualCamera.transform.position = new Vector3(-7.665f, 1.5766f, 1.0269f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0f, 89f, 0f);
            }
            else if (camName == "BathroomAPast_Camera_GameObject")                                                                  // Bathtub puzzle
            {
                __instance.virtualCamera.transform.position = new Vector3(-2.659f, 1.331f, -21.291f);
                __instance.virtualCamera.transform.parent.GetChild(1).position = new Vector3(-3.303f, 1.09f, -20.971f);
            }
            else if (camName == "Sewer_CameraLeft_GameObject")                                                                       // Sewer left eye thing
            {
                __instance.virtualCamera.transform.position = new Vector3(5.315f, -1.964f, -9.948f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else if (camName == "Sewer_CameraRight_GameObject")                                                                       // Sewer right eye thing
            {
                __instance.virtualCamera.transform.position = new Vector3(-0.986f, -1.998f, -10.071f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0f, 7.6f, 0f);
            }
            else if (camName == "BunkerEntranceCamera_GameObject")                                                                  // Bunker entrance puzzle
            {
                __instance.virtualCamera.transform.position = new Vector3(0.645f, -2.035f, 1.483f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0f, 149f, 0f);
            }
            else if (camName == "PumpEngineCamera_GameObject")                                                                  // Bunker generator puzzle
                __instance.virtualCamera.transform.position = new Vector3(-2.431f, -9.885f, -0.835f);
            else if (camName == "Bunker2b_HandleCamera")                                                                            // Gas pump 
            {
                __instance.virtualCamera.transform.position = new Vector3(1.181f, 1.039f, -6.151f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0f, 89f, 0f);
            }
            else if (camName == "Bunker_6BFinalExposition_ZoomCamera ")                                                            // Stairs receiver
                __instance.virtualCamera.transform.position = new Vector3(3.536f, 0.899f, -8.235f);
            else if (camName == "PipesCamera")                                                                                      // Valve puzzles 
            {
                __instance.virtualCamera.transform.position = new Vector3(0.5708f, 1.488f, -5.816f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            }
            else if (camName == "Bunker_6BAntidode_VirtualCamera0")                                                                 // Anna 
            {
                __instance.virtualCamera.transform.position = new Vector3(5.654f, 2.657f, -11.252f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0f, 178f, 0f);
            }
            else if (SceneManager.GetActiveScene().name == "Bunker_6A")
            {
                if (camName == "CameraZoom1_GameObject")                                                                            // Boss fight panel 1 
                    __instance.virtualCamera.transform.position = new Vector3(6.9501f, -8.418f, 5.337f);
                else if (camName == "CameraZoom2_GameObject")                                                                       // Boss fight panel 2 
                    __instance.virtualCamera.transform.position = new Vector3(-16.503f, -3.3318f, 2.742f);
                else if (camName == "CameraZoom3_GameObject")                                                                       // Boss fight panel 3
                    __instance.virtualCamera.transform.position = new Vector3(-6.3892f, -4.624f, -10.36f);
            }
            else if (camName == "ExitDoorCamera")                                                                                   // Ending door
            {
                __instance.virtualCamera.transform.position = new Vector3(-2.209f, 2.37f, -3.427f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0f, 174f, 0f);
            }
            else if (camName == "BA_ElevatorVirtualCamera_GameObject" && SceneManager.GetActiveScene().name == "Corridor_1B")       // Elevator panel bottom floor
            {
                __instance.virtualCamera.transform.position = new Vector3(-9.2681f, 2.2473f, 2.465f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0f, 102f, 0f);
            }
            else if (camName == "BA_ElevatorVirtualCamera_GameObject" && SceneManager.GetActiveScene().name == "Corridor_2A")       // Elevator panel
            {
                __instance.virtualCamera.transform.position = new Vector3(-10.84f, 1.2232f, -6.9491f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0f, 102f, 0f);
            }
            else if (__instance.transform.parent.parent.name == "TapeRecorder" && SceneManager.GetActiveScene().name == "ExamRoom") // The exam room save
            {
                __instance.virtualCamera.transform.position = new Vector3(-6.4032f, 1.4941f, -0.5686f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0f, 268f, 0f);
            }
            else if (SceneManager.GetActiveScene().name == "SewingRoom")                                                            // The sewing room save
            {
                __instance.virtualCamera.transform.position = new Vector3(-2.6576f, 1.4861f, -1.5905f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0f, 354f, 0f);
            }
            else if (SceneManager.GetActiveScene().name == "Bathroom_C")                                                            // Office like save room
            {
                __instance.virtualCamera.transform.position = new Vector3(3.0959f, 1.6061f, -0.5924f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0f, 86f, 0f);
            }
            else if (SceneManager.GetActiveScene().name == "Room_2E")                                                               // Upstairs break room save
            {
                __instance.virtualCamera.transform.position = new Vector3(-3.3754f, 1.0501f, -0.405f);
                __instance.virtualCamera.transform.parent.GetChild(1).position = new Vector3(-3.8732f, 0.7211f, 2.8718f);
            }
            else if (SceneManager.GetActiveScene().name == "DeliveryRoom")                                                          // Delivery room save
            {
                __instance.transform.position = new Vector3(-7.649f, 0.9524f, 2.0619f);
                __instance.transform.localRotation = Quaternion.Euler(0, 37, 0);
            }
            else if (SceneManager.GetActiveScene().name == "Stair_Save")                                                            // Stair room save
            {
                __instance.virtualCamera.transform.position = new Vector3(-0.8574f, 1.5401f, -0.0159f);
                __instance.virtualCamera.transform.parent.GetChild(1).position = new Vector3(-0.8647f, 1.2111f, -0.4122f);
            }
            else if (SceneManager.GetActiveScene().name == "SewerRoom")                                                             // Bedroom in sewer
                __instance.virtualCamera.transform.position = new Vector3(-4.6679f, 1.9721f, 2.0822f);
            else if (SceneManager.GetActiveScene().name == "Bunker_3C")                                                             // Bunker save
                __instance.virtualCamera.transform.position = new Vector3(2.0482f, 1.5011f, 0.5886f);
            else if (SceneManager.GetActiveScene().name == "Bunker_6C")                                                             // Bunker last save
                __instance.virtualCamera.transform.position = new Vector3(1.2652f, 0.9581f, -4.3462f);
        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(ActorEvent_FakeCursorRaycastReceiver), "Start")]
        private static void FixStatueButtonInteractPositions(ActorEvent_FakeCursorRaycastReceiver __instance)
        {
            if (SceneManager.GetActiveScene().name != "MirrorRoom_B_P")
                return;

            if (__instance.fakeCursorProxy.name == "Button1")
            {
                GameObject newButton = UnityEngine.Object.Instantiate(__instance.fakeCursorProxy.gameObject);
                FakeCursorProxy newButtonProxy = newButton.GetComponent<FakeCursorProxy>();
                newButtonProxy.OnMouseClickEvent = __instance.fakeCursorProxy.OnMouseClickEvent;
                newButtonProxy.OnMouseEnterEvent = __instance.fakeCursorProxy.OnMouseEnterEvent;
                newButtonProxy.OnMouseExitEvent = __instance.fakeCursorProxy.OnMouseExitEvent;
                newButton.transform.GetChild(0).gameObject.SetActive(false);
                newButton.transform.position = new Vector3(-11.3615f, 1.1461f, 4.5268f);
            }
            else if (__instance.fakeCursorProxy.name == "Button2")
            {
                GameObject newButton = UnityEngine.Object.Instantiate(__instance.fakeCursorProxy.gameObject);
                FakeCursorProxy newButtonProxy = newButton.GetComponent<FakeCursorProxy>();
                newButtonProxy.OnMouseClickEvent = __instance.fakeCursorProxy.OnMouseClickEvent;
                newButtonProxy.OnMouseEnterEvent = __instance.fakeCursorProxy.OnMouseEnterEvent;
                newButtonProxy.OnMouseExitEvent = __instance.fakeCursorProxy.OnMouseExitEvent;
                newButton.transform.position = new Vector3(-11.2997f, 1.053f, 4.5416f);
                newButton.transform.GetChild(0).gameObject.SetActive(false);
            }
            else if (__instance.fakeCursorProxy.name == "Button3")
            {
                GameObject newButton = UnityEngine.Object.Instantiate(__instance.fakeCursorProxy.gameObject);
                FakeCursorProxy newButtonProxy = newButton.GetComponent<FakeCursorProxy>();
                newButtonProxy.OnMouseClickEvent = __instance.fakeCursorProxy.OnMouseClickEvent;
                newButtonProxy.OnMouseEnterEvent = __instance.fakeCursorProxy.OnMouseEnterEvent;
                newButtonProxy.OnMouseExitEvent = __instance.fakeCursorProxy.OnMouseExitEvent;
                newButton.transform.position = new Vector3(-11.3097f, 0.863f, 4.4606f);
                newButton.transform.GetChild(0).gameObject.SetActive(false);
            }
            else if (__instance.fakeCursorProxy.name == "Button4")
            {
                GameObject newButton = UnityEngine.Object.Instantiate(__instance.fakeCursorProxy.gameObject);
                FakeCursorProxy newButtonProxy = newButton.GetComponent<FakeCursorProxy>();
                newButtonProxy.OnMouseClickEvent = __instance.fakeCursorProxy.OnMouseClickEvent;
                newButtonProxy.OnMouseEnterEvent = __instance.fakeCursorProxy.OnMouseEnterEvent;
                newButtonProxy.OnMouseExitEvent = __instance.fakeCursorProxy.OnMouseExitEvent;
                newButton.transform.position = new Vector3(-11.2844f, 0.926f, 4.5643f);
                newButton.transform.GetChild(0).gameObject.SetActive(false);
            }
            else if (__instance.fakeCursorProxy.name == "Button5")
            {
                GameObject newButton = UnityEngine.Object.Instantiate(__instance.fakeCursorProxy.gameObject);
                FakeCursorProxy newButtonProxy = newButton.GetComponent<FakeCursorProxy>();
                newButtonProxy.OnMouseClickEvent = __instance.fakeCursorProxy.OnMouseClickEvent;
                newButtonProxy.OnMouseEnterEvent = __instance.fakeCursorProxy.OnMouseEnterEvent;
                newButtonProxy.OnMouseExitEvent = __instance.fakeCursorProxy.OnMouseExitEvent;
                newButton.transform.position = new Vector3(-11.302f, 0.828f, 4.5393f);
                newButton.transform.GetChild(0).gameObject.SetActive(false);
            }

        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(ActorEvent_OnOffRotator), "Awake")]
        private static void FixEngineRoomGasPuzzleInteractPositions(ActorEvent_OnOffRotator __instance)
        {
            if (__instance.name == "ValvulaD")
            {
                Transform electricBoxDoorPanel = __instance.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0);
                if (electricBoxDoorPanel.name == "Quad")
                    electricBoxDoorPanel.position = new Vector3(-1.5491f, 1.6008f, 2.3423f);
                Transform gasValveTrigger = __instance.transform.GetChild(2).GetChild(0);
                gasValveTrigger.position = new Vector3(-1.4283f, 1.4635f, 2.6013f);
            }
            else if (__instance.name == "ValvulaF")
            {
                Transform gasValveTrigger = __instance.transform.GetChild(6).GetChild(0);
                gasValveTrigger.position = new Vector3(-1.2478f, 1.4773f, 2.5699f);
            }
            else if (__instance.name == "Valvula A")
            {
                Transform gasValveTrigger = __instance.transform.GetChild(9).GetChild(0);
                gasValveTrigger.position = new Vector3(-1.032f, 1.7768f, 2.5815f);
            }
            else if (__instance.name == "ValvulaE")
            {
                Transform gasValveTrigger = __instance.transform.GetChild(6).GetChild(0);
                gasValveTrigger.position = new Vector3(-1.3469f, 1.4267f, 2.5815f);
            }

        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ActorEvent_FakeCursorRaycastReceiver), "Start")]
        private static void RepositionInteractions(ActorEvent_FakeCursorRaycastReceiver __instance)
        {
            if (__instance.name == "CandleRightClickReceiver_GameObject" && SceneManager.GetActiveScene().name == "Bathroom_E") // The right candle for the mirror
                __instance.transform.position = new Vector3(-1.426f, 1.3075f, -0.0461f);
            else if (__instance.name == "CandleLeftClickReceiver_GameObject" && SceneManager.GetActiveScene().name == "Bathroom_E") // The left candle for the mirror
                __instance.transform.position = new Vector3(-1.726f, 1.0075f, -0.5461f);
            else if (__instance.name == "CandleRightClickReceiver_GameObject") // The right candle for the mirror
                __instance.transform.position = new Vector3(3.8332f, -0.892f, -3.405f);
            else if (__instance.name == "CandleLeftClickReceiver_GameObject") // The left candle for the mirror
                __instance.transform.position = new Vector3(3.8841f, -0.892f, -2.9076f);
            else if (__instance.name == "BabyDoll_ChestReceiver") // The babys heart in the mirror world
                __instance.transform.position = new Vector3(1.981f, 1.352f, -5.504f);
            else if (__instance.name == "ElectricGiverSwitchCollider") // The babys heart in the mirror world
                __instance.transform.position = new Vector3(2.0536f, 1.2608f, -6.093f);
            else if (__instance.name == "Corridor_1A_TrapdoorRope_GameObject") // Trapdoor with rope on it
                __instance.transform.position = new Vector3(3.1741f, 0.24f, 7.9234f);
            else if (__instance.name == "ElectricLeverClickReceiver")
            {
                __instance.transform.position = new Vector3(2.0126f, 1.1932f, -6.008f);
                __instance.transform.parent.localRotation = Quaternion.identity;
            }
            else if (__instance.name == "Room2B_Past_Item") // Mannequin hand in chest
                __instance.fakeCursorProxy.transform.position = new Vector3(2.0958f, 0.7173f, 3.6944f);
            else if (__instance.name == "CoinSlotClickReceiver_GameObject") // Vending machine coin slot
                __instance.transform.position = new Vector3(-15.116f, 0.758f, -4.221f);
            else if (__instance.name == "CoinDropperClickReceiver_GameObject")// Vending machine coin dropper
                __instance.transform.position = new Vector3(-15.126f, 0.681f, -4.2092f);
            else if (__instance.name == "DrinkCanClickReceiver_GameObject")  // Vending machine item pickup
            {
                __instance.fakeCursorProxy.transform.position = new Vector3(-15.088f, 0.779f, -4.3973f);
                __instance.fakeCursorProxy.transform.localRotation = Quaternion.Euler(60, 0, 0);
            }
            else if (__instance.name == "FloppyDriveSlotClickReceiver_GameObject" && SceneManager.GetActiveScene().name == "Corridor_2D")// Floppy disk door lock
                __instance.fakeCursorProxy.transform.position = new Vector3(-11.9607f, 1.1277f, -1.3798f);
            else if (__instance.name == "FloppyDriveSlotClickReceiver_GameObject")// Computer floppy disk slot
                __instance.transform.position = new Vector3(-5.8631f, -0.8093f, 3.3889f);
            else if (__instance.name == "EjectButtonClickReceiver_GameObject" && SceneManager.GetActiveScene().name != "Corridor_2D")// Computer floppy disk eject button
                __instance.fakeCursorProxy.transform.localScale = new Vector3(0.1297f, 0.1118f, 0.11f);
            else if (__instance.name == "MoldLiquidClickReceiver")// Fridge slot
                __instance.fakeCursorProxy.transform.position = new Vector3(-0.5556f, 1.6956f, -3.1099f);
            else if (__instance.name == "FridgeSlot ")// Fridge slot
                __instance.transform.position = new Vector3(-0.5556f, 1.6956f, -3.1099f);
            else if (__instance.name == "RealRustyHatch_Handle_GameObject")// Rusty hatch door
                __instance.fakeCursorProxy.transform.position = new Vector3(2.77f, 1.316f, -12.003f);
            else if (__instance.name == "RealRustyHatch_TopGear_GameObject")// Rusty hatch door top gear
                __instance.fakeCursorProxy.transform.position = new Vector3(2.52f, 1.166f, -12.003f);
            else if (__instance.name == "RealRustyHatch_BottomGear_GameObject")// Rusty hatch door bottom gear
                __instance.fakeCursorProxy.transform.position = new Vector3(2.62f, 1.516f, -12.003f);
            else if (__instance.name == "Mausoleo_Collider_GameObject")// Statue hand 
                __instance.fakeCursorProxy.transform.position = new Vector3(-0.2406f, 0.807f, -14.29f);
            else if (__instance.name == "BatteryClickReceiver_GameObject" && SceneManager.GetActiveScene().name == "Library") // Library hydraulic lift battery
                __instance.fakeCursorProxy.transform.position = new Vector3(-14.4294f, 1.4791f, -3.2173f);
            else if (__instance.name == "SewerRoom_LeftClickReceiver") // Sewer bedroom mirror left candle
                __instance.fakeCursorProxy.transform.position = new Vector3(-4.4423f, 1.887f, 0.3838f);
            else if (__instance.name == "SewerRoom_RightClickReceiver") // Sewer bedroom mirror right candle
                __instance.fakeCursorProxy.transform.position = new Vector3(-4.6621f, 1.877f, 0.7939f);
            else if (__instance.name == "BathroomAPast_EyeReceiver_GameObject") // Bathtub eye 
                __instance.fakeCursorProxy.transform.position = new Vector3(-2.626f, 1.013f, -20.741f);
            else if (__instance.name == "Sewer_ButtonRight_GameObject") // Bathtub eye 
            {
                __instance.fakeCursorProxy.transform.localScale = new Vector3(0.1574f, 0.1096f, 0.1574f);
                __instance.transform.localRotation = Quaternion.identity;
            }
            else if (__instance.name == "BunkerEntrance_Start_GameObject") // Bunker green button
                __instance.fakeCursorProxy.transform.position = new Vector3(1.121f, -2.0664f, 1.2299f);
            else if (__instance.name == "BunkerEntrance_Stop_GameObject") // Bunker red button
                __instance.fakeCursorProxy.transform.position = new Vector3(1.0417f, -2.1317f, 1.3099f);
            else if (__instance.name == "BunkerEntrance_RightLock_GameObject") // Bunker lock
                __instance.fakeCursorProxy.transform.position = new Vector3(0.8918f, -2.1712f, 0.9148f);
            else if (__instance.name == "TopClickReceiver_GameObject") // Generator gas tank lid
                __instance.fakeCursorProxy.transform.position = new Vector3(-2.891f, -9.947f, -0.742f);
            else if (__instance.name == "RopeCLickReceiver_GameObject") // Generator Pull start
                __instance.fakeCursorProxy.transform.position = new Vector3(-2.858f, -9.958f, -0.955f);
            else if (SceneManager.GetActiveScene().name == "Bunker_2B") // Valve puzzle stuff
            {
                if (__instance.name == "PipesPlug0ClickReceiver")
                    __instance.fakeCursorProxy.transform.position = new Vector3(0.8571f, 1.6926f, -6.2104f);
                else if (__instance.name == "PipesPlug1ClickReceiver")
                    __instance.fakeCursorProxy.transform.position = new Vector3(0.9344f, 1.469f, -6.248f);
                else if (__instance.name == "PipesPlug2ClickReceiver")
                    __instance.fakeCursorProxy.transform.position = new Vector3(0.745f, 1.49f, -6.454f);
                else if (__instance.name == "PipesPlug3ClickReceiver")
                    __instance.fakeCursorProxy.transform.position = new Vector3(0.4122f, 1.5731f, -6.473f);
                else if (__instance.name == "PipesPlug4ClickReceiver")
                    __instance.fakeCursorProxy.transform.position = new Vector3(0.728f, 1.3521f, -6.5086f);
                else if (__instance.name == "PipesPlug5ClickReceiver")
                    __instance.fakeCursorProxy.transform.position = new Vector3(0.8758f, 1.336f, -6.3641f);
                else if (__instance.name == "PipesPlug6ClickReceiver")
                    __instance.fakeCursorProxy.transform.position = new Vector3(0.617f, 1.323f, -6.263f);
            }
            else if (__instance.name == "ExitDoorKeyClickReceiver") // Ending door receiver
                __instance.fakeCursorProxy.transform.position = new Vector3(-2.17f, 2.247f, -3.809f);
        }


        // The valve puzzle needs to have all the valve interatable positions moved
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GasPipelineValve), "SetupButton")]
        private static void RepositionValveInteractions(GasPipelineValve __instance)
        {
            if (__instance.name == "Logic0")
                __instance.transform.position = new Vector3(0.784f, 1.6692f, -6.2766f);
            else if (__instance.name == "Logic1")
                __instance.transform.position = new Vector3(0.8667f, 1.5546f, -6.5771f);
            else if (__instance.name == "Logic2")
                __instance.transform.position = new Vector3(0.741f, 1.558f, -6.5771f);
            else if (__instance.name == "Logic3")
                __instance.transform.position = new Vector3(0.6248f, 1.559f, -6.6742f);
            else if (__instance.name == "Logic4")
                __instance.transform.position = new Vector3(0.567f, 1.5239f, -6.6771f);
            else if (__instance.name == "Logic5")
                __instance.transform.position = new Vector3(0.4795f, 1.5459f, -6.5798f);
            else if (__instance.name == "Logic6")
                __instance.transform.position = new Vector3(0.8667f, 1.447f, -6.3771f);
            else if (__instance.name == "Logic7")
                __instance.transform.position = new Vector3(0.567f, 1.4098f, -6.3771f);
            else if (__instance.name == "Logic8")
                __instance.transform.position = new Vector3(0.674f, 1.344f, -6.5771f);
        }

        // The vending machine interactables are tied to the button mesh so create a new button with the button component so it can be repositioned
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SpendingMachineButton), "SetupButton")]
        private static void RepositionVendingMachineButtons(SpendingMachineButton __instance)
        {
            if (__instance.transform.parent.name == "Button" && __instance.transform.childCount == 0)
            {
                GameObject newButton = UnityEngine.Object.Instantiate(__instance.gameObject);
                newButton.GetComponent<MeshRenderer>().enabled = false;
                SpendingMachineButton buttonComp = newButton.GetComponent<SpendingMachineButton>();
                buttonComp.OnButtonPressed = __instance.OnButtonPressed;
                buttonComp.m_id = __instance.m_id;
                newButton.transform.position = new Vector3(-15.135f, 0.841f, -4.16f);
                newButton.transform.localRotation = Quaternion.Euler(0, 90, 0);
                newButton.transform.parent = __instance.transform;
            }
            else if (__instance.transform.parent.name == "Button (1)" && __instance.transform.childCount == 0)
            {
                GameObject newButton = UnityEngine.Object.Instantiate(__instance.gameObject);
                newButton.GetComponent<MeshRenderer>().enabled = false;
                SpendingMachineButton buttonComp = newButton.GetComponent<SpendingMachineButton>();
                buttonComp.OnButtonPressed = __instance.OnButtonPressed;
                buttonComp.m_id = __instance.m_id;
                newButton.transform.position = new Vector3(-15.1162f, 0.878f, -4.1942f);
                newButton.transform.localRotation = Quaternion.Euler(0, 90, 0);
                newButton.transform.parent = __instance.transform;
            }
            else if (__instance.transform.parent.name == "Button (2)" && __instance.transform.childCount == 0)
            {
                GameObject newButton = UnityEngine.Object.Instantiate(__instance.gameObject);
                newButton.GetComponent<MeshRenderer>().enabled = false;
                SpendingMachineButton buttonComp = newButton.GetComponent<SpendingMachineButton>();
                buttonComp.OnButtonPressed = __instance.OnButtonPressed;
                buttonComp.m_id = __instance.m_id;
                newButton.transform.position = new Vector3(-15.1162f, 0.922f, -4.1942f);
                newButton.transform.localRotation = Quaternion.Euler(0, 90, 0);
                newButton.transform.parent = __instance.transform;
            }
            else if (__instance.transform.parent.name == "Button (3)" && __instance.transform.childCount == 0)
            {
                GameObject newButton = UnityEngine.Object.Instantiate(__instance.gameObject);
                newButton.GetComponent<MeshRenderer>().enabled = false;
                SpendingMachineButton buttonComp = newButton.GetComponent<SpendingMachineButton>();
                buttonComp.OnButtonPressed = __instance.OnButtonPressed;
                buttonComp.m_id = __instance.m_id;
                newButton.transform.position = new Vector3(-15.1162f, 0.963f, -4.1942f);
                newButton.transform.localRotation = Quaternion.Euler(0, 90, 0);
                newButton.transform.parent = __instance.transform;
            }
            else if (__instance.transform.parent.name == "Button (4)" && __instance.transform.childCount == 0)
            {
                GameObject newButton = UnityEngine.Object.Instantiate(__instance.gameObject);
                newButton.GetComponent<MeshRenderer>().enabled = false;
                SpendingMachineButton buttonComp = newButton.GetComponent<SpendingMachineButton>();
                buttonComp.OnButtonPressed = __instance.OnButtonPressed;
                buttonComp.m_id = __instance.m_id;
                newButton.transform.position = new Vector3(-15.1162f, 1.012f, -4.1942f);
                newButton.transform.localRotation = Quaternion.Euler(0, 90, 0);
                newButton.transform.parent = __instance.transform;
            }
        }
    }
}
