using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSetPosition : MonoBehaviour
{
    public GameObject VRCam;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<RectTransform>().SetPositionAndRotation(VRCam.transform.position + 2 * VRCam.transform.forward, VRCam.transform.rotation);
    }
}
