using System.Collections.Specialized;
using UnityEngine;
using Valve.VR;

public class XRayFrustrumPlanes : MonoBehaviour
{
    public Camera cameraLeft;
    public Camera cameraRight;
    public Material xrayMaterial;

    public GameObject leftPlaneViz;
    public GameObject rightPlaneViz;

    public SteamVR_Action_Boolean beamSizeActionPlus;
    public SteamVR_Action_Boolean beamSizeActionMinus;

    public SteamVR_Action_Boolean volumeForward;
    public SteamVR_Action_Boolean volumeBackward;


    bool leftUp;
    bool leftRight;
    bool leftDown;
    bool leftLeft;

    bool rightUp;
    bool rightRight;
    bool rightDown;
    bool rightLeft;

    float leftWidth = 1;
    float leftHeight = 1;

    float rightWidth = 1;
    float rightHeight = 1;

    public float stepSize = 0.01f;
    public float minSize = 0.001f;

    public float originalSize = 0.2f;

    Rect leftRect = new Rect(0,0,1,1);
    Rect rightRect = new Rect(0,0,1,1);

    // Start is called before the first frame update
    void Start()
    {
        cameraRight.aspect = 1;
        cameraLeft.aspect = 1;
        
        beamSizeActionPlus.AddOnChangeListener(OnLeftHeightPlus, SteamVR_Input_Sources.LeftHand);
        beamSizeActionMinus.AddOnChangeListener(OnLeftHeightMinus, SteamVR_Input_Sources.LeftHand);
        volumeForward.AddOnChangeListener(OnLeftWidthPlus, SteamVR_Input_Sources.LeftHand);
        volumeBackward.AddOnChangeListener(OnLeftWidthMinus, SteamVR_Input_Sources.LeftHand);

        
        beamSizeActionPlus.AddOnChangeListener(OnRightHeightPlus, SteamVR_Input_Sources.RightHand);
        beamSizeActionMinus.AddOnChangeListener(OnRightHeightMinus, SteamVR_Input_Sources.RightHand);
        volumeForward.AddOnChangeListener(OnRightWidthPlus, SteamVR_Input_Sources.RightHand);
        volumeBackward.AddOnChangeListener(OnRightWidthMinus, SteamVR_Input_Sources.RightHand);


        //var planes = GeometryUtility.CalculateFrustumPlanes(cameraLeft);
        //foreach (var p in planes)
        //{
        //    var n = p.normal;
        //    Debug.Log(Vector3.Dot(n, p.distance * n) + "   " + p.distance);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
        Inside();

    }

    void FixedUpdate()
    {
        float leftWidthChange = 0f;
        float leftHeightChange = 0f;

        float rightWidthChange = 0f;
        float rightHeightChange = 0f;

        if (leftUp)
        {
            leftHeightChange += stepSize;
        }

        if (leftDown)
        {
            leftHeightChange -= stepSize;
        }

        if (leftRight)
        {
             leftWidthChange += stepSize;
        }

        if (leftLeft)
        {
            leftWidthChange -= stepSize;
        }


        if (rightUp)
        {
            rightHeightChange += stepSize;
        }

        if (rightDown)
        {
            rightHeightChange -= stepSize;
        }

        if (rightRight)
        {
            rightWidthChange += stepSize;
        }

        if (rightLeft)
        {
            rightWidthChange -= stepSize;
        }


        leftWidth += leftWidthChange;
        leftHeight += leftHeightChange;

        rightWidth += rightWidthChange;
        rightHeight += rightHeightChange;

        leftWidth = QuickClamp(leftWidth);
        leftHeight = QuickClamp(leftHeight);

        rightWidth = QuickClamp(rightWidth);
        rightHeight = QuickClamp(rightHeight);

        leftRect.width = leftWidth;
        leftRect.height = leftHeight;

        rightRect.width = rightWidth;
        rightRect.height = rightHeight;

        cameraLeft.orthographicSize = leftWidth * originalSize;
        cameraRight.orthographicSize = rightWidth * originalSize;

        cameraLeft.aspect = leftHeight / leftWidth;
        cameraRight.aspect = rightHeight / rightWidth;

        //cameraLeft.rect = leftRect;
        //cameraRight.rect = rightRect;

        leftPlaneViz.transform.localScale = new Vector3(leftHeight * 0.4f, leftWidth * 0.4f, 1);
        rightPlaneViz.transform.localScale = new Vector3(rightHeight * 0.4f, rightWidth * 0.4f, 1);


    }

    void Inside()
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(cameraLeft);
        
        int i = 1;
        foreach (var p in planes)
        {
            if (i == 6)
            {
                continue;
            }
            var n = p.normal;
            var a = n.x;
            var b = n.y;
            var c = n.z;
            var d = p.distance;
            //var dot = Vector4.Dot(new Vector4(a,b,c,d), new Vector4(pos.x,pos.y,pos.z,1));
            Shader.SetGlobalVector("plane"+i, new Vector4(a,b,c,d));
            //inside &= dot > 0;
            i++;

        }
        planes = GeometryUtility.CalculateFrustumPlanes(cameraRight);
        i = 1;
        foreach (var p in planes)
        {
            if (i == 6)
            {
                continue;
            }
            var n = p.normal;
            var a = n.x;
            var b = n.y;
            var c = n.z;
            var d = p.distance;
            //var dot = Vector4.Dot(new Vector4(a,b,c,d), new Vector4(pos.x,pos.y,pos.z,1));
            Shader.SetGlobalVector("plane" + (i+5), new Vector4(a, b, c, d));
            //inside &= dot > 0;
            i++;
        }
        Shader.SetGlobalFloat("lamp", -1.0f);
        xrayMaterial.SetFloat("_usePlanes", 1);
    }

    void OnLeftHeightPlus(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
    {
        leftUp = newState;
    }

    void OnLeftHeightMinus(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
    {
        leftDown = newState;
    }

    void OnRightHeightPlus(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
    {
        rightUp = newState;
    }

    void OnRightHeightMinus(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
    {
        rightDown = newState;
    }


    void OnLeftWidthPlus(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
    {
        leftRight = newState;
    }

    void OnLeftWidthMinus(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
    {
        leftLeft = newState;
    }

    void OnRightWidthPlus(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
    {
        rightRight = newState;
    }

    void OnRightWidthMinus(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
    {
        rightLeft = newState;
    }

    float QuickClamp(float input)
    {
        if(input > 1)
        {
            return 1;
        }

        if(input < minSize)
        {
            return minSize;
        }

        return input;
    }
}
