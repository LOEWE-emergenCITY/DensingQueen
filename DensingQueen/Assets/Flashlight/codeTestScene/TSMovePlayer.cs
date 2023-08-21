using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Flashlight.codeTestScene
{
    public class TSMovePlayer : MonoBehaviour
    {
        private float walkSpeed = 5f;
        private float turnSpeed = 60f;

        public string goForward;
        public string goBackward;
        public string goLeft;
        public string goRight;
        public string turnL;
        public string turnR;
        

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(goForward))
            {
                transform.Translate(0, 0, walkSpeed * Time.deltaTime);
            }

            if (Input.GetKey(goBackward))
            {
                transform.Translate(0, 0, -walkSpeed / 2 * Time.deltaTime);
            }

            if (Input.GetKey(goRight))
            {
                transform.Translate(walkSpeed * Time.deltaTime, 0, 0);
            }

            if (Input.GetKey(goLeft))
            {
                transform.Translate(-walkSpeed * Time.deltaTime, 0, 0);
            }

            if (Input.GetKey(turnL))
            {
                transform.Rotate(0, -turnSpeed * Time.deltaTime, 0);
            }

            if (Input.GetKey(turnR))
            {
                transform.Rotate(0, turnSpeed * Time.deltaTime, 0);
            }
        }
    }
}

