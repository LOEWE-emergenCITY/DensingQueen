using UnityEngine;

namespace Flashlight.codeTestScene
{
    public class TSFollowHeadForCopiedProjCam : MonoBehaviour
    {
        public GameObject vrCam;
        private Vector3 center;

        // Start is called before the first frame update
        private void Start()
        {
            var transformParent = transform.parent;
            center = transformParent.position;
        }

        // Update is called once per frame
        private void Update()
        {
            var position = vrCam.transform.position;
            var directionToProjection = center - position;
            var rotationCam = Quaternion.LookRotation(directionToProjection, transform.up);
            transform.SetPositionAndRotation(position, rotationCam);
        }
    }
}
