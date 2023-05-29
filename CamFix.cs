using HarmonyLib;
using System;
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
        }


        // Need to manually set the virtual camera positions for a lot of interactable stuff
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ActorEvent_VirtualCamera), "Start")]
        private static void SetVirtualCamPosition(ActorEvent_VirtualCamera __instance)
        {
            string camName = __instance.name;
            if (camName == "ShelfCamera") // Shelf at the start of the game
                __instance.virtualCamera.transform.position = new Vector3(1.918f, 1.6789f, -21.771f);
            else if (camName == "PadlockCameraActor") // Padlock on the shelf at the start of the game
                __instance.virtualCamera.transform.position = new Vector3(2.0541f, 1.6139f, -22.03f);
            else if (camName == "DoorCameraActor") // Door in the starting room you need to use the wrench on
                __instance.virtualCamera.transform.position = new Vector3(-0.753f, 0.9859f, -20.391f);
            else if (camName == "Xray_A_VirtualCamera0_GameObject") // The X-ray room save
            {
                __instance.virtualCamera.transform.position = new Vector3(-16.006f, 1.084f, 2.493f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0f, 194f, 0f);
            }
            else if (camName == "MotorPuzzleCamera_GameObject") { // The motor thing in engine room
                __instance.virtualCamera.transform.position = new Vector3(-5.0709f, 2.2117f, 0.5292f);
                __instance.virtualCamera.transform.rotation = Quaternion.identity;
            }
            else if (camName == "ValvePuzzleNewCamera_GameObject") // The engine room gas box door
            {
                __instance.virtualCamera.transform.position = new Vector3(-1.3899f, 1.546f, 1.6511f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0,273,0);
            }
            else if (camName == "ValvePuzzleCloserCamera_GameObject") // The engine room gas box inside
            {
                __instance.virtualCamera.transform.position = new Vector3(-1.1139f, 1.5912f, 2.1472f);
                __instance.virtualCamera.transform.rotation = Quaternion.Euler(0, 90, 0);
            }
            else if (__instance.transform.parent.parent.name == "TapeRecorder" && SceneManager.GetActiveScene().name == "ExamRoom") // The exam room save
            {
                __instance.virtualCamera.transform.position = new Vector3(-6.4032f, 1.4941f, -0.5686f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0f, 268f, 0f);
            }
            else if (SceneManager.GetActiveScene().name == "SewingRoom") // The sewing room save
            {
                __instance.virtualCamera.transform.position = new Vector3(-2.7388f, 1.4861f, -1.3017f);
                __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0f, 354f, 0f);
            }
        }
//        string camName = __instance.name;
//            if (camName == "ShelfCamera")
//                __instance.virtualCamera.transform.position = new Vector3(1.918f, 1.6789f, -21.771f);
//            else if (camName == "PadlockCameraActor")
//                __instance.virtualCamera.transform.position = new Vector3(2.0541f, 1.6139f, -22.03f);
//            else if (camName == "DoorCameraActor")
//                __instance.virtualCamera.transform.position = new Vector3(-0.753f, 0.9859f, -20.391f);
//            else if (camName == "Xray_A_VirtualCamera0_GameObject")
//            {
//                __instance.virtualCamera.transform.position = new Vector3(-16.106f, 0.884f, 2.693f);
//        __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0f, 194f, 0f);
//            }
//            else if (camName == "MotorPuzzleCamera_GameObject") {
//                __instance.virtualCamera.transform.position = new Vector3(-5.0709f, 2.2117f, 0.5292f);
//    __instance.virtualCamera.transform.rotation = Quaternion.identity;
//            }
//            else if (__instance.transform.parent.parent.name == "TapeRecorder" && SceneManager.GetActiveScene().name == "ExamRoom")
//{
//    __instance.virtualCamera.transform.position = new Vector3(-6.4162f, 1.4941f, -0.3169f);
//    __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0f, 268f, 0f);
//}
//else if (SceneManager.GetActiveScene().name == "SewingRoom")
//{
//    __instance.virtualCamera.transform.position = new Vector3(-2.7388f, 1.4861f, -1.3017f);
//    __instance.virtualCamera.transform.localRotation = Quaternion.Euler(0f, 354f, 0f);
//}

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
                    vrCamera.backgroundColor = Color.black;
                    vrCamera.gameObject.AddComponent<SteamVR_TrackedObject>();
                    UnityEngine.Object.DontDestroyOnLoad(camRoot);
                    headsetPos = CamFix.vrCamera.transform.localPosition;

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
            //Vector3 movementVector = new Vector3(vector.x, 0f, vector.y);
            //movementVector.Normalize();
            float rotationSpeed = 100f; 

            // Calculate the rotation amount based on the joystick rotation
           
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
            else 
                camRoot.transform.rotation = Quaternion.Lerp(camRoot.transform.rotation, __instance.transform.rotation, 8 * Time.deltaTime);

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
