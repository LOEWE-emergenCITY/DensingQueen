using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeVisLine : MonoBehaviour
{

    public GameObject near;
    public GameObject far;

    public LineRenderer[] lineRenderers;

    public int lines = 4;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderers = new LineRenderer[lines];
        for (int i = 0; i < lines; i++)
        {
            var g = new GameObject("line");
            g.transform.parent = this.transform;
            g.transform.position = Vector3.zero;
            var l = g.AddComponent<LineRenderer>();
            lineRenderers[i] = l;
            l.positionCount = 2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        var nrad = near.transform.localScale.x * near.transform.parent.localScale.x;
        var frad = far.transform.localScale.x * far.transform.parent.localScale.x;
        var npos = near.transform.localPosition;
        var fpos = far.transform.localPosition;

        for(int i = 0; i < lines; i ++)
        {
            var a = Mathf.PI/lines;
            var x1 = npos.x + nrad * Mathf.Cos(a);
            var y1 = npos.y + nrad * Mathf.Sin(a);

            var x2 = fpos.x + frad * Mathf.Cos(a);
            var y2 = fpos.y + frad * Mathf.Sin(a);

            lineRenderers[i].SetPosition(0, new Vector3(x1, y1 / 20, npos.z));
            lineRenderers[i].SetPosition(1, new Vector3(x2, y2 / 20, fpos.z));
        }

       
    }
}
