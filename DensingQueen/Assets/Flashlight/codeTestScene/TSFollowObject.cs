using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Flashlight.codeTestScene
{
    public class TSFollowObject : MonoBehaviour
    {
        public Material followMaterial;
        public GameObject linePrefab;
        public int markLayer = 15;
        public GameObject followingObjectMarkerPrefab;

        private bool following;
        public bool IsInSwitchMode { get; private set; }

        private HashSet<GameObject> followingObjects;
        private GameObject lineObject;
        private GameObject marker;
        private LineRenderer lineRenderer;

        // Start is called before the first frame update
        private void Start()
        {
            followingObjects = new HashSet<GameObject>();
            lineObject = Instantiate(linePrefab);
            lineRenderer = lineObject.GetComponent<LineRenderer>();
            lineObject.SetActive(false);
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.CapsLock)) // grab button in AR
            {
                IsInSwitchMode = !IsInSwitchMode;
            }

            //Debug.Log("FollowObject : active = " + IsInSwitchMode);

            lineObject.SetActive(IsInSwitchMode);


            if (IsInSwitchMode)
            {
                var originTransform = transform;
                var position = originTransform.position;
                lineRenderer.SetPositions(new[] {position, position + 100 * transform.TransformDirection(Vector3.forward)});
                lineRenderer.material.renderQueue = 4000;

                if(Input.GetMouseButtonDown(2)) // Mouse click (scroll button) : to follow automatically a (moving) object
                {
                    if (!following)
                    {
                        var layerMask = 1 << markLayer;

                        // Does the ray intersect any objects excluding the player layer
                        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out var hit,
                            Mathf.Infinity, layerMask))
                        {
                            var hitObject = hit.collider.gameObject;
                            marker = Instantiate(followingObjectMarkerPrefab, hitObject.transform);

                            var positionObject = hit.transform.position;
                            marker.transform.position = new Vector3(positionObject.x,
                                2 * hitObject.GetComponent<Collider>().bounds.extents.y + 1, positionObject.z);
                            var localScale = hitObject.transform.localScale;
                            marker.transform.localScale =
                                new Vector3(0.5f / localScale.x, 0.5f / localScale.y, 0.5f / localScale.z);

                            GetComponent<TSBehindWallCameraController>().SetGameObjectToFollow(hitObject);
                            following = true;
                        }
                    }
                    else
                    {
                        GetComponent<TSBehindWallCameraController>().SetGameObjectToFollow(null);
                        Destroy(marker);
                        following = false;
                    }
                }

            if (Input.GetMouseButtonDown(0)) // Mouse left click: draw the shape of the object on the wall 
                {
                    var layerMask = 1 << markLayer;

                    // Does the ray intersect any objects excluding the player layer
                    if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out var hit,
                        Mathf.Infinity, layerMask))
                    {
                        var flashlightTransform = transform;
                        var flashlightPosition = flashlightTransform.position;
                        Debug.DrawRay(flashlightPosition, hit.transform.position - flashlightPosition, Color.red);
                        var colliderGameObject = hit.collider.gameObject;
                        Debug.Log("FollowObject Hit object: " + colliderGameObject);

                        if (followingObjects.Contains(colliderGameObject))
                        {
                            var renderers = colliderGameObject.GetComponentsInChildren<Renderer>();
                            foreach (var r in renderers)
                            {
                                var materials = new Material[1];
                                materials[0] = r.materials[0];
                                r.materials = materials;
                                followingObjects.Remove(colliderGameObject);
                            }
                        }
                        else
                        {
                            followingObjects.Add(colliderGameObject);

                            var renderers = colliderGameObject.GetComponentsInChildren<Renderer>();
                            foreach (var r in renderers)
                            {
                                var materials = new Material[2];
                                materials[0] = r.materials[0];
                                materials[1] = Instantiate(followMaterial);
                                r.materials = materials;
                            }
                        }
                    }
                }
            }

            var center = new Vector3(4.81f, 0, 1.7f);

            var items = (from followingObject in followingObjects
                let distance = (followingObject.transform.position - center).magnitude
                select Tuple.Create(followingObject, distance)).ToList();

            items.Sort((first, second) => first.Item2.CompareTo(second.Item2));

            var renderQueue = 3100;
            foreach (var item in items)
            {
                Debug.Log("RenderQueue = " + renderQueue);

                var renderers = item.Item1.GetComponentsInChildren<Renderer>();
                foreach (var r in renderers)
                {
                    var materials = r.materials;
                    materials[1].renderQueue = renderQueue;
                    r.materials = materials;
                }

                renderQueue++;
            }
        }
    }
}