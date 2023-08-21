using UnityEngine;
using Valve.VR;

namespace Flashlight.code
{
    public class BehindWallCameraController : MonoBehaviour
    {
        public float scrollModifier = 0.1f;
        public bool useScroll;
        public bool useAlternativeClipping = true;
        public GameObject collidableClippingPlanes;

        public float visibleDistance = 7;
        private Vector2 nearFar;
        private const float START_NEAR = 0.3f;

        public Camera behindWallCamera;
        public float speed = 12;
        public SteamVR_Action_Boolean cameraMoveActionForward;
        public SteamVR_Action_Boolean cameraMoveActionBackward;

        private bool moveForward;
        private bool moveBackward;
        private GameObject followObject;
        private GameObject projCamNearPlane;
        private GameObject projCamFarPlane;

        // Start is called before the first frame update
        void Start()
        {
            projCamNearPlane = collidableClippingPlanes.transform.Find("ProjCamNearCube").gameObject;
            projCamFarPlane = collidableClippingPlanes.transform.Find("ProjCamFarCube").gameObject;
        
            nearFar = new Vector2(START_NEAR, START_NEAR + visibleDistance);
            UpdateClippingPlanes();

            cameraMoveActionBackward.AddOnChangeListener(OnBackward, SteamVR_Input_Sources.RightHand);
            cameraMoveActionForward.AddOnChangeListener(OnForward, SteamVR_Input_Sources.RightHand);
        }

        private void OnForward(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
        {
            moveForward = newState;
        }

        private void OnBackward(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
        {
            moveBackward = newState;
        }

        private void UpdateClippingPlanes()
        {
            if(followObject != null)
            {
                Vector3 centerOfGameObject = followObject.transform.position;
                float distCamGameObj = (centerOfGameObject - behindWallCamera.transform.position).magnitude;
                nearFar.x = distCamGameObj - visibleDistance / 2;
                nearFar.y = distCamGameObj + visibleDistance / 2;
                if (nearFar.x < 0)
                {
                    nearFar = new Vector2(0, visibleDistance);
                }
            }
            behindWallCamera.nearClipPlane = nearFar.x;
            behindWallCamera.farClipPlane = nearFar.y;
        }



        public void SetGameObjectToFollow(GameObject objectToFollow)
        {
            followObject = objectToFollow;
        }

        // Update is called once per frame
        void Update()
        {
            float add = 0;

            Debug.Log("CameraMoveActionForward.state = " + cameraMoveActionForward.state);
            Debug.Log("CameraMoveActionBackward.state = " + cameraMoveActionBackward.state);


            if (useScroll)
            {
                add = Input.GetAxis("Mouse ScrollWheel") * scrollModifier;
                Debug.Log(add);
            }
            else { 
                if (moveForward)
                {
                    add = 1f;
                }else if (moveBackward)
                {
                    add = -1f;
                }
            }
            

            //print(touchPad.y);

            Vector3 direction = new Vector3(0f,0f,add);
            Vector3 movement = Time.deltaTime * speed * direction;

            //print("Local Position: " + camera.transform.localPosition);
            //print("Movement: " + movement);

            if (nearFar.x + movement.z >= START_NEAR)
            {
                nearFar += new Vector2(movement.z, movement.z);
                UpdateClippingPlanes();
                if (useAlternativeClipping)
                {
                    UpdateGameObjects();
                }
            }
        }

        private void UpdateGameObjects()
        {
            var transform1 = behindWallCamera.transform;
            var forward = transform1.forward;
            var up = transform1.up;
            projCamNearPlane.transform.rotation = Quaternion.LookRotation(forward, up);
            projCamFarPlane.transform.rotation = Quaternion.LookRotation(forward, up);

            var position = behindWallCamera.transform.position;
            projCamNearPlane.transform.position = position + forward * behindWallCamera.nearClipPlane;
            projCamFarPlane.transform.position = position + forward * behindWallCamera.farClipPlane;

            var nearHalfSize = behindWallCamera.nearClipPlane * Mathf.Tan(Mathf.Deg2Rad * behindWallCamera.fieldOfView / 2) * 10;
            var farHalfSize = behindWallCamera.farClipPlane * Mathf.Tan(Mathf.Deg2Rad * behindWallCamera.fieldOfView / 2) * 10;

            projCamFarPlane.transform.localScale = new Vector3(farHalfSize * 2, farHalfSize * 2, 2) ;
            projCamNearPlane.transform.localScale = new Vector3(nearHalfSize * 2, nearHalfSize * 2, 2);
        }

    }
}