using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Flashlight.codeTestScene;

namespace Flashlight.codeTestScene2
{ 
    public class TS2ProjectorController : MonoBehaviour
    {
        public float maxFov = 100;
        public float minFov = 10;
        public float initialFov = 20;
        public float deleteTolerance = 0.7f;
        public int maxCopiedCameras = 3;
        public GameObject sphereCoupleCopiedProjCam;
        public GameObject projectorLogic;
        public GameObject FlashlightHead;
        public Camera projectorCamera;
        // public SteamVR_Action_Boolean beamSizeActionPlus; // now RightArrow
        // public SteamVR_Action_Boolean beamSizeActionMinus; // now LeftArrow
        // public SteamVR_Action_Boolean freezeBeamAction; // now Mouse click (scroll button)
        public GameObject collidableFrustumPlanes;

        private bool followObjectsActive;
        private TSCheckInFrustum checkInFrustum;

        private List<Tuple<Vector3, float, GameObject, RenderTexture>>
            listPositionsAndSizeWindows;

        private static readonly int PROJ_TEX = Shader.PropertyToID("_ProjTex");
        private Projector projector;

        private void Start()
        {
            projector = projectorLogic.GetComponent<Projector>();
            checkInFrustum = collidableFrustumPlanes.GetComponent<TSCheckInFrustum>();
            listPositionsAndSizeWindows = new List<Tuple<Vector3, float, GameObject, RenderTexture>>();
            projectorLogic.GetComponent<Projector>().fieldOfView = initialFov;
            projectorCamera.fieldOfView = initialFov;
        }
 
        private void Update()
        {
            //followObjectsActive = GetComponent<TS2FollowObject>().IsInSwitchMode;
            //Debug.Log("followObjectsActive" + followObjectsActive);
            Component TS2fo = GetComponent<TS2FollowObject>();
            Debug.Log("TS2FO" + TS2fo);


            if (Input.GetMouseButtonDown(2))
            {
                //if (followObjectsActive) return;
                if (FlashlightHead.GetComponent<TS2FollowObject>().IsInSwitchMode) return;
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
                                            let fovLoop = item.Item2
                                            let plane = new Plane(wallTransform.forward, center)
                                            where (int)Math.Round(plane.GetDistanceToPoint(hitPosition)) == 0 &&
                                                (hitPosition - center).magnitude <= (hitPosition - transform.position).magnitude *
                                                Mathf.Tan(Mathf.Deg2Rad * fovLoop / 2) * deleteTolerance
                                            select item)
                    {
                        Destroy(item.Item4);
                        Destroy(item.Item3);

                        var copiedCamera = listPositionsAndSizeWindows[0].Item3.GetComponentInChildren<Camera>();
                        //Debug.Log("CAMERA =  " + copiedCamera);
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
                            //Debug.Log("CAMERA =  " + copiedCamera);
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
                        copiedProjectorLogic.GetComponent<TSProjectorWrapper>().enabled = true;
                        copiedProjectorLogic.GetComponent<TSProjectorSetPosition>().enabled = false;
                        copiedProjectorLogic.GetComponent<TSFollowHeadForCopiedProjCam>().enabled = true;
                        copiedProjectorLogic.GetComponent<TSFollowGameObjectForCopiedProjCam>().enabled = true;

                        var copiedProjectorCamera =
                            Instantiate(projectorCamera, objectCoupleCopiedProjCam.transform);
                        var copiedTransform = copiedProjectorCamera.transform;
                        copiedTransform.position = position;
                        copiedTransform.rotation = rotation;
                        copiedProjectorCamera.GetComponent<TSFollowHeadForCopiedProjCam>().enabled = true;
                        copiedProjectorCamera.GetComponent<TSFollowGameObjectForCopiedProjCam>().enabled = true;

                        var copiedRenderTexture =
                            Instantiate(projectorCamera.GetComponent<Camera>().targetTexture);
                        copiedProjectorCamera.GetComponent<Camera>().targetTexture = copiedRenderTexture;
                        copiedProjectorLogic.GetComponent<TSProjectorWrapper>().inputFeed = copiedRenderTexture;

                        var copiedProjector = copiedProjectorLogic.GetComponent<Projector>();
                        var copiedMaterial = Object.Instantiate(copiedProjector.material);
                        copiedMaterial.renderQueue -= 1;
                        copiedMaterial.SetTexture(PROJ_TEX, copiedRenderTexture);
                        copiedProjector.material = copiedMaterial;

                        checkInFrustum.SetCollidersToIgnore(copiedProjectorCamera);

                        var fovProj = projectorLogic.GetComponent<Projector>().fieldOfView;
                        listPositionsAndSizeWindows.Add(new Tuple<Vector3, float, GameObject, RenderTexture>(hitPosition,
                            fovProj, objectCoupleCopiedProjCam, copiedRenderTexture));
                    }
                }
            }

            float add = 0;

            //if (increaseBeamSize)
            if (Input.GetKey(KeyCode.RightArrow))
            {
                add += 1f;
            }
            //else if (decreaseBeamSize)
            else if (Input.GetKey(KeyCode.LeftArrow))
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