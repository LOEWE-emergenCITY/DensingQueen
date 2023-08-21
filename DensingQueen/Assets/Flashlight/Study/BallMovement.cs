using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    public Collider MoveSpace;

    // Start is called before the first frame update
    void Start()
    {
        MoveSpace = this.transform.parent.GetComponent<Collider>();
        StartCoroutine(MoveToPosition(RandomPointInBounds(MoveSpace.bounds), Random.value * 1 + 2));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    public IEnumerator MoveToPosition(Vector3 position, float timeToMove)
    {
        var currentPos = transform.position;
        var t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / timeToMove;
            transform.position = Vector3.Lerp(currentPos, position, t);
            yield return null;
        }
        StartCoroutine(MoveToPosition(RandomPointInBounds(MoveSpace.bounds), Random.value * 1 + 2));
    }
}
