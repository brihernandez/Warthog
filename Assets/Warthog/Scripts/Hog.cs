using UnityEngine;
using System.Collections.Generic;

namespace Warthog
{
    [RequireComponent(typeof(Rigidbody))]
    public class Hog : MonoBehaviour
    {
        public OrbitCamera steeringSource = null;

        [SerializeField] [Range(-1, 1)] private float steering = 0f;
        [SerializeField] [Range(-1, 1)] private float throttle = 0f;

        [SerializeField] [Range(-1, 1)] private float autoSteerPower = 3f;
        [SerializeField] [Range(-1, 1)] private float centerOfMassOffset = -.5f;

        private List<Wheel> wheels = new List<Wheel>();
        private Rigidbody rigid = null;

        private void Awake()
        {
            wheels = new List<Wheel>(GetComponentsInChildren<Wheel>());
            rigid = GetComponent<Rigidbody>();

            var centerOfMass = rigid.centerOfMass;
            centerOfMass.y += centerOfMassOffset;
            rigid.centerOfMass = centerOfMass;
        }

        private void Update()
        {
            steering = GetSteering(steeringSource, autoSteerPower);
            throttle = Input.GetAxis("Vertical");

            foreach (var wheel in wheels)
            {
                wheel.SteerInput = steering;
                wheel.Throttle = throttle;
            }
        }

        private float GetSteering(OrbitCamera source, float autoPower)
        {
            float steering = 0f;

            if (source == null)
                steering = Input.GetAxis("Horizontal");
            else
            {
                Vector3 localLook = transform.InverseTransformDirection(source.transform.forward);

                if (localLook.z < 0f)
                    localLook.x = Mathf.Sign(localLook.x);

                steering = localLook.x * autoPower;
                steering = Mathf.Clamp(steering, -1f, 1f);
            }

            return steering;
        }
    }
}
