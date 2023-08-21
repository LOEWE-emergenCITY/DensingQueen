using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flashlight.codeTestScene
{
    public class TSMoveHead : MonoBehaviour
    {

        private float turnSpeed = 60f;

        public string lookUp;
        public string lookDown;
        public string lookL;
        public string lookR;
        public string rotateXL;
        public string rotateXR;
        public string reset;


        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(lookUp))
            {
                transform.Rotate(-turnSpeed * Time.deltaTime, 0, 0);
            }

            if (Input.GetKey(lookDown))
            {
                transform.Rotate(turnSpeed * Time.deltaTime, 0, 0);
            }

            if (Input.GetKey(lookL))
            {
                transform.Rotate(0, -turnSpeed * Time.deltaTime, 0);
            }

            if (Input.GetKey(lookR))
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

            if (Input.GetKey(reset))
            {
                Transform transformParent = transform.root;
                transform.rotation = Quaternion.LookRotation(transformParent.forward, transformParent.up);
            }
        }
    }
}
