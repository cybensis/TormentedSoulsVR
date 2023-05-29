using Rewired.Data.Mapping;
using System.Collections.Generic;
using System.Linq;
using Rewired;
using Rewired.Data;
using static UnityEngine.UIElements.UIR.BestFitAllocator;

namespace TormentedSoulsVR
{
    internal static class RewiredAddons
    {

        internal static CustomController CreateRewiredController()
        {
            HardwareControllerMap_Game hcMap = new HardwareControllerMap_Game(
                "VRControllers",
                new ControllerElementIdentifier[]
                {
                    // The ID values here are from Controllers
                    new ControllerElementIdentifier(Controllers.LeftJoyStickHor, "MoveX", "MoveXPos", "MoveXNeg", ControllerElementType.Axis, true),
                    new ControllerElementIdentifier(Controllers.LeftJoyStickVert, "MoveY", "MoveYPos", "MoveYNeg", ControllerElementType.Axis, true),
                    new ControllerElementIdentifier(Controllers.RightJoyStickHor, "MoveCameraX", "MoveCameraXPos", "MoveCameraXNeg", ControllerElementType.Axis, true),
                    new ControllerElementIdentifier(Controllers.RightJoyStickVert, "MoveCameraY", "MoveCameraYPos", "MoveCameraYNeg", ControllerElementType.Axis, true),
                    new ControllerElementIdentifier(Controllers.ButtonA, "ButtonA", "", "", ControllerElementType.Button, true),
                    new ControllerElementIdentifier(Controllers.ButtonB, "ButtonB", "", "", ControllerElementType.Button, true),
                    new ControllerElementIdentifier(Controllers.ButtonX, "ButtonX", "", "", ControllerElementType.Button, true),
                    new ControllerElementIdentifier(Controllers.ButtonY, "ButtonY", "", "", ControllerElementType.Button, true),
                    new ControllerElementIdentifier(Controllers.LeftTrigger, "LeftTrigger", "", "", ControllerElementType.Axis, true),
                    new ControllerElementIdentifier(Controllers.RightTrigger, "RightTrigger", "", "", ControllerElementType.Axis, true),
                    new ControllerElementIdentifier(Controllers.LeftGrip, "LeftGrip", "", "", ControllerElementType.Button, true),
                    new ControllerElementIdentifier(Controllers.ClickLeftJoystick, "ClickLeftJoystick", "", "", ControllerElementType.Button, true),
                    new ControllerElementIdentifier(Controllers.ClickRightJoystick, "ClickRightJoystick", "", "", ControllerElementType.Button, true),
                    new ControllerElementIdentifier(Controllers.RightGrip, "RightGrip", "", "", ControllerElementType.Button, true),
                    new ControllerElementIdentifier(Controllers.NorthDPAD, "Left Grip + Y", "", "", ControllerElementType.Button, true),
                    new ControllerElementIdentifier(Controllers.EastDPAD, "Left Grip + B", "", "", ControllerElementType.Button, true),
                    new ControllerElementIdentifier(Controllers.SouthDPAD, "Left Grip + A", "", "", ControllerElementType.Button, true),
                    new ControllerElementIdentifier(Controllers.WestDPAD, "Left Grip + X", "", "", ControllerElementType.Button, true),
                    new ControllerElementIdentifier(Controllers.Back, "Back", "", "", ControllerElementType.Button, true),
                    new ControllerElementIdentifier(Controllers.RightJoyStickUp, "RightJoystickUp", "", "", ControllerElementType.Button, true),
                    new ControllerElementIdentifier(Controllers.Start, "Start", "", "", ControllerElementType.Button, true),
                    new ControllerElementIdentifier(Controllers.RightJoyStickDown, "RightJoyStickDown", "", "", ControllerElementType.Button, true),

                },
           
                new int[] { },
                new int[] { },
                new AxisCalibrationData[]
                {
                    new AxisCalibrationData(true, 0.1f, 0, -1, 1, false, true),
                    new AxisCalibrationData(true, 0.1f, 0, -1, 1, false, true),
                    new AxisCalibrationData(true, 0.1f, 0, -1, 1, false, true),
                    new AxisCalibrationData(true, 0.1f, 0, -1, 1, false, true),
                    new AxisCalibrationData(true, 0.1f, 0, 0, 1, false, true), //analog trigger
                    new AxisCalibrationData(true, 0.1f, 0, 0, 1, false, true) //analog trigger
                },
                new AxisRange[]
                {
                    AxisRange.Full,
                    AxisRange.Full,
                    AxisRange.Full,
                    AxisRange.Full,
                    AxisRange.Positive, //analog trigger
                    AxisRange.Positive //analog trigger
                },
                new HardwareAxisInfo[]
                {
                    new HardwareAxisInfo(AxisCoordinateMode.Absolute, false, 0f, SpecialAxisType.None),
                    new HardwareAxisInfo(AxisCoordinateMode.Absolute, false, 0f, SpecialAxisType.None),
                    new HardwareAxisInfo(AxisCoordinateMode.Absolute, false,  0f, SpecialAxisType.None),
                    new HardwareAxisInfo(AxisCoordinateMode.Absolute, false, 0f, SpecialAxisType.None),
                    new HardwareAxisInfo(AxisCoordinateMode.Absolute, false, 0f, SpecialAxisType.None),  //analog trigger
                    new HardwareAxisInfo(AxisCoordinateMode.Absolute, false, 0f, SpecialAxisType.None)  //analog trigger
                },
                new HardwareButtonInfo[] { },
                null
            );

            ReInput.UserData.AddCustomController();
            CustomController_Editor newController = ReInput.UserData.customControllers.Last();
            newController.name = "VRControllers";
            foreach (ControllerElementIdentifier element in hcMap.elementIdentifiers.Values)
            {
                if (element.elementType == ControllerElementType.Axis)
                {
                    newController.AddAxis();
                    newController.elementIdentifiers.RemoveAt(newController.elementIdentifiers.Count - 1);
                    newController.elementIdentifiers.Add(element);
                    CustomController_Editor.Axis newAxis = newController.axes.Last();
                    newAxis.name = element.name;
                    newAxis.elementIdentifierId = element.id;
                    newAxis.deadZone = hcMap.hwAxisCalibrationData[newController.axisCount - 1].deadZone;
                    newAxis.zero = 0;
                    newAxis.min = hcMap.hwAxisCalibrationData[newController.axisCount - 1].min;
                    newAxis.max = hcMap.hwAxisCalibrationData[newController.axisCount - 1].max;
                    newAxis.invert = hcMap.hwAxisCalibrationData[newController.axisCount - 1].invert;
                    newAxis.axisInfo = hcMap.hwAxisInfo[newController.axisCount - 1];
                    newAxis.range = hcMap.hwAxisRanges[newController.axisCount - 1];
                }
                else if (element.elementType == ControllerElementType.Button)
                {
                    newController.AddButton();
                    newController.elementIdentifiers.RemoveAt(newController.elementIdentifiers.Count - 1);
                    newController.elementIdentifiers.Add(element);
                    CustomController_Editor.Button newButton = newController.buttons.Last();
                    newButton.name = element.name;
                    newButton.elementIdentifierId = element.id;
                }
            }

            CustomController customController = ReInput.controllers.CreateCustomController(newController.id);

            customController.useUpdateCallbacks = false;

            return customController;
        }
        internal static CustomControllerMap CreateGameplayMap(int controllerID) {
            List<ActionElementMap> controllerMap = new List<ActionElementMap>() {
                new ActionElementMap(VerticalDPADID, ControllerElementType.Button, Controllers.NorthDPAD, Pole.Positive, AxisRange.Positive, false),
                new ActionElementMap(VerticalDPADID, ControllerElementType.Button, Controllers.SouthDPAD, Pole.Positive, AxisRange.Positive, false),
                new ActionElementMap(HorizontalDPADID, ControllerElementType.Button, Controllers.WestDPAD, Pole.Positive, AxisRange.Positive, false),
                new ActionElementMap(HorizontalDPADID, ControllerElementType.Button, Controllers.EastDPAD, Pole.Positive, AxisRange.Positive, false),
                new ActionElementMap(OpenDebugMenuAID, ControllerElementType.Button, Controllers.NorthDPAD, Pole.Positive, AxisRange.Positive, false),
                new ActionElementMap(AddItemsButtonAID, ControllerElementType.Button, Controllers.SouthDPAD, Pole.Positive, AxisRange.Positive, false),
                new ActionElementMap(ConfirmID , ControllerElementType.Button, Controllers.ButtonA , Pole.Positive, AxisRange.Positive, false), 
                new ActionElementMap(CancelID , ControllerElementType.Button, Controllers.ButtonB, Pole.Positive, AxisRange.Positive, false),
                new ActionElementMap(AimID , ControllerElementType.Button, Controllers.RightGrip, Pole.Positive, AxisRange.Positive, false),
                new ActionElementMap(ShootID, ControllerElementType.Button, Controllers.ButtonA , Pole.Positive, AxisRange.Positive, false),
                new ActionElementMap(RunID, ControllerElementType.Button, Controllers.ClickLeftJoystick, Pole.Positive, AxisRange.Positive, false), 
                new ActionElementMap(MenuID, ControllerElementType.Button, Controllers.ButtonY, Pole.Positive, AxisRange.Positive, false),
                new ActionElementMap(ShowInputDebugID , ControllerElementType.Button, Controllers.Back, Pole.Positive, AxisRange.Positive, false),
                new ActionElementMap(ReloadID , ControllerElementType.Button, Controllers.ButtonB, Pole.Positive, AxisRange.Positive, false),
                new ActionElementMap(OptionsID , ControllerElementType.Button, Controllers.Start, Pole.Positive, AxisRange.Positive, false),
                new ActionElementMap(LanternSwitchID , ControllerElementType.Button, Controllers.ClickRightJoystick, Pole.Positive, AxisRange.Positive, false),
                new ActionElementMap(SkipLeftID, ControllerElementType.Button, Controllers.LeftGrip, Pole.Positive, AxisRange.Positive, false),
                new ActionElementMap(SkipRightID, ControllerElementType.Button, Controllers.RightGrip, Pole.Positive, AxisRange.Positive, false),
                new ActionElementMap(HorizontalAxisID, ControllerElementType.Button, Controllers.LeftJoyStickHor, Pole.Positive, AxisRange.Positive, false),
                new ActionElementMap(VerticalAxisID, ControllerElementType.Button, Controllers.LeftJoyStickVert, Pole.Positive, AxisRange.Positive, false),
                new ActionElementMap(AimID, ControllerElementType.Button, Controllers.LeftTrigger, Pole.Positive, AxisRange.Positive, false),
                new ActionElementMap(ShootID, ControllerElementType.Button, Controllers.RightTrigger, Pole.Positive, AxisRange.Positive, false),
                new ActionElementMap(RightHorizontalAxisID, ControllerElementType.Button, Controllers.RightJoyStickHor, Pole.Positive, AxisRange.Positive, false),
                new ActionElementMap(RightVerticalAxisID, ControllerElementType.Button, Controllers.RightJoyStickVert, Pole.Positive, AxisRange.Positive, false),
                new ActionElementMap(TestingModifierID, ControllerElementType.Button, Controllers.LeftTrigger, Pole.Positive, AxisRange.Positive, false)
            };
            return CreateCustomMap("VRControls", 0, controllerID, controllerMap);
        }


        



