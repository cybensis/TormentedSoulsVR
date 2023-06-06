using HarmonyLib;
using System;
using TormentedSoulsVR.UI;
using TormentedSoulsVR.VRBody;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;


namespace TormentedSoulsVR.cam
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





        // Moves one of the cinematic cams on the bandage removal intro scene
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SceneCinematicRefs), "GetAllSceneCinematics")]
        private static void FixBandageRemovalCam(SceneCinematicRefs __instance)
        {
            if (SceneManager.GetActiveScene().name != "Bathroom_A")
                return;
            Transform camToMove = __instance.sceneCinematics[1].transform.GetChild(4).GetChild(3);
            camToMove.position = new Vector3(-2.791f, 1.8268f, -25.7342f);
        }


        // The lighter colour kind of blocks vision in first person so change it
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LightZoneColliderUpdater), "Start")]
        private static void ChangeLighterColour(LightZoneColliderUpdater __instance)
        {
            __instance.parentLight.color = new Color(1, 0.9355f, 0.6415f, 0.2533f);
        }


        // Add IK to the arms
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerWeaponBehaviour), "InitialSetup")]
        private static void ApplyArmIK(PlayerWeaponBehaviour __instance)
        {
            if (__instance.RightHandWeaponPoint.transform.parent.gameObject.GetComponent<ArmIK>() == null)
            {
                __instance.RightHandWeaponPoint.transform.parent.gameObject.AddComponent<ArmIK>();
                __instance.LeftHandWeaponPoint.transform.parent.gameObject.AddComponent<ArmIK>();
            }
        }



        // The player is disabled in most cutscenes so when they're enabled it likely means the cinematic has ended so return cam to player
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerController), "OnEnable")]
        private static void ReturnCamViewToPlayerOnEnable(PlayerController __instance)
        {
            inCinematic = false;
            player = __instance;
            if (menus != null)
                vrHandlerInstance.SetMenuPosOnExitSave();
        }

        // These two patches are just to be safe, making sure the player gets set and cinematics end
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerController), "Awake")]
        private static void ReturnCamViewToPlayerOnAwake(PlayerController __instance)
        {
            inCinematic = false;
            player = __instance;
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerController), "InitializeStates")]
        private static void SetPlayerInstanceOnInit(PlayerController __instance)
        {
            player = __instance;
        }

        // This is called when a player enters a new area
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerController), "SpawnPlayerOnPosition")]
        private static void ReturnCamViewToPlayerOnSpawn(PlayerController __instance)
        {
            // Set the local rotation of the menu to nothing when entering a new area
            if (menus != null)
                menus.transform.localRotation = Quaternion.identity;

            HUDPatches.targetHUDYRot = 0f;
            // Init camRoot rotation to the player body rotation
            camRoot.transform.rotation = __instance.transform.rotation;
            player = __instance;
            //player.m_moveBehaviour.m_runSpeed = 7f;

            // VR Cam needs to have the same culling mask because in the mirror world areas, both meshes are rendered at the same time, but the culling mask hides one of them
            vrCamera.cullingMask = Camera.main.cullingMask;
            // For some reason the player models gets bigger in the sewer part so increase the height for this scene
            if (SceneManager.GetActiveScene().name == "Sewer")
                vrHandlerInstance.heightOffset = 1.82f;
            else
                vrHandlerInstance.heightOffset = 1.575f;
            AudioListener OGCamAudio = Camera.main.GetComponent<AudioListener>();
            if (OGCamAudio != null)
                OGCamAudio.enabled = false;

        }


        // Need to move the statue eye interactable down in the clock puzzle to fit everything in 
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PuzzleModelInteractuable), "Awake")]
        private static void MoveClockEyeInteraction(PuzzleModelInteractuable __instance)
        {
            if (__instance.name == "cardCollider")
            {
                __instance.transform.localScale = new Vector3(0.31f, 0.3f, 0.37f);
            }
        }


        // The death screen uses a different canvas than the normak menus so set it to worldspace and position it
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ItemOptionMenuButtonBehaviour), "Start")]
        private static void SetDeathScreenCanvas(ItemOptionMenuButtonBehaviour __instance)
        {
            if (__instance.name != "MenuButton (1)" && __instance.transform.root.name != "Canvas")
                return;

            Transform deathScreen = __instance.transform.root;
            deathScreen.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            deathScreen.localScale = new Vector3(0.006f, 0.006f, 0.006f);
            deathScreen.position = new Vector3(0, 1, 6);
            deathScreen.rotation = Quaternion.identity;

            camRoot.transform.position = Vector3.zero;
            camRoot.transform.rotation = Quaternion.identity;
        }






        [HarmonyPostfix]
        [HarmonyPatch(typeof(ViewOptionsMenu), "Start")]
        private static void InjectVR(ViewOptionsMenu __instance)
        {
            if (!vrStarted && GameDB.instance != null)
            {
                if (camRoot == null)
                {
                    vrHandlerInstance = GameDB.instance.gameObject.AddComponent<VRHandler>();
                    camRoot = new GameObject("camRoot");
                    camHolder = new GameObject("camHolder");
                    camHolder.transform.parent = camRoot.transform;
                    vrCamera = new GameObject("VRCamera").AddComponent<Camera>();

                    vrCamera.renderingPath = RenderingPath.DeferredShading;
                    vrCamera.transform.parent = camHolder.transform;
                    vrCamera.nearClipPlane = 0.001f;
                    vrCamera.clearFlags = CameraClearFlags.SolidColor;
                    vrCamera.backgroundColor = Color.black;
                    vrCamera.cullingMask = Camera.main.cullingMask;
                    vrCamera.gameObject.AddComponent<SteamVR_TrackedObject>();
                    UnityEngine.Object.DontDestroyOnLoad(camRoot);
                    headsetPos = vrCamera.transform.localPosition;
                    // Add AudioListener here so audio is based on the headsets position and rotation
                    vrCamera.gameObject.AddComponent<AudioListener>();

                }
                else
                {
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
                vrStarted = true;

            }
        }


        // Sets up the main menu screen to select the first available button
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ExitGameWaringController), "ButtonPressed")]
        private static void ResetCanvasOnReturnToMenu(ExitGameWaringController __instance, bool exitButton)
        {
            if (exitButton)
            {
                // Change vrStarted to false so it will fix the main menus position and everything when returning to menu
                vrStarted = false;
                inCinematic = false;
                // Delete the in-game menu manager as its regenerated when loading a save or starting a new game
                if (camRoot.transform.childCount >= 2)
                    UnityEngine.Object.Destroy(camRoot.transform.GetChild(1).gameObject);
            }
        }



        // Normally when aiming using the left joystick just rotates the player left or right so disable that
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerSM_Aim), "ManageInput")]
        public static bool DisableAimingModeRotation(PlayerSM_Aim __instance, UserInputManager.UserAction action, object payload)
        {

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

            return false;

        }

        // Based on the games original code, used to set movement speed then calls on SetRigidBodySpeed which is for movement
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerMovement), "Movement")]
        private static bool SetVRSpeedAndMoveAnim(PlayerMovement __instance, Vector2 input, bool dpad = false)
        {
            Vector2 vector = input;
            bool animatorParams = false;

            if (dpad)
            {
                return true;
            }
            // Else if joystick is being used and they're not in aiming mode (the left trigger isn't pulled
            else if (SteamVR_Actions._default.LeftTrigger.axis < 0.7)
            {
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
            else
                __instance.speed = 0;
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
            }
            else if (Mathf.Abs(camRoot.transform.eulerAngles.y - __instance.transform.rotation.eulerAngles.y) > 45)
                camRoot.transform.rotation = Quaternion.Lerp(camRoot.transform.rotation, __instance.transform.rotation, 8 * Time.deltaTime);

            // Calculate the rotation amount based on the joystick rotation
            float joystickRotAmount = SteamVR_Actions._default.RightJoystick.axis.x * rotationSpeed * Time.deltaTime;
            rotationAmount += joystickRotAmount;
            camRoot.transform.Rotate(0, joystickRotAmount, 0);
            // Apply the rotation to the __instance variable
            __instance.transform.Rotate(0f, rotationAmount, 0f);




            return false;


        }

        // Patch of the original code to allow horizontal move
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerMovement), "SetRigidbodySpeed")]
        public static bool AllowVRMovement(PlayerMovement __instance, float targetSpeed)
        {
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
                headsetPos.x += camDistanceFromBody.x / 1.5f * Time.deltaTime;
                if (__instance.currentSpeed == 0)
                    __instance.currentSpeed += 0.5f;
            }
            if (camDistanceFromBody.z > 0.1f || camDistanceFromBody.z <= -0.05f)
            {
                input.z += camDistanceFromBody.z * 2f;
                headsetPos.z += camDistanceFromBody.z / 1.5f * Time.deltaTime;
                if (__instance.currentSpeed == 0)
                {
                    __instance.currentSpeed += 0.5f;
                    if (camDistanceFromBody.z < -0.1f)
                        __instance.currentSpeed += 0.5f;
                }

            }
            Vector3 movement = __instance.transform.right * input.x + __instance.transform.forward * input.z; // Calculate movement vector


            __instance.m_rigidbody.velocity = new Vector3(movement.x * __instance.currentSpeed, __instance.m_rigidbody.velocity.y, movement.z * __instance.currentSpeed);
            if ((double)__instance.m_rigidbody.velocity.sqrMagnitude < 0.01)
            {
                __instance.m_rigidbody.velocity = Vector3.zero;
            }
            return false;
        }



        // Need to hide all the head meshes
        [HarmonyPostfix]
        [HarmonyPatch(typeof(BloodBehaviour), "Start")]
        public static void DisableHeadMeshes(BloodBehaviour __instance)
        {
            headsetPos = vrCamera.transform.localPosition;
            int[] childrenMeshesToDisable = { 0, 9, 14, 15, 16, 17, 18, 20, 23, 27, 28 };
            if (childrenMeshesToDisable[childrenMeshesToDisable.Length - 1] + 1 != __instance.transform.childCount)
                return;
            for (int i = 0; i < childrenMeshesToDisable.Length; i++)
            {
                __instance.transform.GetChild(childrenMeshesToDisable[i]).GetComponent<SkinnedMeshRenderer>().enabled = false;
            }

        }

        // When aiming the player rotates to nearby enemies so disabe that
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerSM_Aim), "TrackEnemy")]
        public static bool DisableCamTrackingToEnemy(PlayerSM_Aim __instance)
        {
            return false;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerSM_Aim), "RotateTowardsEnemy")]
        public static bool DisableRotateCamToEnemy(PlayerSM_Aim __instance)
        {
            return false;
        }
    }
}
