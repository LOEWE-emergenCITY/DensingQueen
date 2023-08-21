using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flashlight.codeTestScene
{
    public class TSMoveHand : MonoBehaviour
    {
        private float moveSpeed = 3f;
        private float turnSpeed = 60f;

        public bool UseMouse;

        public string goForward;
        public string goBackward;
        public string goLeft;
        public string goRight;
        public string goUp;

        public string rotateUp;
        public string rotateDown;
        public string rotateL;
        public string rotateR;
        public string rotateXL;
        public string rotateXR;
        public string resetPos;
        public string resetRot;

        private Vector3 worldMousePosition;
        private Vector3 initialLocalPosition;

        // Start is called before the first frame update
        void Start()
        {
            initialLocalPosition = transform.localPosition;
        }

        // Update is called once per frame
        void Update()
        {
            // Translations
            if (Input.GetKey(goForward))
            {
                transform.Translate(0, 0, moveSpeed * Time.deltaTime);
            }

            if (Input.GetKey(goBackward))
            {
                transform.Translate(0, 0, -moveSpeed * Time.deltaTime);
            }

            if (Input.GetKey(goRight))
            {
                transform.Translate(moveSpeed * Time.deltaTime, 0, 0);
            }

            if (Input.GetKey(goLeft))
            {
                transform.Translate(-moveSpeed * Time.deltaTime, 0, 0);
            }

            if (Input.GetKey(goUp))
            {
                transform.Translate(0, moveSpeed * Time.deltaTime, 0);
            }

            if (Input.GetKey(KeyCode.Space)) // goDown
            {
                transform.Translate(0, -moveSpeed * Time.deltaTime, 0);
            }

            //Rotations

            if (UseMouse)
            {
                var layerMask = 1 << 13;
                layerMask = ~layerMask;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out var hit, Mathf.Infinity, layerMask))
                {
                    Vector3 hitPosition = transform.position + transform.TransformDirection(Vector3.forward) * hit.distance;
                    Vector3 hitposition_CSmainCam = Camera.main.transform.InverseTransformPoint(hitPosition);
                    worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, hitposition_CSmainCam.z));
                    Vector3 direction = (worldMousePosition - transform.position);
                    direction.Normalize();
                    transform.rotation = Quaternion.LookRotation(direction, transform.root.up);                    
                }
            }

            if (!UseMouse)
            {
                if (Input.GetKey(rotateUp))
                {
                    transform.Rotate(-turnSpeed * Time.deltaTime, 0, 0);
                }

                if (Input.GetKey(rotateDown))
                {
                    transform.Rotate(turnSpeed * Time.deltaTime, 0, 0);
                }

                if (Input.GetKey(rotateL))
                {
                    transform.Rotate(0, -turnSpeed * Time.deltaTime, 0);
                }

                if (Input.GetKey(rotateR))
                {
                    transform.Rotate(0, turnSpeed * Time.deltaTime, 0);
                }

                if (Input.GetKey(rotateXL))
                {
                    transform.Rotate(0, 0, turnSpeed * Time.deltaTime);
                }

                if (Input.GetKey(rotateXR))
                {
                    transform.Rotate(0, 0, -turnSpeed * Time.deltaTime);
                }
            }
                // Reset
                if (Input.GetKey(resetPos))
                {
                    transform.SetPositionAndRotation(transform.root.position, transform.rotation);
                    transform.Translate(initialLocalPosition);
                }

                if (Input.GetKey(resetRot))
                {
                    Transform transformParent = transform.root;
                    transform.rotation = Quaternion.LookRotation(transformParent.forward, transformParent.up);
                }
        }
    }
}

