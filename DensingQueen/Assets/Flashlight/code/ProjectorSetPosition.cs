using UnityEngine;

namespace Flashlight.code
{
    public class ProjectorSetPosition : MonoBehaviour
    {
        public GameObject projectorCam;
        // Update is called once per frame
        private void Update()
        {
            transform.SetPositionAndRotation(projectorCam.transform.position, projectorCam.transform.rotation); 
        }
    }
}
