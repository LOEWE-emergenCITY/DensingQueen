using UnityEngine;

namespace Flashlight.codeTestScene
{
    public class TSCyborgMovement : MonoBehaviour
    {
        Animator cyborgAnimator;

        public GameObject TurnAround;
        public float rotationRadius;
        public int periodMovement = 5;

        private float walkSpeed = 0.75f;
        private float runSpeed = 1.5f;
        private float elapsedTime;
        private Vector3 target;

        // Start is called before the first frame update
        void Start()
        {
            cyborgAnimator = GetComponent<Animator>();
            elapsedTime = 0;
            target = TurnAround.transform.position;
            cyborgAnimator.SetFloat("Speed", 0);
            transform.position = new Vector3(target.x, transform.position.y, target.z - rotationRadius);
            transform.rotation = Quaternion.LookRotation(target - transform.position, Vector3.up);
            transform.rotation = Quaternion.AngleAxis(-90, Vector3.up);
        }

        // Update is called once per frame
        void Update()
        {
            elapsedTime += Time.deltaTime;

            if (Mathf.Round(elapsedTime / periodMovement) % 2 == 0)
            {
                cyborgAnimator.SetFloat("Speed", 2); // update animation in Animator for Speed > or < to 3
                transform.RotateAround(target, Vector3.up, walkSpeed / rotationRadius * Time.deltaTime * 360);
            }
            else
            {
                cyborgAnimator.SetFloat("Speed", 5); // update animation in Animator for Speed > or < to 3
                transform.RotateAround(target, Vector3.up, runSpeed / rotationRadius * Time.deltaTime * 360);
            }
       
        }
    }
}
