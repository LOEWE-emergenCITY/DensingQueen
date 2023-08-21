using System;
using UnityEngine;

namespace Flashlight.code
{
    public class PeriodicDisappear : MonoBehaviour
    {

        public GameObject wall;
        public int period; // in seconds
        private float elapsedTime;

        // Start is called before the first frame update
        private void Start()
        {
            wall.SetActive(false);
            elapsedTime = 0;
        }

        // Update is called once per frame
        private void Update()
        {
            elapsedTime += Time.deltaTime;
            wall.SetActive(Math.Abs(Mathf.Round(elapsedTime / period) % 2) < 0.1);
        }
    }
}

