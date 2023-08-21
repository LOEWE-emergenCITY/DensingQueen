using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustSpotAngle : MonoBehaviour
{
    public Camera cullingCamera;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Light>().spotAngle = FindObjectOfType<StudyControls>().fov*90;
    }
}
