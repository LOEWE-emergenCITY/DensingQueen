using System.Collections.Generic;
using UnityEngine;

namespace Flashlight.code
{
    public class CheckInFrustum : MonoBehaviour
    {

        private readonly Dictionary<Camera, HashSet<Collider>> ignoreColliders = new Dictionary<Camera, HashSet<Collider>>();

        public Camera projectorCamera;
        public GameObject decoration;

        public int frustumLayer = 15;
        public int defaultLayer = 0;
        
        private readonly HashSet<Collider> currentlyVisible = new HashSet<Collider>();
        

        public void SetCollidersToIgnore(Camera forCamera)
        {
            ignoreColliders[forCamera] = new HashSet<Collider>(currentlyVisible);
        }

        public void RemoveCollidersToIgnore(Camera forCamera)
        {
            ignoreColliders.Remove(forCamera);
        }

        private HashSet<Collider> GetAllCollidersToIgnore()
        {
            HashSet<Collider> allColliders = new HashSet<Collider>();
            foreach(Camera key in ignoreColliders.Keys)
            {
                allColliders.UnionWith(ignoreColliders[key]);
            }
            return allColliders;
        }
        

        // Update is called once per frame
        private void Update()
        {
            currentlyVisible.Clear();
            var collidingObjects = new HashSet<Collider>();
            foreach(Transform child in transform)
            {
                collidingObjects.UnionWith(child.GetComponent<PlaneTrigger>().CollidingObjects);
            }

            var frustumPlanes = GeometryUtility.CalculateFrustumPlanes(projectorCamera);

            var collidersToIgnore = GetAllCollidersToIgnore();
            CheckFrustumRecursively(decoration, frustumPlanes, collidingObjects, collidersToIgnore);
        }

        private void CheckFrustumRecursively(GameObject obj, Plane[] frustrumPlanes, HashSet<Collider> collidingObjects, HashSet<Collider> collidersToIgnore)
        {
            var c = obj.GetComponentInParent<Collider>();
            if (c != null)
            {
                var inFrustum = GeometryUtility.TestPlanesAABB(frustrumPlanes, c.bounds);
                if (inFrustum && !collidingObjects.Contains(c))
                {
                    obj.layer = frustumLayer;
                    currentlyVisible.Add(c);
                
                }
                else if (!collidersToIgnore.Contains(c))
                {
                    obj.layer = defaultLayer;
                }
            }

            foreach(Transform child in obj.transform)
            {
                CheckFrustumRecursively(child.gameObject, frustrumPlanes, collidingObjects, collidersToIgnore);
            }
        }
    }
}

