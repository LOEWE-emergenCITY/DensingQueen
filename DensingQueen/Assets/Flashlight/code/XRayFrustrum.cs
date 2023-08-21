using UnityEngine;

public class XRayFrustrum : MonoBehaviour
{
    public Camera camera;
    public Material xrayMaterial;

    public GameObject NearC;
    public GameObject FarC;

    private StudyControls studyControls;
    // Start is called before the first frame update
    void Start()
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(camera);
        foreach (var p in planes)
        {
            var n = p.normal;
        }
        studyControls = FindObjectOfType<StudyControls>();
    }

    // Update is called once per frame
    void Update()
    {
        Inside();
        NearC.transform.position = camera.transform.position + camera.transform.forward * camera.nearClipPlane;
        FarC.transform.position = camera.transform.position + camera.transform.forward * camera.farClipPlane;
        
        var a = Mathf.Tan(studyControls.fov / camera.farClipPlane);
        var lr = Mathf.Atan(a) * camera.nearClipPlane;
        var nearScale = 40.0f * lr;// studyControls.fov;//Mathf.Atan(studyControls.fov) * camera.nearClipPlane;
        var farScale = 40.0f* studyControls.fov;//Mathf.Atan(studyControls.fov) * camera.farClipPlane ;
        NearC.transform.localScale = new Vector3(nearScale,0.01f,nearScale);
        FarC.transform.localScale = new Vector3(farScale, 0.01f, farScale);

        
    }

    void Inside()
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(camera);

        int i = 1;
        foreach (var p in planes)
        {
            var n = p.normal;
            var a = n.x;
            var b = n.y;
            var c = n.z;
            var d = p.distance;
            //var dot = Vector4.Dot(new Vector4(a,b,c,d), new Vector4(pos.x,pos.y,pos.z,1));
            Shader.SetGlobalVector("plane" + i, new Vector4(a, b, c, d));
            //inside &= dot > 0;
            i++;
        }
        var op = planes[4].normal;
        Shader.SetGlobalVector("plane0", new Vector4(op.x, op.y, op.z, 0));// camera.transform.position.magnitude));
        Shader.SetGlobalFloat("angle", Mathf.Tan(studyControls.fov / camera.farClipPlane));
        Shader.SetGlobalFloat("far", camera.farClipPlane);
        var pos = camera.transform.position;
        var dir = camera.transform.forward;
        Shader.SetGlobalVector("rayOri", new Vector3(pos.x, pos.y, pos.z));
        Shader.SetGlobalVector("rayDir", new Vector3(dir.x, dir.y, dir.z));
        xrayMaterial.SetFloat("_usePlanes", 0);
        Shader.SetGlobalFloat("lamp", 1.0f);
    }
}
