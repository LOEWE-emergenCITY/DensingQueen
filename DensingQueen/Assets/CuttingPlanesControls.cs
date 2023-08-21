using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingPlanesControls : MonoBehaviour
{
    public GameObject CuttingPlaneL;
    public GameObject CuttingPlaneR;

    private List<GameObject> Planes = new List<GameObject>();
    private GameObject FarLeftPlane;
    private GameObject FloorPlane;
    private GameObject SkyPlane;
    private GameObject FarRightPlane;

    // Start is called before the first frame update
    void Start()
    {
        CreateSupplementaryPlanes();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePositionsPlanes();
        Inside();
    }

    void CreateSupplementaryPlanes()
    {
        Planes.Add(CuttingPlaneL);

        FarLeftPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        FarLeftPlane.GetComponent<MeshRenderer>().enabled = false;
        FarLeftPlane.transform.SetPositionAndRotation(CuttingPlaneL.transform.position + 20 * CuttingPlaneL.transform.up, CuttingPlaneL.transform.rotation);
        Planes.Add(FarLeftPlane);

        FloorPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        FloorPlane.GetComponent<MeshRenderer>().enabled = false;
        FloorPlane.transform.position = Vector3.zero;
        Planes.Add(FloorPlane);

        SkyPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        SkyPlane.GetComponent<MeshRenderer>().enabled = false;
        SkyPlane.transform.position = new Vector3(0, 20, 0);
        Planes.Add(SkyPlane);

        Planes.Add(CuttingPlaneR);

        FarRightPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        FarRightPlane.GetComponent<MeshRenderer>().enabled = false;
        FarRightPlane.transform.SetPositionAndRotation(CuttingPlaneR.transform.position + 20 * CuttingPlaneR.transform.up, CuttingPlaneR.transform.rotation);
        Planes.Add(FarRightPlane);
    }

    void UpdatePositionsPlanes()
    {
        FarLeftPlane.transform.SetPositionAndRotation(CuttingPlaneL.transform.position + 20 * CuttingPlaneL.transform.up, CuttingPlaneL.transform.rotation);
        FarRightPlane.transform.SetPositionAndRotation(CuttingPlaneR.transform.position + 20 * CuttingPlaneR.transform.up, CuttingPlaneR.transform.rotation);
    }

    void Inside()
    {
        int i = 1;
        foreach (GameObject p in Planes)
        {
            var n = p.transform.up;
            var a = n.x;
            var b = n.y;
            var c = n.z;
            var d = (p.transform.position - transform.parent.position).magnitude;
            //var dot = Vector4.Dot(new Vector4(a,b,c,d), new Vector4(pos.x,pos.y,pos.z,1));
            Shader.SetGlobalVector("plane" + i, new Vector4(a, b, c, d));
            //inside &= dot > 0;
            i++;
        }
        var pos = transform.parent.position; //(CuttingPlaneL.transform.position + CuttingPlaneR.transform.position)/2;
        var dir = transform.parent.up; //(CuttingPlaneL.transform.forward + CuttingPlaneR.transform.forward).normalized;
        Shader.SetGlobalVector("rayOri", new Vector3(pos.x, pos.y, pos.z));
        Shader.SetGlobalVector("rayDir", new Vector3(dir.x, dir.y, dir.z));
    }
}
