using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementCube : MonoBehaviour
{
    public static event Action<float> OnCorrentClick;
    public static event Action OnIncorrectClick;
    public static event Action OnMissedClick;

    public Texture2D PlusTexture;

    private float Speed;
    private Vector3 direction;
    private bool IsColliding = false;
    private Vector3 normal;
    private int nbCurrentCollisions;

    private float[] TimestampsOfChange = {6.0f,18.0f,30.0f,42.0f,54.0f};
    private float variance = 2.0f;
    private float counter = 0.0f;
    private int nextTimeStamp = 0;

    private float changeCounter = 0.0f;
    private float changeDuration = 5.0f;
    private bool isMarked = false;

    // Start is called before the first frame update
    public void Start()
    {
        float MinSpeed = 0.28f;//GetComponentInParent<ThreeTests>().MinSpeed;
        float MaxSpeed = 0.28f;//GetComponentInParent<ThreeTests>().MaxSpeed;
        float Xcomponent = UnityEngine.Random.Range(-1f, 1f);
        float Ycomponent = UnityEngine.Random.Range(-1f, 1f);
        float Zcomponent = UnityEngine.Random.Range(-1f, 1f);
        direction = new Vector3(Xcomponent, Ycomponent, Zcomponent);
        Speed = UnityEngine.Random.Range(MinSpeed, MaxSpeed);

        ConditionSetup.OnConditionFinished += ConditionSetup_OnConditionFinished;

        for(int i = 0; i < TimestampsOfChange.Length; i++)
        {
            TimestampsOfChange[i] += (UnityEngine.Random.value-0.5f) * 2 * variance;
            Debug.Log("Timestamp " +i + "   " + TimestampsOfChange[i]);
        }
        Change(false);
    }


    private void ConditionSetup_OnConditionFinished(float time)
    {
        if(this != null)
            this.enabled = false;
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

        counter += Time.deltaTime;
        if(nextTimeStamp < TimestampsOfChange.Length && counter > TimestampsOfChange[nextTimeStamp])
        {
            nextTimeStamp++;
            Change(true);
        }
        if (isMarked)
        {
            changeCounter += Time.deltaTime;
            if(changeCounter > changeDuration)
            {
                
                OnMissedClick?.Invoke();
                Change(false);
                changeCounter = 0.0f;
            }
        }
    }

    private void Change(bool t)
    {
        changeCounter = 0.0f;
        isMarked = t;
        if (isMarked)
        {
            GetComponent<MeshRenderer>().material.mainTexture = PlusTexture;
            GetComponent<MeshRenderer>().material.color = Color.white;
        }
        else
        {
            GetComponent<MeshRenderer>().material.mainTexture = null;
            GetComponent<MeshRenderer>().material.color = Color.blue;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "wall")
        {
            IsColliding = true;
            normal += collision.transform.right;
            nbCurrentCollisions += 1;
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        nbCurrentCollisions -= 1;
        if (nbCurrentCollisions == 0) IsColliding = false;
    }

    public void OnClick()
    {
        if (isMarked)
        {
            OnCorrentClick?.Invoke(changeCounter);
            Change(false);
        }
        else
        {
            OnIncorrectClick?.Invoke();
        }
    }
}
