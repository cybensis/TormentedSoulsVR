﻿using Rewired;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Valve.VR;


namespace TormentedSoulsVR
{
    public static class Controllers
    {
        public static bool ControllersAlreadyInit = false;

        public static CustomController vrControllers;
        private static CustomControllerMap vrControlsMap;


        private static bool hasRecentered;
        private static bool initializedMainPlayer;
        private static bool initializedLocalUser;

        private static BaseInput[] inputs;
        private static List<BaseInput> modInputs = new List<BaseInput>();

        internal static int leftJoystickID { get; private set; }
        internal static int rightJoystickID { get; private set; }

        internal static int ControllerID => vrControllers.id;

        internal static void Init()
        {
            if (!ControllersAlreadyInit)
            {
                ReInput.InputSourceUpdateEvent += UpdateVRInputs;
                SetupControllerInputs();
                ControllersAlreadyInit = true;
            }
        }


        public static void ResetControllerVars() { 
            initializedMainPlayer = false;
            ControllersAlreadyInit = false;
        }



        private static void SetupControllerInputs()
        {
            vrControllers = RewiredAddons.CreateRewiredController();

            vrControlsMap = RewiredAddons.CreateGameplayMap(vrControllers.id);

            inputs = new BaseInput[]
            {
                    new VectorInput(SteamVR_Actions._default.LeftJoystick, LeftJoyStickHor, LeftJoyStickVert),
                    new VectorInput(SteamVR_Actions._default.RightJoystick, RightJoyStickHor, RightJoyStickVert),
                    new ButtonInput(SteamVR_Actions._default.ButtonA, ButtonA),
                    new ButtonInput(SteamVR_Actions._default.ButtonB, ButtonB),
                    new ButtonInput(SteamVR_Actions._default.ButtonX, ButtonX),
                    new ButtonInput(SteamVR_Actions._default.ButtonY, ButtonY),
                    new AxisInput(SteamVR_Actions._default.LeftTrigger, LeftTrigger),
                    new AxisInput(SteamVR_Actions._default.RightTrigger, RightTrigger),
                    new ButtonInput(SteamVR_Actions._default.LeftGrip, LeftGrip),
                    new ButtonInput(SteamVR_Actions._default.ClickLeftJoystick, ClickLeftJoystick),
                    new ButtonInput(SteamVR_Actions._default.ClickRightJoystick, ClickRightJoystick),
                    new ButtonInput(SteamVR_Actions._default.RightGrip, RightGrip),
                    new ButtonInput(SteamVR_Actions._default.NorthDPAD, NorthDPAD),
                    new ButtonInput(SteamVR_Actions._default.EastDPAD, EastDPAD),
                    new ButtonInput(SteamVR_Actions._default.SouthDPAD, SouthDPAD),
                    new ButtonInput(SteamVR_Actions._default.WestDPAD, WestDPAD),
                    new ButtonInput(SteamVR_Actions._default.Back, Back),
                    new VectorButton(SteamVR_Actions._default.RightJoystick, RightJoyStickUp),
                    new VectorButton(SteamVR_Actions._default.RightJoystick, RightJoyStickDown),
                    new ButtonInput(SteamVR_Actions._default.Start, Start)

                    //new ButtonInput(SteamVR_Actions.default_nexttarget, 14) // right joystick in DPAD mode? pressed east
            };
        }

        public static void Update()
        {
            if (!initializedMainPlayer)
            {
                Player p = ReInput.players.AllPlayers[1];
                if (AddVRController(p))
                {
                    initializedMainPlayer = true;
                    Debug.Write("VRController successfully added");
                }
            }

        }

        internal static bool AddVRController(Player inputPlayer)
        {
            if (!inputPlayer.controllers.ContainsController(vrControllers))
            {
                inputPlayer.controllers.AddController(vrControllers, false);
                vrControllers.enabled = true;
            }

            if (inputPlayer.controllers.maps.GetAllMaps(ControllerType.Custom).ToList().Count < 9)
            {
                if (inputPlayer.controllers.maps.GetMap(ControllerType.Custom, vrControllers.id, VRControlsMapID, 0) == null)
                    inputPlayer.controllers.maps.AddMap(vrControllers, vrControlsMap);
                if (!vrControlsMap.enabled)
                    vrControlsMap.enabled = true;
            }

            return inputPlayer.controllers.ContainsController(vrControllers) && inputPlayer.controllers.maps.GetAllMaps(ControllerType.Custom).ToList().Count >= 1;
        }

        private static void UpdateVRInputs()
        {
            
            foreach (BaseInput input in inputs)
            {
                input.UpdateValues(vrControllers);
            }

            foreach (BaseInput input in modInputs)
            {
                input.UpdateValues(vrControllers);
            }
        }
 

        // Controller ID's
        public static int LeftJoyStickHor = 0;
        public static int LeftJoyStickVert = 1;
        public static int RightJoyStickHor = 2;
        public static int RightJoyStickVert = 3;
        public static int ButtonA = 4;
        public static int ButtonB = 5;
        public static int ButtonX = 6;
        public static int ButtonY = 7;
        public static int LeftTrigger = 8;
        public static int RightTrigger = 9;
        public static int ClickRightJoystick = 10;
        public static int ClickLeftJoystick = 11;
        public static int LeftGrip = 12;
        public static int RightGrip = 13;
        public static int NorthDPAD = 14;
        public static int EastDPAD = 15;
        public static int SouthDPAD = 16;
        public static int WestDPAD = 17;
        public static int Back = 18;
        public static int RightJoyStickUp = 19;
        public static int RightJoyStickDown = 20;
        public static int Start = 21;


        // ReWired custom controller category ID's
        public static int VRControlsMapID = 0;
    }
}
