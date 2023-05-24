using HarmonyLib;
using Rewired;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;
using static Rewired.ComponentControls.Effects.RotateAroundAxis;

namespace TormentedSoulsVR
{
    [HarmonyPatch]
    internal class CamFix
    {
        public static GameObject camHolder;
        public static GameObject vrCamera;
        public static GameObject camRoot;

        private static bool vrStarted = false;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerController), "Update")]
        private static void FixCameraOnPlayer(PlayerController __instance)
        {
            if (camRoot == null)
            {
                camRoot = new GameObject("camRoot");
                camHolder = new GameObject("camHolder");
                vrCamera = new GameObject("VRCamera");
                vrCamera.transform.parent = camHolder.transform;
                camHolder.transform.parent = camRoot.transform;
                vrCamera.AddComponent<Camera>().nearClipPlane = 0.001f;
                vrCamera.AddComponent<SteamVR_TrackedObject>();
                UnityEngine.Object.DontDestroyOnLoad(camRoot);
            }
            Vector3 newPos = vrCamera.transform.localPosition * -1;
            newPos.y += 1.575f;
            newPos.z += 0.05f;
            camHolder.transform.localPosition = newPos;
            camRoot.transform.position = __instance.transform.position;
            camRoot.transform.rotation = __instance.transform.rotation;

        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ViewOptionsMenu), "Start")]
        private static void injeectVR(ViewOptionsMenu __instance)
        {
            if (!vrStarted && GameDB.instance != null)
            {
                GameDB.instance.gameObject.AddComponent<test>();
                vrStarted = true;
                // When returning from the game to the main menu, it deletes the controller scheme so we have to reset it here

                if (camRoot == null)
                {
                    camRoot = new GameObject("camRoot");
                    camHolder = new GameObject("camHolder");
                    vrCamera = new GameObject("VRCamera");
                    vrCamera.transform.parent = camHolder.transform;
                    camHolder.transform.parent = camRoot.transform;
                    Camera cam = vrCamera.AddComponent<Camera>();
                    cam.nearClipPlane = 0.001f;
                    cam.backgroundColor = Color.black;
                    vrCamera.AddComponent<SteamVR_TrackedObject>();
                    UnityEngine.Object.DontDestroyOnLoad(camRoot);

                }

                Canvas mainScreenCanvas = __instance.transform.parent.GetComponent<Canvas>();
                mainScreenCanvas.renderMode = RenderMode.WorldSpace;
                mainScreenCanvas.transform.localScale = new Vector3(0.002f, 0.002f, 0.002f);
                mainScreenCanvas.transform.position = new Vector3(0, 1, 4.3f);
                camHolder.transform.localPosition = vrCamera.transform.localPosition * -1;


            }
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerMovement), "Movement")]
        private static bool VRMovement(PlayerMovement __instance, Vector2 input, bool dpad = false)
        {
            if (!__instance.m_anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") && !__instance.m_anim.GetCurrentAnimatorStateInfo(0).IsName("Movement"))
                return false;

            Vector2 vector = input;
            // I think these lines set the camera angle based on the position of the player in the cameras arc
            __instance.oldCameraAngle = __instance.currentCameraAngle;
            __instance.currentCameraAngle = __instance.m_cameraTransform.eulerAngles.y;
            float num = Mathf.Abs(__instance.getAngle(Vector2.zero, __instance.inputCache) - __instance.getAngle(Vector2.zero, vector));
            float num2 = Mathf.Abs(__instance.currentCameraAngle - __instance.oldCameraAngle);
            bool animatorParams = false;
            if (num2 >= __instance.cameraChangeAngleDelta)
            {
                if (num >= __instance.stickAngleThreshold || vector == Vector2.zero)
                {
                    __instance.inputCache = vector;
                    __instance.correctionReference = 0f;
                }
                else if (num2 > 180f)
                    __instance.currentCameraAngle = Mathf.SmoothDamp(__instance.convertTo180range(__instance.oldCameraAngle), __instance.convertTo180range(__instance.currentCameraAngle), ref __instance.correctionReference, __instance.correctionTimeAfterCameraChange);
                else
                    __instance.currentCameraAngle = Mathf.SmoothDamp(__instance.oldCameraAngle, __instance.currentCameraAngle, ref __instance.correctionReference, __instance.correctionTimeAfterCameraChange);
            }
            else
            {
                __instance.inputCache = vector;
            }
            // If a scene has just started but the current scene isn' set to the current one, do that and lock the joysticks i think?
            if (__instance.CurrentScene != SceneManager.GetActiveScene().name)
            {
                if (!PlayerMovement.ReleasedStick)
                {
                    float num3 = (__instance.currentCameraAngle = __instance.transform.rotation.eulerAngles.y - Mathf.Atan2(vector.x, vector.y) * 57.29578f);
                }
                else
                {
                    __instance.currentCameraAngle = __instance.m_cameraTransform.eulerAngles.y;
                    PlayerMovement.ReleasedStick = false;
                }
                __instance.CurrentScene = SceneManager.GetActiveScene().name;
            }
            // I think this if else is for dpad vs joystick movement
            if (dpad)
            {
                //Debug.LogWarning("DPAD: " + input);
                // vector is the input, if its not zero then get the new rotation
                vector = vector.normalized;
                if (vector != Vector2.zero)
                {
                    //float num4 = (float)((input.y == 0f) ? 30 : 20) * vector.x;
                    //if (Mathf.Abs(vector.x) != 1f && input.x != 0f)
                    //    num4 = (float)(25 * Math.Sign(vector.x)) * 0.8f;

                    //// Using the calculated rotation angle above, we get the new Y rotation for the player, add it to the existing Y rot value and turn it into a quaternion
                    //Quaternion b = Quaternion.Euler(0f, num4 + __instance.transform.rotation.eulerAngles.y, 0f);
                    //// Now slerp between the current rotation and the new one just made
                    //Quaternion rotation = Quaternion.Slerp(__instance.transform.rotation, b, __instance.turnSmoothTime * Time.deltaTime * 50f);
                    //// Now we set the players rotation
                    //__instance.transform.rotation = rotation;

                    //NOTE: All of this rotation stuff can probably be scrapped since we use rightjoystick to rotate

                }
                // if they aren't moving forwards or backwards make the moving animation idle
                if (input.y == 0f && input.x == 0f)
                {
                    __instance.speed = 0f;
                    __instance.animationMove = PlayerMovement.AnimationMove.Idle;
                    animatorParams = true;
                }
                // Else if they're moving forward
                else if (input.y > 0f)
                {
                    // If the player is going up or down stairs
                    if (__instance.stairsTriggers.Count > 0)
                    {
                        // Check angle to see if they're going up or down stairs, can probably leave this whole if else code alone
                        if (Mathf.Abs(__instance.rotationAngle) < 90f)
                        {
                            __instance.animationMove = PlayerMovement.AnimationMove.StairsUp;
                            if (__instance.m_run)
                                __instance.speed = __instance.m_runSpeedStairsUp;
                            else
                                __instance.speed = __instance.m_walkSpeedStairsUp;
                            
                        }
                        else
                        {
                            __instance.animationMove = PlayerMovement. AnimationMove.StairsDown;
                            if (__instance.m_run)
                                __instance.speed = __instance.m_runSpeedStairsDown;
                            else
                                __instance.speed = __instance.m_walkSpeedStairsDown;
                        }
                    }
                    // Else if they aren't going down stairs, then they are going forward so set that animation
                    else
                    {
                        __instance.animationMove = PlayerMovement.AnimationMove.FloorForw;
                        if (__instance.m_run)
                            __instance.speed = __instance.m_runSpeed;
                        else
                            __instance.speed = __instance.m_walkSpeed;
                    }   
                }
                // If y != 0 and its < 0 then they are moving backwards
                else
                {
                    __instance.animationMove = PlayerMovement.AnimationMove.FloorBack;
                    __instance.speed = (0f - __instance.m_walkSpeed) / 2f;
                }
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
            float rotationAmount = SteamVR_Actions._default.RightJoystick.axis.x * rotationSpeed * Time.deltaTime;

            // Apply the rotation to the __instance variable
            __instance.transform.Rotate(0f, rotationAmount, 0f);
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerMovement), "SetRigidbodySpeed")]
        public static bool AllowHorizontalMovement(PlayerMovement __instance, float targetSpeed)
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
            int[] childrenMeshesToDisable = { 0, 9, 14, 15, 16, 17, 18, 20, 23, 27, 28 };
            if (childrenMeshesToDisable[childrenMeshesToDisable.Length - 1] + 1 != __instance.transform.childCount)
                return;
            for (int i = 0; i < childrenMeshesToDisable.Length; i++) {
                __instance.transform.GetChild(childrenMeshesToDisable[i]).GetComponent<SkinnedMeshRenderer>().enabled = false;
            }
            
        }


    }
}
