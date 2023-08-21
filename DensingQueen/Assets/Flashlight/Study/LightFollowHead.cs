using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFollowHead : MonoBehaviour
{

    public GameObject Head;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var rot = Head.transform.eulerAngles;
        rot.x = 14.0f;
        rot.z = 0.0f;
        this.transform.eulerAngles = rot;
    }
}
