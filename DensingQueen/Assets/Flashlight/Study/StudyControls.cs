using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class StudyControls : MonoBehaviour
{
    public float maxFov = 1;
    public float minFov = 0.05f;
    public float fov = 0.2f;
    public float speed = 1;
    private const float START_NEAR = 0.3f;
    public float visibleDistance = 0.5f;
    public float startFov = 20;
    private Vector2 nearFar;

    public float radiusStepSize = 0.02f;

    public Material transMat;
    public Material transMatTarget;

    public SteamVR_Action_Boolean beamSizeActionPlus;
    public SteamVR_Action_Boolean beamSizeActionMinus;

    public SteamVR_Action_Boolean volumeForward;
    public SteamVR_Action_Boolean volumeBackward;

    public SteamVR_Action_Boolean participaneClick;

    public Camera cullingCam;

    private bool increaseBeamSize;
    private bool decreaseBeamSize;
    private bool volFwd;
    private bool volBack;
    // Start is called before the first frame update
    void Start()
    {
        beamSizeActionPlus.AddOnChangeListener(OnBeamSizePlus, SteamVR_Input_Sources.RightHand);
        beamSizeActionMinus.AddOnChangeListener(OnBeamSizeMinus, SteamVR_Input_Sources.RightHand);
        volumeForward.AddOnChangeListener(OnVolumeForward, SteamVR_Input_Sources.RightHand); 
        volumeBackward.AddOnChangeListener(OnVolumeBackward, SteamVR_Input_Sources.RightHand);
        nearFar = new Vector2(START_NEAR, START_NEAR + visibleDistance);
        //cullingCam.fieldOfView = startFov;
        UpdateClippingPlanes();
    }

    // Update is called once per frame
    void Update()
    {
        float addFOV = 0;
        float movVol = 0;
        Shader.SetGlobalFloat("rad", fov);
        //Debug.Log("beamSizeActionPlus.state = " + beamSizeActionPlus.state);
        //Debug.Log("beamSizeActionMinus.state = " + beamSizeActionMinus.state);

        if (increaseBeamSize)
        {
            addFOV += radiusStepSize;
            //movVol += 1f;
        }
        else if (decreaseBeamSize)
        {
            addFOV -= radiusStepSize;
            //movVol -= 1f;
        }
        else if (volFwd)
        {
            movVol += 1f;
        }
        else if (volBack)
        {
            movVol -= 1f;
        }

        fov += addFOV;

        if (fov <= minFov)
            fov = minFov;
        if (maxFov <= fov)
            fov = maxFov;


        //Vector3 direction = new Vector3(0f, 0f, movVol);
        float movement = Time.deltaTime * speed * movVol;
        

        //print("Local Position: " + camera.transform.localPosition);
        //print("Movement: " + movement);


        nearFar += new Vector2(movement, movement);
        if (nearFar.x > 0.1f && nearFar.y < 2.0f)
        {
            UpdateClippingPlanes();
        }
        else
        {
            nearFar -= new Vector2(movement, movement);
        }

        //cullingCam.farClipPlane += movVol;
        //cullingCam.nearClipPlane += movVol;

        if (minFov <= fov && fov <= maxFov)
        {
            
            transMat.SetFloat("_Rad", fov);
            Shader.SetGlobalFloat("rad", fov);




        }
    }

    private void UpdateClippingPlanes()
    {
        cullingCam.nearClipPlane = nearFar.x;
        cullingCam.farClipPlane = nearFar.y;
    }

    private void OnBeamSizePlus(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
    {
        increaseBeamSize = newState;
    }

    private void OnBeamSizeMinus(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
    {
        decreaseBeamSize = newState;
    }


    private void OnVolumeForward(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
    {
        volFwd = newState;
    }

    private void OnVolumeBackward(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
    {
        volBack = newState;
    }
}
