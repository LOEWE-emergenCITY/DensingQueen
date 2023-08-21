using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Condition;

public class InVolumeCheck : MonoBehaviour
{

    public Camera RightPlane;
    public Camera LeftPlane;
    public Camera Lamp;

    private StudyControls studyControls;

    void Start()
    {
        studyControls = FindObjectOfType<StudyControls>();
    }

    public bool IsInVolume(GameObject target, INPUT input)
    {
        bool res = false;
        switch (input)
        {
            case INPUT.LAMP:
                var planes = GeometryUtility.CalculateFrustumPlanes(Lamp);
                res = GeometryUtility.TestPlanesAABB(planes, target.GetComponent<Renderer>().bounds);
                var dir = Lamp.transform.forward;
                var pos = Lamp.transform.position;
                Plane p = new Plane(Lamp.transform.forward,Lamp.transform.position);
                var dis = p.GetDistanceToPoint(target.transform.position);
                var relDis = dis / Lamp.farClipPlane;
                res &= (Vector3.Cross(dir, target.transform.position-pos).magnitude < studyControls.fov*relDis);
                //var dd = Mathf.Acos
                break;
            case INPUT.PLANES:
                var planes1 = GeometryUtility.CalculateFrustumPlanes(RightPlane);
                var planes2 = GeometryUtility.CalculateFrustumPlanes(LeftPlane);
                res = GeometryUtility.TestPlanesAABB(planes1, target.GetComponent<Renderer>().bounds) & GeometryUtility.TestPlanesAABB(planes2, target.GetComponent<Renderer>().bounds);
                break;
            case INPUT.NONE:
                
                break;
        }
        return res;
    }
}