        private static CustomControllerMap CreateCustomMap(string mapName, int categoryId, int controllerId, List<ActionElementMap> actionElementMaps)
        {
            //ReInput.UserData.CreateCustomControllerMap(categoryId, controllerId, 0);
            ReInput.UserData.CreateCustomControllerMap(categoryId, controllerId, 0);
            ControllerMap_Editor newMap = ReInput.UserData.customControllerMaps.Last();

            newMap.name = mapName;

            foreach (ActionElementMap elementMap in actionElementMaps)
            {
                newMap.AddActionElementMap();
                ActionElementMap newElementMap = newMap.GetActionElementMap(newMap.ActionElementMaps.Count() - 1);
                newElementMap.actionId = elementMap.actionId;
                newElementMap.elementType = elementMap.elementType;
                newElementMap.elementIdentifierId = elementMap.elementIdentifierId;
                newElementMap.axisContribution = elementMap.axisContribution;
                if (elementMap.elementType == ControllerElementType.Axis)
                    newElementMap.axisRange = elementMap.axisRange;
                newElementMap.invert = elementMap.invert;
            }
            return ReInput.UserData.jtJLkGhxZsBzutVJygqxKWaYeEx(categoryId, controllerId, 0);
        }
        // Rewired ActionID Mapping
        public static int ConfirmID = 0;
        public static int CancelID = 1;
        public static int AimID = 2;
        public static int RunID = 4;
        public static int HorizontalAxisID = 5;
        public static int VerticalAxisID = 6;
        public static int HorizontalDPADID = 7;
        public static int VerticalDPADID = 8;
        public static int MenuID = 11;
        public static int ShowInputDebugID = 12;
        public static int ReloadID = 13;
        public static int OptionsID = 14;
        public static int LanternSwitchID = 15;
        public static int RightHorizontalAxisID = 16;
        public static int RightVerticalAxisID = 17;
        public static int SkipLeftID = 18;
        public static int SkipRightID = 19;
        public static int ShootID = 21;
        public static int AddItemsButtonAID = 22;
        public static int TestingModifierID = 23;
        public static int OpenDebugMenuAID = 24;

   


    }
}
