using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisionPipe : MonoBehaviour
{
    public bool IsColliding { get; private set; } = false;

    //Detect collisions between the GameObjects with Colliders attached
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "walls" || collision.gameObject.name == "KeyboardPlayer" || collision.gameObject.name == "CylinderTestSetup(Clone)")
        {
            IsColliding = true;
        }
    }
}
