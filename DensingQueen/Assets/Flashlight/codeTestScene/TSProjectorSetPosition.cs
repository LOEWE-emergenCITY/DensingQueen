using UnityEngine;

namespace Flashlight.codeTestScene
{
    public class TSProjectorSetPosition : MonoBehaviour
    {
        public GameObject projectorCam;
        // Update is called once per frame
        private void Update()
        {
            transform.SetPositionAndRotation(projectorCam.transform.position, projectorCam.transform.rotation); 
        }
    }
}
