using UnityEngine;

namespace Flashlight.code
{
    public class MovingSphere : MonoBehaviour
    {
        public float startY = 1.5f;
        public float endY = 4;
        public float speed = 0.5f;

        private bool up = true;

        // Update is called once per frame
        private void Update()
        {
            var deltaS = Time.deltaTime * speed;
            transform.Translate(new Vector3(0, up ? deltaS : -deltaS, 0));
            if (transform.position.y >= endY) up = false;
            else if (transform.position.y <= startY) up = true;
        }
    }
}