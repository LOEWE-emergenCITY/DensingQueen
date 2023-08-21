using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakePipeMaze : MonoBehaviour
{
    public int nb_Pipes;
    public float maxRange = 50;
    public float maxScaleY = 25;

    public GameObject PrefabCylinderTS;
    public GameObject walls;

    public Material red;
    public Material blue;
    public Material green;
    public Material gold;
    
    private float x;
    private float y;
    private float z;
    private float rx;
    private float ry;
    private float rz;
    private float scaleY;
    private Vector3 position;
    private Quaternion rotation;
    private List<Material> colors = new List<Material>();
    private int nb_Pipes_created=0;

    // Start is called before the first frame update
    void Start()
    {
        colors.Add(red);
        colors.Add(blue);
        colors.Add(green);
        colors.Add(gold);
    }

    // Update is called once per frame
    void Update()
    {
        while (nb_Pipes_created != nb_Pipes)
        {
            GameObject cylinder = Instantiate(PrefabCylinderTS, transform);

            x = Random.Range(-maxRange, maxRange);
            y = Random.Range(0, maxRange);
            z = Random.Range(-maxRange, maxRange);
            position = new Vector3(x, y, z);

            rx = Random.Range(-180, 180);
            ry = Random.Range(-180, 180);
            rz = Random.Range(-180, 180);
            rotation.Set(rx, ry, rz, rz);

            scaleY = Random.Range(1, maxScaleY);
            cylinder.transform.localScale += new Vector3(0, scaleY, 0);

            int color = Random.Range(0, colors.Count);
            MeshRenderer meshRenderer = cylinder.GetComponent<MeshRenderer>();
            meshRenderer.material = colors[color];
            cylinder.transform.SetPositionAndRotation(position, rotation);
            nb_Pipes_created++;
        }

        foreach (Transform child in transform)
        {
            bool IsColliding = child.GetComponent<collisionPipe>().IsColliding;
            if (IsColliding)
            {
                Destroy(child.gameObject);
                nb_Pipes_created--;
                break;
            }
        }
    }
}
