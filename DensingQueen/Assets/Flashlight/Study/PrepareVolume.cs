using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrepareVolume : MonoBehaviour
{

    public GameObject fillerPrefab;
    public GameObject targetPrefab;
    public GameObject origin;

    public Vector3Int numObjects;
    public Vector3 objectSize;

    GameObject tempObject;

    public bool doIt;

    public List<Vector3Int> pubTargets;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (doIt)
        {
            doIt = false;
            //SetSingleTarget(new Vector3Int(5,5,5));
            SetMultipleTargets(pubTargets);
        }   
    }

    public void SetMultipleTargets(List<Vector3Int> targets)
    {
        for (int i = 0; i < numObjects.x; i++)
        {
            for (int j = 0; j < numObjects.y; j++)
            {
                for (int k = 0; k < numObjects.z; k++)
                {
                    if (targets.Contains(new Vector3Int(i, j, k)))
                    {
                        tempObject = Instantiate(targetPrefab, origin.transform, false);
                        tempObject.transform.name = "Hier";
                    }
                    else
                    {
                        tempObject = Instantiate(fillerPrefab, origin.transform, false);
                    }
                    tempObject.transform.localPosition = new Vector3(i * objectSize.x, j * objectSize.y, k * objectSize.z);

                }
            }
        }
    }
    public void SetSingleTarget(Vector3Int target)
    {

        for(int i = 0; i < numObjects.x; i++)
        {
            for(int j = 0; j < numObjects.y; j++)
            {
                for(int k = 0; k < numObjects.z; k++)
                {
                    if (new Vector3Int(i, j, k) == target)
                    {
                        tempObject = Instantiate(targetPrefab, origin.transform, false);
                        tempObject.transform.name = "Hier";
                    }
                    else
                    {
                        tempObject = Instantiate(fillerPrefab, origin.transform, false);
                    }
                    tempObject.transform.localPosition = new Vector3(i * objectSize.x, j * objectSize.y, k * objectSize.z);
                    
                }
            }
        }
    }


    public void SetPipes(int numPipes)
    {
        System.Random rand = new System.Random();
        
        float[,,] theVolume = new float[numObjects.x,numObjects.y,numObjects.z];

        for (int i = 0; i < numObjects.x; i++)
        {
            for (int j = 0; j < numObjects.y; j++)
            {
                for (int k = 0; k < numObjects.z; k++)
                {
                    theVolume[i, j, k] = 0;
                }
            }
        }

        for (int num = 0; num < numPipes; num++)
        {
            theVolume[0, (int)rand.NextDouble() * numObjects.y, (int)rand.NextDouble() * numObjects.z] = 1;
            //TODO moar
        }
    }


}
