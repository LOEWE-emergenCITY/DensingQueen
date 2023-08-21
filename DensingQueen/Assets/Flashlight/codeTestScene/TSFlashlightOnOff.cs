using UnityEngine;

namespace Flashlight.codeTestScene
{
    public class TSFlashlightOnOff : MonoBehaviour
    {
        private GameObject flashlight;

        void Start()
        {
            flashlight = transform.Find("ProjectorLogic").gameObject;
            flashlight.SetActive(false);
        }


        private void Update()
        {
            if (GetComponent<TSFollowObject>().IsInSwitchMode)
            {
                flashlight.SetActive(true);
            }

            else
            {
                    if (Input.GetMouseButtonDown(0)) // left mouse click
                    {
                        flashlight.SetActive(!flashlight.activeSelf);
                    }
            }
        }
    }
}
