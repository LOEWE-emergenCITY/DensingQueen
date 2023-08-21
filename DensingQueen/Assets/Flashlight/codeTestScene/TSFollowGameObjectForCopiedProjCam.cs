using UnityEngine;

namespace Flashlight.codeTestScene
{
    public class TSFollowGameObjectForCopiedProjCam : MonoBehaviour
    {
        public GameObject vrCam;
        private Vector3 center;

        // Update is called once per frame
        private void Update()
        {
            //Debug.DrawLine(VRCam.transform.position, center, Color.red);
            var t = transform;
            var transformParent = t.parent;
            center = transformParent.position;
            var position = vrCam.transform.position;
            var directionToProjection = center - position;
            var rotationCam = Quaternion.LookRotation(directionToProjection, t.up);
            transform.SetPositionAndRotation(position, rotationCam);
        }
    }
}
