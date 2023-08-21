using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementDecoyBall : MonoBehaviour
{
    
    private float Speed;
    private Vector3 direction;
    private bool IsColliding = false;
    private Vector3 normal;
    private int nbCurrentCollisions;

    // Start is called before the first frame update
    public void Start()
    {
        float MinSpeed = GetComponentInParent<ThreeTests>().MinSpeed;
        float MaxSpeed = GetComponentInParent<ThreeTests>().MaxSpeed;
        float Xcomponent = Random.Range(-1f, 1f);
        float Ycomponent = Random.Range(-1f, 1f);
        float Zcomponent = Random.Range(-1f, 1f);
        direction = new Vector3(Xcomponent, Ycomponent, Zcomponent);
        Speed = Random.Range(MinSpeed, MaxSpeed);
        ConditionSetup.OnConditionFinished += ConditionSetup_OnConditionFinished;
    }

    private void ConditionSetup_OnConditionFinished(float time)
    {
        try
        {
            this.enabled = false;
        }
        catch (System.Exception e)
        {

        }
        
    }

    // Update is called once per frame
    public void Update()
    {
        if (IsColliding)
        {
            direction = Vector3.Reflect(direction, normal).normalized;
            normal = Vector3.zero;
            IsColliding = false;
        }
        transform.Translate(Speed * direction * Time.deltaTime);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "wall")
        {
            //IsColliding = true;
            normal += collision.transform.right;
            //nbCurrentCollisions += 1;
            Bounce();
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        nbCurrentCollisions -= 1; 
        if (nbCurrentCollisions == 0) 
            IsColliding = false;
    }

    void Bounce()
    {
        direction = Vector3.Reflect(direction, normal).normalized;
        normal = Vector3.zero;
        IsColliding = false;
    }
}
