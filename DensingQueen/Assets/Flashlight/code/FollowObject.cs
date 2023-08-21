using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Valve.VR;

namespace Flashlight.code
{
    public class FollowObject : MonoBehaviour
    {
        public SteamVR_Action_Boolean switchModeAction;
        public SteamVR_Action_Boolean markAction;
        public SteamVR_Action_Boolean followAction;
        public Material followMaterial;
        public GameObject linePrefab;
        public int markLayer = 15;
        public GameObject followingObjectMarkerPrefab;

        private bool following;
        public bool IsInSwitchMode { get; private set; }

        private HashSet<GameObject> followingObjects;
        private GameObject lineObject;
        private Tuple<GameObject, Camera, RenderTexture> copiedCameraForMarkedObject;
        private GameObject marker;
        private LineRenderer lineRenderer;

        // Start is called before the first frame update
        private void Start()
        {
            switchModeAction.AddOnChangeListener(OnModeChange, SteamVR_Input_Sources.RightHand);
            markAction.AddOnStateDownListener(OnMarkObject, SteamVR_Input_Sources.RightHand);
            followAction.AddOnStateDownListener(OnFollowObject, SteamVR_Input_Sources.RightHand);
            followingObjects = new HashSet<GameObject>();
        }

        private void OnFollowObject(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            if (IsInSwitchMode)
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

                        var position = hit.transform.position;
                        marker.transform.position = new Vector3(position.x,
                            2 * hitObject.GetComponent<Collider>().bounds.extents.y + 1, position.z);
                        var localScale = hitObject.transform.localScale;
                        marker.transform.localScale =
                            new Vector3(0.5f / localScale.x, 0.5f / localScale.y, 0.5f / localScale.z);

                        GetComponent<BehindWallCameraController>().SetGameObjectToFollow(hitObject);
                        following = true;
                    }
                }
                else
                {
                    GetComponent<BehindWallCameraController>().SetGameObjectToFollow(null);
                    Destroy(marker);
                    following = false;
                }
            }
        }

        private void OnModeChange(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
        {
            IsInSwitchMode = newState;
            Debug.Log("FollowObject : active = " + IsInSwitchMode);

            if (IsInSwitchMode && lineObject == null)
            {
                lineObject = Instantiate(linePrefab);
                lineRenderer = lineObject.GetComponent<LineRenderer>();
            }

            lineObject.SetActive(IsInSwitchMode);
        }

        private void OnMarkObject(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            Debug.Log("FollowObject active: " + IsInSwitchMode);
            if (IsInSwitchMode)
            {
                var layerMask = 1 << markLayer;

                // Does the ray intersect any objects excluding the player layer
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out var hit,
                    Mathf.Infinity, layerMask))
                {
                    var originTransform = transform;
                    var position = originTransform.position;
                    Debug.DrawRay(position, hit.transform.position - position, Color.red);
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

        // Update is called once per frame
        private void Update()
        {
            if (IsInSwitchMode)
            {
                var originTransform = transform;
                var position = originTransform.position;
                lineRenderer.SetPositions(new[] {position, position + 100 * transform.TransformDirection(Vector3.forward)});
                lineRenderer.material.renderQueue = 4000;
            }

            //Transform player = GameObject.FindWithTag("Player").transform;
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