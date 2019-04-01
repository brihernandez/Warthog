using UnityEngine;

namespace Warthog
{
    public class OrbitCamera : MonoBehaviour
    {
        [SerializeField] private Transform followTarget = null;
        [SerializeField] private bool useFixed = false;
        [SerializeField] private float followSpeed = 10f;
        [SerializeField] private float mouseSensitivity = 10f;

        private void Awake()
        {
            transform.parent = null;
        }

        private void Update()
        {
            if (!useFixed)
                MoveRig();

            float horizontal = Input.GetAxis("Mouse X") * mouseSensitivity;
            transform.Rotate(Vector3.up * horizontal);
        }

        private void FixedUpdate()
        {
            if (useFixed)
                MoveRig();
        }

        private void MoveRig()
        {
            transform.position = Vector3.Lerp(transform.position, followTarget.position, followSpeed * Time.deltaTime);
        }
    }
}
