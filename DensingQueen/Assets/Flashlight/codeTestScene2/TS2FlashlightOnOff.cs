using UnityEngine;

namespace Flashlight.codeTestScene2
{
    public class TS2FlashlightOnOff : MonoBehaviour
    {
        private GameObject flashlight;

        void Start()
        {
            flashlight = transform.Find("ProjectorLogic").gameObject;
            flashlight.SetActive(false);
        }


        private void Update()
        {
            if (GetComponent<TS2FollowObject>().IsInSwitchMode)
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
