using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class MovementOfi : MonoBehaviour
{
    public static event System.Action OnMovementFinished;

    private int initialTrajectory;
    private int Trajectory;
    private float MinDistanceToBorders;
    private int NbMainPoints;
    private float DistanceBetweenMainPoints;

    // values Trajectory 0
    private List<Vector3> PointsPath = new List<Vector3>();
    private int IdCurrentPoint = 0;
    private List<int> curveOrientations = new List<int>(); // PointsPath.count = CurveOrientations.count + 1 
    // 6 possible values for the orientation
    // 0 -x; 1 +x; 2 -y; 3 +y; 4 -z; 5 +z
    // -1 segment;
    private float CurrentCircleRadius;
    private float Speed0;
    private float epsilon = 0.01f;

    private List<Vector3> AllPoints = new List<Vector3>();
    private int NbPointsEachCurve;
    private Vector3 point;
    private Vector3 nextPoint;
    private Vector3 position;
    private float degrees;
    private Vector3 positionOnCurve;
    private int concentrationPoints = 50;
    private Vector3 translationDirection;
    private float changePhase;

    // values Trajectory 1 and 2 
    float circleRadius = 0.2f;
    float Speed1 = 1;

    public Checkpoint[] Checkpoints;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public bool IsInPath(GameObject g)
    {
        bool res = false;
        for(int i = currentPoint; i < AllPoints.Count; i++)
        {
            var v = AllPoints[i];
            if (Vector3.Distance(g.transform.position, v) < 0.1f)
            {
                Debug.Log("is in path!!!");
                return true;
            }
        }
        return false;
    }

    public void Init()
    {
        initialTrajectory = 0;// GetComponentInParent<ThreeTests>().Trajectory;
        float MinSpeed = 0.14f;//GetComponentInParent<ThreeTests>().MinSpeed;
        float MaxSpeed = 0.14f;//GetComponentInParent<ThreeTests>().MaxSpeed / 2;
        Speed0 = Random.Range(MinSpeed, MaxSpeed);
        MinDistanceToBorders = 0.1f;// GetComponentInParent<ThreeTests>().MinDistanceToBorders;
        NbMainPoints = 15;//12;// GetComponentInParent<ThreeTests>().NbMainPoints;
        DistanceBetweenMainPoints = 0.4f;// GetComponentInParent<ThreeTests>().DistanceBetweenMainPoints;
        DefineTrajectory0();

        PlaceCheckpoints(Checkpoints);

        this.transform.position = AllPoints[0];
    }

    // Update is called once per frame
    void Update()
    {
        //Trajectory = GetComponentInParent<ThreeTests>().Trajectory;
        //AdjustTrajectory();
        //FollowTrajectory(Trajectory);
        DoStep();
    }


    private int currentPoint = 0;
    private void DoStep()
    {
        if(currentPoint > AllPoints.Count - 1)
        {
            OnMovementFinished?.Invoke();
            gameObject.SetActive(false);
            return;
        }
        var dis = Vector3.Distance(this.transform.position, AllPoints[currentPoint]);
        var dir = (AllPoints[currentPoint] - this.transform.position).normalized;
        var step = dir * Time.deltaTime * Speed0;
        if (step.magnitude < dis)
        {
            this.transform.Translate(step);
            dis = Vector3.Distance(this.transform.position, AllPoints[currentPoint]);
            if(dis < 0.001f)
            {
                currentPoint++;
            }
        }
        else
        {
            this.transform.position = AllPoints[currentPoint];
            currentPoint++;
            DoStep();
        }
    }

    void AdjustTrajectory()
    {
        if (initialTrajectory != Trajectory)
        {
            initialTrajectory = Trajectory;
            if (Trajectory == 0)
            {
                transform.position = transform.parent.position + PointsPath[0];
                IdCurrentPoint = 0;
            }
        }
    }
    void FollowTrajectory(int Trajectory)
    {
        if (Trajectory == 0)
        {
            GoToNextPoint();
            CheckAndCorrectPosition();
        }


        if (Trajectory == 1) // horizontal circle
        {
            transform.localPosition = new Vector3(circleRadius * Mathf.Cos(Speed1 * Time.time), 0, circleRadius * Mathf.Sin(Speed1 * Time.time));
        }

        if (Trajectory == 2) // little eights
        {
            transform.position = transform.parent.position + new Vector3(circleRadius * Mathf.Cos(Speed1 * Time.time), 0, circleRadius * Mathf.Sin(Speed0 * Time.time));
        }
    }

    void DefineTrajectory0()
    {
        
        CreateRandomMainPoints();
        CreateRandomCurveOrientations();
        

        /*
        // coordinates (x,y,z) with x,y,z € ]-0.5, 0.5[ => Size of the box
        PointsPath.Add(new Vector3(0, 0, 0));
        curveOrientations.Add(1);
        PointsPath.Add(new Vector3(0, 0.4f, 0));
        curveOrientations.Add(1);
        PointsPath.Add(new Vector3(0, -0.3f, 0));
        curveOrientations.Add(2);
        PointsPath.Add(new Vector3(-0.4f, -0.3f, 0));
        curveOrientations.Add(5);
        PointsPath.Add(new Vector3(0.4f, -0.3f, 0));
        curveOrientations.Add(0);
        PointsPath.Add(new Vector3(0.4f, 0.4f, 0));
        curveOrientations.Add(2);
        PointsPath.Add(new Vector3(-0.4f, 0.4f, 0));
        */

        /*
        // Debug Path
        // x movements
        PointsPath.Add(new Vector3(2.0f, 2, 0));
        curveOrientations.Add(0);
        PointsPath.Add(new Vector3(2.4f, 2, 0));
        curveOrientations.Add(1);
        PointsPath.Add(new Vector3(3.0f, 2, 0));
        curveOrientations.Add(2);
        PointsPath.Add(new Vector3(3.3f, 2, 0));
        curveOrientations.Add(3);
        PointsPath.Add(new Vector3(4.0f, 2, 0));
        curveOrientations.Add(-1);
        PointsPath.Add(new Vector3(4.0f, 2, 2));
        curveOrientations.Add(0);
        PointsPath.Add(new Vector3(3.5f, 2, 2));
        curveOrientations.Add(1);
        PointsPath.Add(new Vector3(2.8f, 2, 2));
        curveOrientations.Add(2);
        PointsPath.Add(new Vector3(2.3f, 2, 2));
        curveOrientations.Add(3);
        PointsPath.Add(new Vector3(2.0f, 2, 2));
        // y movements
        curveOrientations.Add(-1);
        PointsPath.Add(new Vector3(1, 2.0f, 0));
        curveOrientations.Add(4);
        PointsPath.Add(new Vector3(1, 2.4f, 0));
        curveOrientations.Add(5);
        PointsPath.Add(new Vector3(1, 3.0f, 0));
        curveOrientations.Add(6);
        PointsPath.Add(new Vector3(1, 3.3f, 0));
        curveOrientations.Add(7);
        PointsPath.Add(new Vector3(1, 4.0f, 0));
        curveOrientations.Add(-1);
        PointsPath.Add(new Vector3(1, 4.0f, -2));
        curveOrientations.Add(4);
        PointsPath.Add(new Vector3(1, 3.5f, -2));
        curveOrientations.Add(5);
        PointsPath.Add(new Vector3(1, 2.8f, -2));
        curveOrientations.Add(6);
        PointsPath.Add(new Vector3(1, 2.3f, -2));
        curveOrientations.Add(7);
        PointsPath.Add(new Vector3(1, 2.0f, -2));
        // z movements
        curveOrientations.Add(-1);
        PointsPath.Add(new Vector3(0, 2, 2.0f));
        curveOrientations.Add(8);
        PointsPath.Add(new Vector3(0, 2, 2.4f));
        curveOrientations.Add(9);
        PointsPath.Add(new Vector3(0, 2, 3.0f));
        curveOrientations.Add(10);
        PointsPath.Add(new Vector3(0, 2, 3.3f));
        curveOrientations.Add(11);
        PointsPath.Add(new Vector3(0, 2, 4.0f));
        curveOrientations.Add(-1);
        PointsPath.Add(new Vector3(-2, 2, 4.0f));
        curveOrientations.Add(8);
        PointsPath.Add(new Vector3(-2, 2, 3.5f));
        curveOrientations.Add(9);
        PointsPath.Add(new Vector3(-2, 2, 2.8f));
        curveOrientations.Add(10);
        PointsPath.Add(new Vector3(-2, 2, 2.3f));
        curveOrientations.Add(11);
        PointsPath.Add(new Vector3(-2, 2, 2.0f));
        */

        CreateAllPoints();
        transform.position = AllPoints[0];
    }

    void CreateRandomMainPoints()
    {
        int i = 0;
        if (NbMainPoints < 1)
        {
            print("Change the value of NbMainPoints in the editor");
            return;
        }
        while (PointsPath.Count != NbMainPoints)
        {
            float dist = 0.5f - MinDistanceToBorders;
            if (PointsPath.Count == 0)
            {
                float Xcomponent = Random.Range(-dist, dist);
                float Ycomponent = Random.Range(-dist, dist);
                float Zcomponent = Random.Range(-dist, dist);
                Vector3 newPoint = new Vector3(Xcomponent, Ycomponent, Zcomponent);
                PointsPath.Add(newPoint);;
                //print("Point0 " + newPoint);
            }
            else
            {
                if (i > 500)
                {
                    print("breakLoop");
                    return;
                }

                float IDcomponent = Random.Range(0, 3);
                Vector3 vdirection = Vector3.zero;
                if (IDcomponent == 0) vdirection = Vector3.right;
                if (IDcomponent == 1) vdirection = Vector3.up;
                if (IDcomponent == 2) vdirection = Vector3.forward;
                float IDcomponentVariation = Random.Range(0, 2); // 0 = '-' ; 1 = '+'
                //float Deltacomponent = Random.Range(-1f, 1f);
                float Deltacomponent = DistanceBetweenMainPoints;
                Vector3 newPoint = PointsPath[PointsPath.Count - 1] + Deltacomponent * vdirection *(-1 + 2 * IDcomponentVariation);
                //print("CandidatnewPoint " + newPoint);
                if (Mathf.Abs(newPoint.x) < dist && Mathf.Abs(newPoint.y) < dist && Mathf.Abs(newPoint.z) < dist) // inside the cube 1x1x1
                {
                    if(PointsPath.Count <= 1)
                    {
                        PointsPath.Add(newPoint);
                        //print("newPoint " + newPoint);
                    }
                    if(PointsPath.Count > 1 && newPoint != PointsPath[PointsPath.Count - 1] && newPoint != PointsPath[PointsPath.Count - 2])
                    {
                        PointsPath.Add(newPoint);
                        //print("newPoint " + newPoint);
                    }
                }
                i++;
            }
        }
    }

    void CreateRandomCurveOrientations()
    {
        for (int i=0; i < PointsPath.Count - 1; i++)
        {
            int typeCurve = Random.Range(0, 4);
            //print("typecurve " + typeCurve);
            if (PointsPath[i + 1].x - PointsPath[i].x != 0) curveOrientations.Add(typeCurve);
            else if (PointsPath[i + 1].y - PointsPath[i].y != 0) curveOrientations.Add(4 + typeCurve);
            else if (PointsPath[i + 1].z - PointsPath[i].z != 0) curveOrientations.Add(8 + typeCurve);
            //print("addedOrientation " + curveOrientations[curveOrientations.Count - 1]);
        }
    }

    void CreateAllPoints()
    {
        for (int u = 0; u < PointsPath.Count - 1; u++)
        {
            AllPoints.Add(transform.position + PointsPath[u]);
            point = PointsPath[u];
            nextPoint = PointsPath[u + 1];
            CurrentCircleRadius = ((nextPoint - point) / 2).magnitude;
            NbPointsEachCurve = (int) (concentrationPoints * CurrentCircleRadius);
            if (curveOrientations[u] != -1)
            {
                for (int v = 0; v < NbPointsEachCurve; v++)
                {
                    degrees = ((float)v) / NbPointsEachCurve * Mathf.PI;
                    CalculateChangePhase();
                    CalculatePositionOnCurve(u);
                    position = transform.position + point + (nextPoint - point) / 2 + positionOnCurve;
                    AllPoints.Add(position);
                }
            }
        }
        AllPoints.Add(transform.position + PointsPath[PointsPath.Count - 1]);
        AllPoints.Add(transform.position + PointsPath[0]); // loop
    }

    void CalculateChangePhase()
    {
        Vector3 difference = nextPoint - point;
        if (Vector3.Dot(difference, Vector3.forward) + Vector3.Dot(difference, Vector3.right) + Vector3.Dot(difference, Vector3.up) > 0) changePhase = Mathf.PI;
        else changePhase = 0;
    }

    void CalculatePositionOnCurve(int u)
    {
        // x diff
        if (curveOrientations[u] == 0) positionOnCurve = new Vector3(CurrentCircleRadius * Mathf.Cos(degrees + changePhase), 0, CurrentCircleRadius * Mathf.Sin(degrees + changePhase));
        if (curveOrientations[u] == 1) positionOnCurve = new Vector3(CurrentCircleRadius * Mathf.Cos(degrees + changePhase), 0, -CurrentCircleRadius * Mathf.Sin(degrees + changePhase));
        if (curveOrientations[u] == 2) positionOnCurve = new Vector3(CurrentCircleRadius * Mathf.Cos(degrees + changePhase), CurrentCircleRadius * Mathf.Sin(degrees + changePhase), 0);
        if (curveOrientations[u] == 3) positionOnCurve = new Vector3(CurrentCircleRadius * Mathf.Cos(degrees + changePhase), -CurrentCircleRadius * Mathf.Sin(degrees + changePhase), 0);

        // y diff
        if (curveOrientations[u] == 4) positionOnCurve = new Vector3(0, CurrentCircleRadius * Mathf.Cos(degrees + changePhase), CurrentCircleRadius * Mathf.Sin(degrees + changePhase));
        if (curveOrientations[u] == 5) positionOnCurve = new Vector3(0, CurrentCircleRadius * Mathf.Cos(degrees + changePhase), -CurrentCircleRadius * Mathf.Sin(degrees + changePhase));
        if (curveOrientations[u] == 6) positionOnCurve = new Vector3(CurrentCircleRadius * Mathf.Sin(degrees + changePhase), CurrentCircleRadius * Mathf.Cos(degrees + changePhase), 0);
        if (curveOrientations[u] == 7) positionOnCurve = new Vector3(-CurrentCircleRadius * Mathf.Sin(degrees + changePhase), CurrentCircleRadius * Mathf.Cos(degrees + changePhase), 0);

        // z diff
        if (curveOrientations[u] == 8) positionOnCurve = new Vector3(CurrentCircleRadius * Mathf.Sin(degrees + changePhase), 0, CurrentCircleRadius * Mathf.Cos(degrees + changePhase));
        if (curveOrientations[u] == 9) positionOnCurve = new Vector3(-CurrentCircleRadius * Mathf.Sin(degrees + changePhase), 0, CurrentCircleRadius * Mathf.Cos(degrees + changePhase));
        if (curveOrientations[u] == 10) positionOnCurve = new Vector3(0, CurrentCircleRadius * Mathf.Sin(degrees + changePhase), CurrentCircleRadius * Mathf.Cos(degrees + changePhase));
        if (curveOrientations[u] == 11) positionOnCurve = new Vector3(0, -CurrentCircleRadius * Mathf.Sin(degrees + changePhase), CurrentCircleRadius * Mathf.Cos(degrees + changePhase));

    }

    void GoToNextPoint()
    {
        if (IdCurrentPoint < AllPoints.Count - 1)
        {
            translationDirection = (AllPoints[IdCurrentPoint + 1] - AllPoints[IdCurrentPoint]).normalized;
            if ((transform.position - AllPoints[IdCurrentPoint + 1]).magnitude > epsilon)
            {
                transform.Translate(translationDirection * Speed0 * Time.deltaTime);
            }
            else
            {
                IdCurrentPoint += 1;
            }
        }
        else
        {
            //IdCurrentPoint = 0;
            OnMovementFinished?.Invoke();
            gameObject.SetActive(false);
        }
    }

    void CheckAndCorrectPosition()
    {
        if(IdCurrentPoint < AllPoints.Count - 1 && (AllPoints[IdCurrentPoint + 1] - transform.position).magnitude > (AllPoints[IdCurrentPoint + 1] - AllPoints[IdCurrentPoint]).magnitude * 1.05)
        {
            var temp = transform.position;
            transform.position = AllPoints[IdCurrentPoint];
            Debug.Log("DASKHFLKSJHvkjrkjb      " + Vector3.Distance(temp,transform.position));
        }
    }

    public void PlaceCheckpoints(Checkpoint[] checkpoints)
    {
        int s = checkpoints.Length;
        float step = AllPoints.Count / (float)(s);
        float variance = 0;// step * 0.1f;
        for(int i = 0; i < s; i++)
        {
            int index = (int) (((float)i+0.8f) * (step - variance * Random.value));
            checkpoints[i].transform.position = AllPoints[index-1];
        }
    }

    private void OnDrawGizmos()
    {
        //for (int u = 0; u < AllPoints.Count; u++)
        //{
        //    Gizmos.color = new Color(1f, ((float)u) / ((float)AllPoints.Count), 1f);
        //    Gizmos.DrawSphere(AllPoints[u], 0.01f);
        //}
    }

}
