using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.Extras;

public class QuadrantChoice : MonoBehaviour
{
    public static event Action<int, float> OnQuadChosen;

    public SteamVR_LaserPointer Laser;
    public GameObject Controller;
    private GameObject[] quads;
    public bool choosing = false;

    public int currentQuad;
    public float time;

    // Start is called before the first frame update
    void Start()
    {
        Laser.PointerIn += Laser_PointerIn;
        Laser.PointerOut += Laser_PointerOut;
        Laser.PointerClick += Laser_PointerClick;
    }

    private void Laser_PointerClick(object sender, PointerEventArgs e)
    {
        if (!choosing)
            return;
        var quad = e.target.gameObject.GetComponent<Quadrant>();
        if(quad != null)
        {
            Debug.Log("QUAD " + quad.ID);
            OnQuadChosen?.Invoke(quad.ID,time);
            Reset();
        }
        else
        {
            Debug.Log("No quad selected");
        }
    }

    private void Laser_PointerOut(object sender, PointerEventArgs e)
    {
        if (!choosing)
            return;
        var quad = e.target.gameObject.GetComponent<Quadrant>();
        if (quad != null)
        {
            quad.GetComponent<MeshRenderer>().material.color = Color.gray;
        }
        else
        {
            Debug.Log("No quad selected");
        }
    }

    private void Laser_PointerIn(object sender, PointerEventArgs e)
    {
        if (!choosing)
            return;
        var quad = e.target.gameObject.GetComponent<Quadrant>();
        if (quad != null)
        {
            quad.GetComponent<MeshRenderer>().material.color = Color.green;
        }
        else
        {
            Debug.Log("No quad selected");
        }
    }

    public void Reset()
    {
        choosing = false;
        Laser.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
        
    }

    public void StartChoosing(GameObject[] Quadrants, float t)
    {
        this.gameObject.SetActive(true);
        quads = Quadrants;
        choosing = true;
        time = t;
        StartCoroutine(WaitThenEnableLaser());
    }

    public IEnumerator WaitThenEnableLaser()
    {
        yield return new WaitForSeconds(0.5f);
        Laser.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        //if (quads == null)
        //    return;
        //var minDis = float.MaxValue;
        //var minInd = 0;
        //int i = 0;
        //foreach(var q in quads)
        //{
        //    var dis = Vector3.Distance(Controller.transform.position, q.transform.position);
        //    if (dis < minDis)
        //    {
        //        minInd = i;
        //        minDis = dis;
        //    }
        //    quads[i].GetComponent<MeshRenderer>().material.color = Color.blue;
        //    i++;
        //}
        //currentQuad = minInd;
        //quads[minInd].GetComponent<MeshRenderer>().material.color = Color.green;
    }
}
