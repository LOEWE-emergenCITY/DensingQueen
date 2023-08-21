using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public static event Action<int> OnCheckpointEntered;
    public static event Action<int> OnCheckpointLeft;

    public bool currentCheckpoint = false;

    public int ID = -1;

    public bool entered = false;
    public float timeSinceEnter = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (entered)
        {
            timeSinceEnter += Time.deltaTime;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (currentCheckpoint)
        {
            OnCheckpointEntered?.Invoke(ID);
            entered = true;
            timeSinceEnter = 0.0f;
        }
    }

    //When the Primitive exits the collision, it will change Color
    private void OnTriggerExit(Collider other)
    {
        if (currentCheckpoint)
        {
            OnCheckpointLeft?.Invoke(ID);
        }
        entered = false;
    }

    internal void OnClicked()
    {
        currentCheckpoint = false;
        GetComponent<MeshRenderer>().enabled = false;
    }
}
