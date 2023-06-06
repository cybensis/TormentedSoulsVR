using TormentedSoulsVR.cam;
using UnityEngine;

namespace TormentedSoulsVR.VRBody
{
    internal class VRCrouch : MonoBehaviour
    {


        private float thighRotAmount = -120;
        private float shinRotAmount = 120;
        private float bodyYAxisChange = -0.55f;

        private Transform leftThigh;
        private Transform rightThigh;

        private Transform leftShin;
        private Transform rightShin;

        private Transform bodyRoot;

        // Base Y height that this was tested with
        private float baseYHeight = 1.65f;

        private float baseMaxCrouchAmount = 0.65f;

        // The max Y amount that the player can crouch
        private float maxCrouchAmount = 0f;
        private void Awake()
        {
            leftThigh = transform.GetChild(0);
            rightThigh = transform.GetChild(1);

            leftShin = leftThigh.GetChild(0).GetChild(0);
            rightShin = rightThigh.GetChild(0).GetChild(0);

            InitHeight();
        }
        private void Update()
        {
            // Get the difference between the initialized headset position and the current position, divide it by the maxCrouchAmount and clamp it between 0 and 1
            float crouchAmount = Mathf.Clamp((CamFix.headsetPos.y - CamFix.vrCamera.transform.localPosition.y) / maxCrouchAmount, -0.5f, 1);

            leftThigh.Rotate(crouchAmount * thighRotAmount, 0, 0);
            rightThigh.Rotate(crouchAmount * thighRotAmount, 0, 0);

            leftShin.Rotate(crouchAmount * shinRotAmount, 0, 0);
            rightShin.Rotate(crouchAmount * shinRotAmount, 0, 0);

            bodyRoot.localPosition = new Vector3(0, crouchAmount * bodyYAxisChange, 0);
        }

        public void InitHeight()
        {
            // If the player is shorter than the base height, multiply the maxCrouch amount by this division so shorter people have to crouch less than taller people
            maxCrouchAmount = baseMaxCrouchAmount * (CamFix.headsetPos.y / baseYHeight);
        }

        public void SetBodyRoot(Transform root)
        {
            bodyRoot = root;
        }
    }
}
