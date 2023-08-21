using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Valve.VR;
using Object = UnityEngine.Object;

namespace Flashlight.code
{
    public class ProjectorController : MonoBehaviour
    {
        public float maxFov = 100;
        public float minFov = 10;
        public float initialFov = 20;
        public float deleteTolerance = 0.7f;
        public int maxCopiedCameras = 3;
        public GameObject sphereCoupleCopiedProjCam;
        public GameObject projectorLogic;
        public Camera projectorCamera;
        public SteamVR_Action_Boolean beamSizeActionPlus;
        public SteamVR_Action_Boolean beamSizeActionMinus;
        public SteamVR_Action_Boolean freezeBeamAction;
        public SteamVR_Action_Boolean modeSwitchAction;
        public GameObject collidableFrustumPlanes;

        private bool followObjectsActive;
        private CheckInFrustum checkInFrustum;
        private bool increaseBeamSize;
        private bool decreaseBeamSize;

        private List<Tuple<Vector3, float, GameObject, RenderTexture>>
            listPositionsAndSizeWindows;

        private static readonly int PROJ_TEX = Shader.PropertyToID("_ProjTex");
        private Projector projector;

        private void Start()
        {
            projector = projectorLogic.GetComponent<Projector>();
            checkInFrustum = collidableFrustumPlanes.GetComponent<CheckInFrustum>();
            listPositionsAndSizeWindows = new List<Tuple<Vector3, float, GameObject, RenderTexture>>();
            freezeBeamAction.AddOnStateDownListener(OnTriggerDown, SteamVR_Input_Sources.RightHand);
            modeSwitchAction.AddOnChangeListener(OnModeChange, SteamVR_Input_Sources.RightHand);
            projectorLogic.GetComponent<Projector>().fieldOfView = initialFov;
            projectorCamera.fieldOfView = initialFov;

            beamSizeActionMinus.AddOnChangeListener(OnBeamSizeMinus, SteamVR_Input_Sources.RightHand);
            beamSizeActionPlus.AddOnChangeListener(OnBeamSizePlus, SteamVR_Input_Sources.RightHand);
        }

        private void OnBeamSizePlus(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
        {
            increaseBeamSize = newState;
        }

        private void OnBeamSizeMinus(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
        {
            decreaseBeamSize = newState;
        }

        private void OnModeChange(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
        {
            followObjectsActive = newState;
        }

        private void OnTriggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            if (followObjectsActive) return;
            var layerMask = 1 << 13;
            layerMask = ~layerMask;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out var hit,
                Mathf.Infinity,
                layerMask))
            {
                var wallTransform = hit.collider.transform;
                var hitPosition = transform.position + transform.TransformDirection(Vector3.forward) * hit.distance;
                var toDelete = new List<Tuple<Vector3, float, GameObject, RenderTexture>>();

                foreach (var item in from item in listPositionsAndSizeWindows
                    let center = item.Item1
                    let fov = item.Item2
                    let plane = new Plane(wallTransform.forward, center)
                    where (int) Math.Round(plane.GetDistanceToPoint(hitPosition)) == 0 &&
                          (hitPosition - center).magnitude <= (hitPosition - transform.position).magnitude *
                          Mathf.Tan(Mathf.Deg2Rad * fov / 2) * deleteTolerance
                    select item)
                {
                    Destroy(item.Item4);
                    Destroy(item.Item3);

                    var copiedCamera = listPositionsAndSizeWindows[0].Item3.GetComponentInChildren<Camera>();
                    Debug.Log("CAMERA =  " + copiedCamera);
                    checkInFrustum.RemoveCollidersToIgnore(copiedCamera);

                    toDelete.Add(item);
                }

                foreach (var item in toDelete)
                {
                    listPositionsAndSizeWindows.Remove(item);
                }

                if (toDelete.Count == 0)
                {
                    if (listPositionsAndSizeWindows.Count == maxCopiedCameras)
                    {
                        Destroy(listPositionsAndSizeWindows[0].Item4);
                        Destroy(listPositionsAndSizeWindows[0].Item3);

                        var copiedCamera = listPositionsAndSizeWindows[0].Item3.GetComponentInChildren<Camera>();
                        Debug.Log("CAMERA =  " + copiedCamera);
                        checkInFrustum.RemoveCollidersToIgnore(copiedCamera);

                        for (var i = 0; i < maxCopiedCameras - 1; i++)
                        {
                            listPositionsAndSizeWindows[i] = listPositionsAndSizeWindows[i + 1];
                        }

                        listPositionsAndSizeWindows.RemoveAt(maxCopiedCameras - 1);
                    }

                    var objectCoupleCopiedProjCam = Object.Instantiate(sphereCoupleCopiedProjCam);
                    objectCoupleCopiedProjCam.transform.position = hitPosition;

                    var copiedProjectorLogic =
                        Instantiate(projectorLogic, objectCoupleCopiedProjCam.transform);
                    var orgTransform = projectorCamera.transform;
                    var position = orgTransform.position;
                    copiedProjectorLogic.transform.position = position;
                    var rotation = orgTransform.rotation;
                    copiedProjectorLogic.transform.rotation = rotation;
                    copiedProjectorLogic.GetComponent<ProjectorWrapper>().enabled = true;
                    copiedProjectorLogic.GetComponent<ProjectorSetPosition>().enabled = false;
                    copiedProjectorLogic.GetComponent<FollowHeadForCopiedProjCam>().enabled = true;

                    var copiedProjectorCamera =
                        Instantiate(projectorCamera, objectCoupleCopiedProjCam.transform);
                    var copiedTransform = copiedProjectorCamera.transform;
                    copiedTransform.position = position;
                    copiedTransform.rotation = rotation;
                    copiedProjectorCamera.GetComponent<FollowHeadForCopiedProjCam>().enabled = true;

                    var copiedRenderTexture =
                        Instantiate(projectorCamera.GetComponent<Camera>().targetTexture);
                    copiedProjectorCamera.GetComponent<Camera>().targetTexture = copiedRenderTexture;
                    copiedProjectorLogic.GetComponent<ProjectorWrapper>().inputFeed = copiedRenderTexture;

                    var copiedProjector = copiedProjectorLogic.GetComponent<Projector>();
                    var copiedMaterial = Object.Instantiate(copiedProjector.material);
                    copiedMaterial.renderQueue -= 1;
                    copiedMaterial.SetTexture(PROJ_TEX, copiedRenderTexture);
                    copiedProjector.material = copiedMaterial;

                    checkInFrustum.SetCollidersToIgnore(copiedProjectorCamera);

                    var fov = projectorLogic.GetComponent<Projector>().fieldOfView;
                    listPositionsAndSizeWindows.Add(new Tuple<Vector3, float, GameObject, RenderTexture>(hitPosition,
                        fov, objectCoupleCopiedProjCam, copiedRenderTexture));
                }
            }
        }


        // Update is called once per frame
        private void Update()
        {
            float add = 0;

            if (increaseBeamSize)
            {
                add += 1f;
            }
            else if (decreaseBeamSize)
            {
                add -= 1f;
            }

            var fov = projector.fieldOfView + add;

            if (minFov <= fov && fov <= maxFov)
            {
                projector.fieldOfView += add;
                projectorCamera.fieldOfView += add;
            }
        }
    }
}