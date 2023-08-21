using UnityEngine;
using Valve.VR;

namespace Flashlight.code
{
    public class FlashlightOnOff : MonoBehaviour
    {
        // a reference to the action
        public SteamVR_Action_Boolean flashlightOn;

        private GameObject flashlight;

        // Start is called before the first frame update
        void Start()
        {
            flashlight = transform.Find("ProjectorLogic").gameObject;
            flashlight.SetActive(false);
            flashlightOn.AddOnStateDownListener(OnTriggerDown, SteamVR_Input_Sources.RightHand);
        }

        private void OnTriggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            ToggleActive();
        }

        private void ToggleActive()
        {
            if (!GetComponent<FollowObject>().IsInSwitchMode)
            {
                flashlight.SetActive(!flashlight.activeSelf);
            }
        
        }
    }
}
