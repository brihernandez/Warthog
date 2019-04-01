using UnityEngine;

namespace Warthog
{
    [RequireComponent(typeof(WheelCollider))]
    public class Wheel : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Transform model = null;
        private WheelCollider wheel = null;

        [Header("Properties")]
        [SerializeField] private float maxAngle = 30f;
        [SerializeField] private float peakTorque = 200f;
        [SerializeField] private float brakeTorque = 200f;
        [SerializeField] private float timeToMaxTorque = 5f;
        [SerializeField] private float timeToZeroTorque = 1f;

        [Header("Input")]
        [SerializeField] [Range(-1, 1)] private float steerInput = 0f;
        [SerializeField] [Range(-1, 1)] private float throttle = 0f;

        [Header("Effects")]
        [SerializeField] ParticleSystem dustTrail = null;
        [SerializeField] ParticleSystem dirtDots = null;

        [Header("Debug")]
        [SerializeField] private float torque = 0f;

        public float SteerInput { set { steerInput = value; } }
        public float Throttle { set { throttle = value; } }

        private void Awake()
        {
            wheel = GetComponent<WheelCollider>();
        }

        // Update is called once per frame
        void Update()
        {
            if (wheel == null)
                return;

            if (model != null)
            {
                Vector3 wheelPos = Vector3.zero;
                Quaternion wheelQuat = Quaternion.identity;
                wheel.GetWorldPose(out wheelPos, out wheelQuat);
                model.position = wheelPos;
                model.rotation = wheelQuat;
            }

            wheel.steerAngle = steerInput * maxAngle;

            ApplyTorque();
            ApplyBrake();

            PlayEffects();
        }

        private void PlayEffects()
        {
            if (wheel.rpm > 5f && wheel.isGrounded)
                PlayParticleSystem(dustTrail);
            else
                StopParticleSystem(dustTrail);

            if (torque > 0f && wheel.isGrounded)
                PlayParticleSystem(dirtDots);
            else
                StopParticleSystem(dirtDots);
        }

        private void PlayParticleSystem(ParticleSystem system)
        {
            if (system.isStopped)
                system.Play();
        }

        private void StopParticleSystem(ParticleSystem system)
        {
            if (system.isPlaying)
                system.Stop();
        }

        private void ApplyBrake()
        {
            float torque = 0f;

            if (throttle < -.1f)
                torque = brakeTorque * -throttle;

            wheel.brakeTorque = torque;
        }

        private void ApplyTorque()
        {
            float targetTorque = (throttle >= 0f) ? peakTorque * throttle : 0f;

            float torqueRate = (throttle >= 0f)
                ? peakTorque / timeToMaxTorque
                : peakTorque / timeToZeroTorque;

            torque = Mathf.MoveTowards(torque, targetTorque, torqueRate * Time.deltaTime);
            wheel.motorTorque = torque;
        }
    }
}
