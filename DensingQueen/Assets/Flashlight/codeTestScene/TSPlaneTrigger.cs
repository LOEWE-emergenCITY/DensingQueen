﻿using System.Collections.Generic;
using UnityEngine;

namespace Flashlight.codeTestScene
{
    public class TSPlaneTrigger : MonoBehaviour
    {
        public HashSet<Collider> CollidingObjects { get; } = new HashSet<Collider>();

        private void OnTriggerEnter(Collider other)
        {
            CollidingObjects.Add(other);
        }

        private void OnTriggerExit(Collider other)
        {
            CollidingObjects.Remove(other);
        }
    }
}
