using Knife.RealBlood.SimpleController;
using Knife.RealBlood;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TormentedSoulsVR.VRBody;
using UnityEngine;
using Valve.VR;
using TormentedSoulsVR.cam;

namespace TormentedSoulsVR.weapons
{
    internal class VRCrowbar : MonoBehaviour
    {


        // To be registered as swinging, the swing needs to reach a velocity of 1.6 which is kind of slow
        // but players shouldn't have to swing fast to actualy swing (going on saints and sinners logic)
        private const float VELOCITY_THRESHOLD = 1.6f;
        // A swing at the threshold velocity needs to be maintained for 0.35f seconds for it to be a full swing
        private const float SWING_MAINTAINED_THRESHOLD = 0.35f;
        // No matter how fast the player swings, it must be maintained for atleast 0.08 seconds
        private const float SWING_MAINTAINED_MIN = 0.08f;
        // Threshold - min is what this value represents, this is the time difference allowed to be modified by reaching a velocity beyond threshold
        private const float SWING_VELOCITY_TIME_MODIFIER = SWING_MAINTAINED_THRESHOLD - SWING_MAINTAINED_MIN;
        // A velocity of 6 is pretty fast, players really shouldn't go beyond this
        private const float MAX_VELOCITY = 6f;
        // Used in the swing calculation where we divide this value by the current velocity minus the threshold velocity
        private const float MAX_VELOCITY_MINUS_THRESHOLD = MAX_VELOCITY - VELOCITY_THRESHOLD;


        // Used to tell if the velocity has exceeded the threshold
        private bool isSwinging = false;
        // Once the minimum time for a swing has been reached, the swing is fired
        private bool swingFired = false;
        // The time when the swing started
        private float swingStart = 0f;

        // Once a hit has been found and the minimum swing time has been reached, the hit is fired and this gets set
        private bool hitFired = false;

        private int currentAttackSound = 0;

        private const int TOTAL_ATTACK_SOUNDS = 3;
        private const string CROWBAR_SWING_SFX_ID = "crowbar_whoosh";
        private string[] attackSounds = {"caroline_crowbar1", "caroline_crowbar2", "caroline_crowbar3"};

        private float attackDelay = 1f;
        private float delayLength = 0f;
        private bool delayAttack = false;
        private float delayStartTime;

        private Collider colliderHit;
        private NailerPullerWeaponBehaviour weaponBehaviour;

        void Update()
        {
            if (weaponBehaviour == null)
                weaponBehaviour = transform.GetComponent<ColliderEventSender>().OnTriggerEnterAction.Target as NailerPullerWeaponBehaviour;
            if (delayAttack && Time.time - delayStartTime > delayLength)
                delayAttack = false;
            else if (delayAttack)
                return;

            float swingVelocity =  Mathf.Clamp(SteamVR_Actions._default.SkeletonRightHand.velocity.magnitude, 0, MAX_VELOCITY);

            // Trigger sound cue
            if (isSwinging && !swingFired && Time.time - swingStart >= SWING_MAINTAINED_THRESHOLD - SWING_VELOCITY_TIME_MODIFIER * ((swingVelocity - VELOCITY_THRESHOLD) / MAX_VELOCITY_MINUS_THRESHOLD))
            {
                swingFired = true;
                CamFix.player.m_sfxManager.PlaySFX(attackSounds[currentAttackSound],1,1);
                currentAttackSound = (currentAttackSound + 1) % TOTAL_ATTACK_SOUNDS;
                CamFix.player.m_sfxManager.PlaySFX(CROWBAR_SWING_SFX_ID, 1, 1);
                //weaponBehaviour.Attack();
                if (weaponBehaviour.IsTarget(colliderHit.transform) && weaponBehaviour.IsInLineOfSight(colliderHit) && weaponBehaviour.CheckIfMakesDamage(colliderHit.transform))
                {
                    hitFired = true;
                    weaponBehaviour.m_owner.GetComponentInParent<PlayerController>().RequestCameraShake();
                    weaponBehaviour.RequestJoystickRumble();
                }

            }

            if (swingVelocity >= VELOCITY_THRESHOLD && !isSwinging)
            {
                resetSwingVariables();
                swingStart = Time.time;
                isSwinging = true;
                Debug.LogError("SWING START");
            }
            else if (swingVelocity < VELOCITY_THRESHOLD && isSwinging)
            {
                if (swingFired) {
                    if (!hitFired && weaponBehaviour.IsTarget(colliderHit.transform) && weaponBehaviour.IsInLineOfSight(colliderHit) && weaponBehaviour.CheckIfMakesDamage(colliderHit.transform)) {
                        hitFired = true;
                        weaponBehaviour.m_owner.GetComponentInParent<PlayerController>().RequestCameraShake();
                        weaponBehaviour.RequestJoystickRumble();
                    }
                    SetDelay(attackDelay);
                }
                resetSwingVariables();
                Debug.LogError("SWING END");

            }


        }

        void OnTriggerEnter(Collider other)
        {
            colliderHit = other;

        }

        private void resetSwingVariables()
        {
            swingFired = false;
            hitFired = false;
            isSwinging = false;
        }

        private void SetDelay(float timeToDelay)
        {
            delayLength = timeToDelay;
            delayStartTime = Time.time;
            delayAttack = true;
        }
    }
}
