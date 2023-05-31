using HarmonyLib;
using System;
using TormentedSoulsVR.VRBody;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using Valve.VR;
using static FlowDirectorManager;
using static Rewired.ComponentControls.Effects.RotateAroundAxis;

namespace TormentedSoulsVR
{
    [HarmonyPatch]
    internal class CamFix
    {
        public static GameObject camHolder;
        public static Camera vrCamera;
        public static GameObject camRoot;
        public static GameplayMenuManager menus;

        public static VRCrouch crouchInstance;

        public static PlayerController player;

        private static bool vrStarted = false;

        public static bool inCinematic = false;

        public static bool inIntro = true;

        public static bool onTapeRecorder = false;

        public static Vector3 headsetPos = Vector3.zero;

        public static VRHandler vrHandlerInstance;



        [HarmonyPostfix]
        [HarmonyPatch(typeof(ActorEvent_OnOffRotator), "Awake")]
        private static void FixEngineRoomGasPuzzleInteractPositions(ActorEvent_OnOffRotator __instance)
        {
            if (__instance.name == "ValvulaD") {
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
        private static void RepositionMirrorCandleInteractions(ActorEvent_OnOffRotator __instance) {
            if (__instance.name == "CandleRightClickReceiver_GameObject") // The right candle for the mirror
                __instance.transform.position = new Vector3(3.8332f, -0.892f, -3.405f);
            else if (__instance.name == "CandleLeftClickReceiver_GameObject") // The left candle for the mirror
                __instance.transform.position = new Vector3(3.8841f, -0.892f, -2.9076f);
            else if (__instance.name == "BabyDoll_ChestReceiver") // The babys heart in the mirror world
                __instance.transform.position = new Vector3(1.981f, 1.352f, -5.504f);
            else if (__instance.name == "ElectricGiverSwitchCollider") // The babys heart in the mirror world
                __instance.transform.position = new Vector3(2.0536f, 1.2608f, -6.093f);
            else if (__instance.name == "ElectricLeverClickReceiver") { 
                __instance.transform.position = new Vector3(2.0126f, 1.1932f, -6.008f);
                __instance.transform.parent.localRotation = Quaternion.identity;
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(HintableBehaviour), "Awake")]
        private static void FixBandageRemovalCam(HintableBehaviour __instance)
        {
            if (__instance.transform.parent.name != "Cinematic1_CameraChanges")
                return;

            Transform camToMove = __instance.transform.parent.GetChild(4).GetChild(3);
            camToMove.position = new Vector3(-2.291f, 1.8268f, -26.4342f);
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(LightZoneColliderUpdater), "Start")]
        private static void ChangeLighterColour(LightZoneColliderUpdater __instance)
        {
            __instance.parentLight.color = new Color(1, 0.9355f, 0.6415f, 0.2533f);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerWeaponBehaviour), "InitialSetup")]
        private static void ApplyArmIK(PlayerWeaponBehaviour __instance)
        {
            if (__instance.RightHandWeaponPoint.transform.parent.gameObject.GetComponent<VRBody.ArmIK>() == null) { 
                __instance.RightHandWeaponPoint.transform.parent.gameObject.AddComponent<VRBody.ArmIK>();
                __instance.LeftHandWeaponPoint.transform.parent.gameObject.AddComponent<VRBody.ArmIK>();
            }
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerDetector), "ForcePlayerExitFromCollider")]
        private static void EnableCamSwapOnHintInteract(PlayerDetector __instance)
        {
            CamFix.inCinematic = true;
            headsetPos = CamFix.vrCamera.transform.localPosition;
            CamFix.menus.transform.localPosition = new Vector3(0, 0, 0.3f);
            if (__instance.transform.parent.parent.name == "Xray_A_TriggerFlow2_GameObject")
                CamFix.menus.transform.localPosition = new Vector3(0,0,0.3f);
            else if (__instance.transform.parent.parent.parent.parent.name == "TapeRecorder") {
                vrHandlerInstance.SetMenuPosOnSave();
            }

        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerController), "OnEnable")]
        private static void ReturnCamViewToPlayerOnEnable(PlayerController __instance)
        {
            CamFix.inCinematic = false;
            player = __instance;
            if (CamFix.menus != null)
                vrHandlerInstance.SetMenuPosOnExitSave();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerController), "Awake")]
        private static void ReturnCamViewToPlayerOnAwake(PlayerController __instance)
        {
            CamFix.inCinematic = false;
            player = __instance;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerController), "InitializeStates")]
        private static void ReturnCamViewToPlayerOnInit(PlayerController __instance)
        {
            player = __instance;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerController), "SpawnPlayerOnPosition")]
        private static void ReturnCamViewToPlayerOnSpawn(PlayerController __instance)
        {
            if (camRoot.transform.childCount >= 2) { 
                camRoot.transform.GetChild(1).localRotation = Quaternion.identity;
            }
            camRoot.transform.rotation = __instance.transform.rotation;
            player = __instance;
            AudioListener OGCamAudio = Camera.main.GetComponent<AudioListener>();
            if (OGCamAudio != null)
                OGCamAudio.enabled = false;

        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(ItemOptionMenuButtonBehaviour), "Start")]
        private static void SetDeathScreenCanvas(ItemOptionMenuButtonBehaviour __instance)
        {
            if (__instance.name != "MenuButton (1)" && __instance.transform.root.name != "Canvas")
                return;

            Transform deathScreen = __instance.transform.root;
            deathScreen.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            deathScreen.localScale = new Vector3(0.006f, 0.006f, 0.006f);
            deathScreen.position = new Vector3(0,1,6);
            deathScreen.rotation = Quaternion.identity;

            CamFix.camRoot.transform.position = Vector3.zero;
            CamFix.camRoot.transform.rotation = Quaternion.identity;
        }



        // Need to manually set the virtual camera positions for a lot of interactable stuff
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ActorEvent_VirtualCamera), "Start")]
        private static void SetVirtualCamPosition(ActorEvent_VirtualCamera __instance)
        {
            string camName = __instance.name;
            if (camName == "ShelfCamera")                                                                                           // Shelf at the start of the game
                __instance.virtualCamera.transform.position = new Vector3(1.918f, 1.6789f, -21.771f);
            else if (camName == "PadlockCameraActor")                                                                               // Padlock on the shelf at the start of the game
                __instance.virtualCamera.transform.position = new Vector3(2.1541f, 1.6139f, -22.03f);
            else if (camName == "DoorCameraActor")                                                                                  // Door in the starting room you need to use the wrench on
                __instance.virtualCamera.transform.position = new Vector3(-0.753f, 0.9859f, -20.391f);
            else if (camName == "Xray_A_VirtualCamera0_GameObject")                                                                 // The X-ray room save
            {
                __instance.virtualCamera.transform.position = new Vector3(-16.006f, 1.084f, 2.493f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0f, 194f, 0f);
            }
            else if (camName == "MotorPuzzleCamera_GameObject")                                                                     // The motor thing in engine room
            { 
                __instance.virtualCamera.transform.position = new Vector3(-5.0709f, 2.2117f, 0.5292f);
                __instance.virtualCamera.transform.rotation = Quaternion.identity;
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
            else if (camName == "HydraulicTable")                                                                                   // Library hydraulic lift puzzle
            {
                __instance.virtualCamera.transform.position = new Vector3(-14.896f, 1.673f, -3.2214f);
                __instance.virtualCamera.transform.parent.GetChild(1).position = new Vector3(-14.509f, 1.558f, -3.2696f);
            }
            else if (camName == "FuseboxReception_Camera_GameObject" && SceneManager.GetActiveScene().name == "Corridor_2A")        // Reception area fuse box
            {
                __instance.virtualCamera.transform.position = new Vector3(-6.282f, 1.7057f, - 7.1045f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0f, 289f, 0f);
            }
            else if (camName == "FuseboxReception_Camera_GameObject")                                                               // Reception area fuse box
            {
                __instance.virtualCamera.transform.position = new Vector3(2.873f, 2.121f, 1.834f);
                __instance.virtualCamera.transform.parent.GetChild(1).position = new Vector3(2.578f, 1.978f, 1.28f);
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
            else if (SceneManager.GetActiveScene().name == "Bathroom_C")                                                            // Definitely not a bathroom, more like a small office
            {
                __instance.virtualCamera.transform.position = new Vector3(3.0959f, 1.6061f, -0.5924f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0f, 86f, 0f);
            }
            else if (SceneManager.GetActiveScene().name == "Room_2E")                                                               // Upstairs break room save
            {
                __instance.virtualCamera.transform.position = new Vector3(-3.3754f, 1.0501f, -0.405f);
                __instance.virtualCamera.transform.parent.GetChild(1).position = new Vector3(-3.8732f, 0.7211f, 2.8718f);
            }
        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(ViewOptionsMenu), "Start")]
        private static void InjectVR(ViewOptionsMenu __instance)
        {
            if (!vrStarted && GameDB.instance != null)
            {
                //layer and volme
                vrStarted = true;
                if (camRoot == null)
                {
                    vrHandlerInstance = GameDB.instance.gameObject.AddComponent<VRHandler>();
                    camRoot = new GameObject("camRoot");
                    camHolder = new GameObject("camHolder");
                    camHolder.transform.parent = camRoot.transform;
                    vrCamera = new GameObject("VRCamera").AddComponent<Camera>();
                    //vrCamera.CopyFrom(Camera.main);
                    vrCamera.renderingPath = RenderingPath.DeferredShading;
                    vrCamera.transform.parent = camHolder.transform;
                    vrCamera.nearClipPlane = 0.001f;
                    vrCamera.clearFlags = CameraClearFlags.SolidColor;
                    vrCamera.backgroundColor = Color.black;
                    vrCamera.gameObject.AddComponent<SteamVR_TrackedObject>();
                    UnityEngine.Object.DontDestroyOnLoad(camRoot);
                    headsetPos = CamFix.vrCamera.transform.localPosition;
                    vrCamera.gameObject.AddComponent<AudioListener>();

                }
                else {
                    camRoot.transform.position = Vector3.zero;
                    camRoot.transform.rotation = Quaternion.identity;
                    camRoot.transform.localRotation = Quaternion.identity;
                }
                Canvas mainScreenCanvas = __instance.transform.parent.GetComponent<Canvas>();
                mainScreenCanvas.renderMode = RenderMode.WorldSpace;
                mainScreenCanvas.transform.localScale = new Vector3(0.002f, 0.002f, 0.002f);
                mainScreenCanvas.transform.position = new Vector3(0, 1, 4.3f);
                camHolder.transform.localPosition = headsetPos * -1;
                
                CameraManager.Setup();

            }
        }


        // Sets up the main menu screen to select the first available button
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ExitGameWaringController), "ButtonPressed")]
        private static void ResetCanvasOnReturnToMenu(ExitGameWaringController __instance, bool exitButton)
        {
            if (exitButton) { 
                // Change vrStarted to false so it will fix the main menus position and everything when returning to menu
                vrStarted = false;
                inCinematic = false;
                // Delete the in-game menu manager as its regenerated when loading a save or starting a new game
                if (camRoot.transform.childCount >= 2)
                    UnityEngine.Object.Destroy(camRoot.transform.GetChild(1).gameObject);
            }
        }




        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerSM_Aim), "ManageInput")]
        public static bool DisableAimingModeRotation(PlayerSM_Aim __instance, UserInputManager.UserAction action, object payload) {

            if (action == UserInputManager.UserAction.ReloadButton && __instance.m_weaponBase.CanReload())
                __instance.m_anim.SetTrigger("Reload");
            if (action == UserInputManager.UserAction.StartRunButton && ((Tuple<bool, bool>)payload).Item2)
                __instance.m_anim.SetTrigger("Backdash");
            if (action != UserInputManager.UserAction.ShootButton)
                return false;
            if (!__instance.m_playerManager.IsInLightZone())
            {
                __instance.m_playerController.triggerShootInShadowsMessage?.Invoke();
                return false;
            }
            __instance.m_anim.SetFloat("WeaponType", __instance.m_weaponBase.GetWeaponAnimatorFloat());
            if (__instance.m_weaponBase.CanShoot() || __instance.m_weaponBase.weaponType == WeaponType.Crowbar)
                __instance.m_anim.SetTrigger("Shoot");
            else if (__instance.m_weaponBase.CanReload())
                __instance.m_anim.SetTrigger("Reload");
            else
                __instance.m_anim.SetTrigger("EmptyShoot");
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerSM_LocomotionMachine), "ExitStateManually")]
        public static bool PreventMovementBlockingOnAim(PlayerSM_LocomotionMachine __instance)
        {
            PlayerController playerController = __instance.m_playerController;
            playerController.OnHitEvent = (Action)Delegate.Remove(playerController.OnHitEvent, new Action(__instance.OnHit));
            //__instance.m_anim.SetBool("OnLocomotion", value: false);
            //PlayerController playerController2 = __instance.m_playerController;
            //playerController2.UserActionEvent = (Action<UserInputManager.UserAction, object>)Delegate.Remove(playerController2.UserActionEvent, new Action<UserInputManager.UserAction, object>(__instance.m_moveBehaviour.ManageInput));
            //PlayerController playerController3 = __instance.m_playerController;
            //playerController3.UserActionEvent = (Action<UserInputManager.UserAction, object>)Delegate.Remove(playerController3.UserActionEvent, new Action<UserInputManager.UserAction, object>(__instance.OnUserActionEvent));
            //__instance.m_moveBehaviour.EnableMovement();
            return false;

        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerMovement), "Movement")]
        private static bool SetVRSpeedAndMoveAnim(PlayerMovement __instance, Vector2 input, bool dpad = false)
        {
            //if (!__instance.m_anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") && !__instance.m_anim.GetCurrentAnimatorStateInfo(0).IsName("Movement"))
            //    return false
            Vector2 vector = input;
            bool animatorParams = false;

            if (dpad)
            {
                return true;
            }
            // Else if joystick is being used
            else
            {

                // vector is the input, if its not zero then
                if (vector != Vector2.zero)
                {
                    // This calculates a rotation angle based on the input X and Y components

                }
                float magnitude = vector.magnitude;
                if (__instance.stairsTriggers.Count > 0)
                {
                    if (Mathf.Abs(__instance.rotationAngle) < 90f)
                    {
                        __instance.animationMove = PlayerMovement.AnimationMove.StairsUp;
                        if (magnitude > __instance.m_walkInputRange.max)
                            __instance.speed = __instance.m_runSpeedStairsUp;
                        else if (magnitude > __instance.m_walkInputRange.min)
                            __instance.speed = __instance.m_walkSpeedStairsUp;
                        else
                            __instance.speed = 0f;
                    }
                    else
                    {
                        __instance.animationMove = PlayerMovement.AnimationMove.StairsUp;
                        if (magnitude > __instance.m_walkInputRange.max)
                            __instance.speed = __instance.m_runSpeedStairsDown;
                        else if (magnitude > __instance.m_walkInputRange.min)
                            __instance.speed = __instance.m_walkSpeedStairsDown;
                        else
                            __instance.speed = 0f;
                    }
                }
                __instance.animationMove = PlayerMovement.AnimationMove.FloorForw;
                if (magnitude > __instance.m_walkInputRange.max)
                    __instance.speed = __instance.m_runSpeed;
                else if (magnitude > __instance.m_walkInputRange.min)
                    __instance.speed = __instance.m_walkSpeed;
                else
                    __instance.speed = 0f;

                if (SteamVR_Actions._default.LeftJoystick.axis.y < -0.4)
                {
                    __instance.animationMove = PlayerMovement.AnimationMove.FloorBack;
                    __instance.speed = __instance.speed / 2f; 
                }
            }
            // After calculating the speed based on the factors above, add it to the current speed and start setting the body, animator and sound vars
            float num6 = __instance.speed * __instance.GetCurrentSpeedMultiplier();
            __instance.SetRigidbodySpeed(num6);
            __instance.SetAnimatorParams(animatorParams);
            __instance.SetIsMakingSound(__instance.IsRunning(num6));



            float rotationSpeed = 100f; 

           
            float rotationAmount = 0;
            if (__instance.speed != 0)
            {
                Vector3 vrRot = CameraManager.LeftHand.transform.eulerAngles;
                Vector3 bodyRot = __instance.transform.rotation.eulerAngles;
                // If there is a difference of x between the body and camera rotation
                if (Mathf.DeltaAngle(vrRot.y, bodyRot.y) > 17.5f)
                    // for every x degrees of difference in body and camera rotation, rotate the player x * -2f to rotate left or x * 2f to rotate right
                    rotationAmount = -2f * (Mathf.DeltaAngle(vrRot.y, bodyRot.y) / 17.5f);
                else if (Mathf.DeltaAngle(vrRot.y, bodyRot.y) < -17.5f)
                    rotationAmount = 2f * (Mathf.DeltaAngle(vrRot.y, bodyRot.y) / -17.5f);
                //camRoot.transform.Rotate(0f, rotationAmount * -1, 0f, Space.World);
            }
            else if (Mathf.Abs(camRoot.transform.eulerAngles.y - __instance.transform.rotation.eulerAngles.y) > 45 )
                camRoot.transform.rotation = Quaternion.Lerp(camRoot.transform.rotation, __instance.transform.rotation, 8 * Time.deltaTime);

            // Calculate the rotation amount based on the joystick rotation
            float joystickRotAmount = SteamVR_Actions._default.RightJoystick.axis.x * rotationSpeed * Time.deltaTime;
            rotationAmount += joystickRotAmount;
            camRoot.transform.Rotate(0, joystickRotAmount,0);
            // Apply the rotation to the __instance variable
            __instance.transform.Rotate(0f, rotationAmount, 0f);
            //if (__instance.speed != 0)
            //    __instance.transform.rotation = Quaternion.Euler(0, CameraManager.LeftHand.transform.eulerAngles.y, 0);
            
            
            
            return false;


        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerMovement), "SetRigidbodySpeed")]
        public static bool AllowVRMovement(PlayerMovement __instance, float targetSpeed)
        {
            //__instance.currentSpeed = Mathf.SmoothDamp(__instance.currentSpeed, targetSpeed, ref __instance.speedSmoothVelocity, __instance.speedSmoothTime * Time.deltaTime * 30f);
            ////Vector3 vector = __instance.transform.forward * __instance.currentSpeed;
            //Vector3 vector = __instance.transform.right * (__instance.currentSpeed * SteamVR_Actions._default.LeftJoystick.axis.x);
            //__instance.m_rigidbody.velocity = new Vector3(__instance.m_rigidbody.velocity.x, vector.y, vector.z);

            __instance.currentSpeed = Mathf.SmoothDamp(__instance.currentSpeed, targetSpeed, ref __instance.speedSmoothVelocity, __instance.speedSmoothTime * Time.deltaTime * 30f);

            Vector3 input = new Vector3(SteamVR_Actions._default.LeftJoystick.axis.x, 0f, SteamVR_Actions._default.LeftJoystick.axis.y); // Get input for both horizontal and vertical axes
            
            // This gets the difference between the player body and the camera
            Vector3 camDistanceFromBody = __instance.transform.InverseTransformPoint(vrCamera.transform.position);
            // If the users x and y hmd axis is further away than 0.5 something must be wrong, so limit it to -+0.5
            camDistanceFromBody.x = Mathf.Clamp(camDistanceFromBody.x, -0.5f, 0.5f);
            camDistanceFromBody.z = Mathf.Clamp(camDistanceFromBody.z, -0.5f, 0.5f);


            // If the camera is beyond -+0.1 distance from the body, then move it in that direction
            if (camDistanceFromBody.x >= 0.1f || camDistanceFromBody.x <= -0.1f)
            {
                input.x += camDistanceFromBody.x * 2f;
                // Since the cam holder is a child of the player body, we need to offset the movement with this
                headsetPos.x += ((camDistanceFromBody.x / 1.5f) * Time.deltaTime);
                __instance.currentSpeed += 0.5f;
            }
            if (camDistanceFromBody.z > 0.1f || camDistanceFromBody.z <= -0.05f)
            {
                input.z += camDistanceFromBody.z * 2f;
                headsetPos.z += ((camDistanceFromBody.z / 1.5f) * Time.deltaTime);
                __instance.currentSpeed += 0.5f;
                if (camDistanceFromBody.z < -0.1f)
                    __instance.currentSpeed += 0.5f;

            }
            Vector3 movement = __instance.transform.right * input.x + __instance.transform.forward * input.z; // Calculate movement vector


            __instance.m_rigidbody.velocity = new Vector3(movement.x * __instance.currentSpeed, __instance.m_rigidbody.velocity.y, movement.z * __instance.currentSpeed);
            if ((double)__instance.m_rigidbody.velocity.sqrMagnitude < 0.01)
            {
                __instance.m_rigidbody.velocity = Vector3.zero;
            }
            return false;
        }



        [HarmonyPostfix]
        [HarmonyPatch(typeof(BloodBehaviour), "Start")]
        public static void DisableHeadMeshes(BloodBehaviour __instance)
        {
            headsetPos = CamFix.vrCamera.transform.localPosition;
            int[] childrenMeshesToDisable = { 0, 9, 14, 15, 16, 17, 18, 20, 23, 27, 28 };
            if (childrenMeshesToDisable[childrenMeshesToDisable.Length - 1] + 1 != __instance.transform.childCount)
                return;
            for (int i = 0; i < childrenMeshesToDisable.Length; i++) {
                __instance.transform.GetChild(childrenMeshesToDisable[i]).GetComponent<SkinnedMeshRenderer>().enabled = false;
            }
            
        }


        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(PlayerMovement), "ManageInput")]
        //public static bool EnableMovementWhenAiming(PlayerMovement __instance, UserInputManager.UserAction action, object payload) {
        //    if (__instance.FastTurnTrigger && (__instance.MovementEnabled || isAiming))
        //    {
        //        __instance.FastTurn();
        //        return false;
        //    }
        //    if (action == UserInputManager.UserAction.LeftAxisMove)
        //    {
        //        Vector2 vector = (Vector2)payload;
        //        __instance.ResetCurrentScene(vector);
        //        if ((__instance.MovementEnabled || isAiming) && !__instance.GamePaused)
        //        {
        //            __instance.Movement(vector);
        //        }
        //    }
        //    if (action == UserInputManager.UserAction.DPADMove)
        //    {
        //        Vector2 vector2 = (Vector2)payload;
        //        __instance.ResetCurrentScene(vector2);
        //        if ((__instance.MovementEnabled || isAiming) && !__instance.GamePaused && vector2.sqrMagnitude > 0f)
        //        {
        //            __instance.Movement(vector2, dpad: true);
        //        }
        //    }
        //    if (action == UserInputManager.UserAction.StartRunButton)
        //    {
        //        __instance.m_run = true;
        //    }
        //    if (action == UserInputManager.UserAction.StopRunButton)
        //    {
        //        __instance.m_run = false;
        //    }
        //    switch (action)
        //    {
        //        case UserInputManager.UserAction.FastTurn:
        //            __instance.FastTurnTrigger = true;
        //            __instance.CurrentActionFrame = 0f;
        //            break;
        //        case UserInputManager.UserAction.StartRunButton:
        //            __instance.CurrentActionFrame = 0f;
        //            break;
        //    }
        //    return false;
        //}

    }
}
