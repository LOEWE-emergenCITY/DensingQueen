using UnityEngine;

namespace Flashlight.codeTestScene
{
    public class TSCameraLookAtProjection : MonoBehaviour
    {
        public Camera projectorCam;
        void Update()
        {
            var layerMask = 1 << 13;
            layerMask = ~layerMask; 

            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out var hit, Mathf.Infinity, layerMask))
            {
                var flashlightTransform = transform;
                var currentHitPosition = flashlightTransform.position + flashlightTransform.forward * hit.distance;
                var directionToCollision = currentHitPosition - projectorCam.transform.position;
                var rotationCam = Quaternion.LookRotation(directionToCollision, flashlightTransform.up);

                if (hit.collider.gameObject.layer == 12) // walls are on layer "real" = 12th layer
                {
                    projectorCam.transform.SetPositionAndRotation(projectorCam.transform.position, rotationCam);
                }
            }
        }
    }
}