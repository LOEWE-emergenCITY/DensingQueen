using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Condition;

public class Logging : MonoBehaviour
{

    public TransformLogging transformLogging;

    public InVolumeCheck volumeCheck;

    private INPUT input;
    private int currentRep;
    private int currentPart;
    private Condition currentCondition;

    private bool isLogging = false;

    private bool correctQuadChosen = false;
    // Start is called before the first frame update
    void Start()
    {
        ConditionSelection.OnConditionLoaded += StartLogging;
        ConditionSetup.OnConditionStarted += ConditionSetup_OnConditionStarted;
        ConditionSetup.OnConditionFinished += StopLogging;

        ConditionSetup.OnMissedClick += () => { missedClick = true; Debug.Log("MISSED CLICK"); };
        ConditionSetup.OnIncorrectClick += () => { wrongClick = true; Debug.Log("WRONG CLICK"); };
        ConditionSetup.OnCorrentClick += (float t) => { CorrectClick(t); Debug.Log("CORRECT CLICK"); };

        ConditionSetup.OnQuadrantChosen += ConditionSetup_OnQuadrantChosen;

    }

    private void ConditionSetup_OnConditionStarted(GameObject t)
    {
        target = t;
        StartLogging();
        isLogging = true;
        transformLogging.initiateLogging(currentPart, GetTransformFileName(),target);
    }

    private void ConditionSetup_OnQuadrantChosen(bool orrect, float t)
    {
        Debug.Log("correct quadrant " + orrect);
        TCT = t;
        correctQuadChosen = orrect;
    }

    private void StopLogging(float time)
    {
        isLogging = false;
        WrapLogging(time);
        transformLogging.wrapLogging();
    }

    private void StartLogging(Condition condition, int id, int rep)
    {
        Reset();

        currentCondition = condition;
        currentPart = id;
        input = condition.input;
        currentTask = condition.task;
        currentRep = rep;
    }

    public void Reset()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isLogging)
        {
            DoLogging();
        }
    }
    public GameObject hmd;
    public GameObject left;
    public GameObject right;

    GameObject target;

    float hmdRot = 0;
    float hmdPos = 0;

    float leftRot = 0;
    float leftPos = 0;

    float rightRot = 0;
    float rightPos = 0;

    Vector3 hmdPosOld;
    Vector3 leftPosOld;
    Vector3 rightPosOld;

    Quaternion hmdRotOld;
    Quaternion leftRotOld;
    Quaternion rightRotOld;

    float correctClick;

    bool missedClick = false;

    bool wrongClick = false;

    string currentFileName;

    float timeInVolume = 0f;
    int numMissedClicks = 0;
    int numWrongClicks = 0;

    float TCT = 0;

    bool currentlyInVolume = false;
    float inVolumeStartTime = 0;

    TASK currentTask;

    // Start is called before the first frame update

    public void StartLogging()
    {
        string headerCont = "time,hmdPos,leftPos,rightPos,hmdRot,leftRot,rightRot,correctClickWithTime,isInVolume,wrongClick,headTargetDistance,targetInCamFrustum";
        string headerSingular = "hmdPosSum,leftPosSum,rightPosSum,hmdRotSum,leftRotSum,rightRotSum,numWrongClicks,timeInVolume,totaltime";

        correctQuadChosen = false;

        hmdRot = 0;
        hmdPos = 0;

        leftRot = 0;
        leftPos = 0;

        rightRot = 0;
        rightPos = 0;

        timeInVolume = 0f;
        numMissedClicks = 0;
        numWrongClicks = 0;

        TCT = 0;

        switch (currentTask)
        {
            // special logging
            case TASK.FOLLOW:
                {
                    headerCont = headerCont + ",checkpointMissed";
                    headerSingular = headerSingular + ",missedCheckpoints";
                    break;
                }

            case TASK.TRACK:
                {
                    headerCont = headerCont + ",changeMissed";
                    headerSingular = headerSingular + ",missedChanges";
                    break;
                }
            case TASK.FIND:
                {
                    headerCont = headerCont + "";
                    headerSingular = headerSingular + ",TCT,success";
                    break;
                }
            default:
                {
                    Debug.Log("What? Switch broke!");
                    break;
                }
                // general logging
        }

        if (!Directory.Exists(currentPart.ToString()))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/" + currentPart.ToString() + "/");
        }
        File.WriteAllText(GetContFileName(), headerCont);
        File.WriteAllText(GetFinalFileName(), headerSingular);
    }


    public void DoLogging()
    {
        string time = Time.time.ToString();



        float hmdRotDif = Quaternion.Angle(hmd.transform.rotation, hmdRotOld);
        float leftRotDif = Quaternion.Angle(left.transform.rotation, leftRotOld);
        float rightRotDif = Quaternion.Angle(right.transform.rotation, rightRotOld);

        hmdRot += hmdRotDif;
        leftRot += leftRotDif;
        rightRot += rightRotDif;

        hmdRotOld = hmd.transform.rotation;
        leftRotOld = left.transform.rotation;
        rightRotOld = right.transform.rotation;

        float hmdPosDif = Vector3.Distance(hmd.transform.position, hmdPosOld);
        float leftPosDif = Vector3.Distance(left.transform.position, leftPosOld);
        float rightPosDif = Vector3.Distance(right.transform.position, rightPosOld);

        hmdPos += hmdPosDif;
        leftPos += leftPosDif;
        rightPos += rightPosDif;

        hmdPosOld = hmd.transform.position;
        leftPosOld = left.transform.position;
        rightPosOld = right.transform.position;

        // add data coming from the testing software
        bool targetInFrustum = false;
        Plane[] planes = null;
        planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        targetInFrustum = GeometryUtility.TestPlanesAABB(planes, target.GetComponent<Renderer>().bounds);    
        string contEntry = time + "," + hmdPosDif.ToString() + "," + leftPosDif.ToString() + "," + rightPosDif.ToString() + "," + hmdRotDif.ToString()
            + "," + leftRotDif.ToString() + "," + rightRotDif.ToString() + "," + correctClick.ToString() + "," + IsInVolume().ToString() + "," + wrongClick.ToString()+","+Vector3.Distance(hmdPosOld,target.transform.position).ToString()+","+ targetInFrustum.ToString();

        switch (currentTask)
        {
            // special logging
            case TASK.FOLLOW:
                {
                    contEntry += "," + missedClick.ToString();
                    break;
                }

            case TASK.TRACK:
                {
                    contEntry += "," + missedClick.ToString();
                    break;
                }
            case TASK.FIND:
                {
                    break;
                }
            default:
                {
                    Debug.Log("What? Switch broke!");
                    break;
                }
                // general logging
        }
        if (wrongClick)
        {
            numWrongClicks++;
        }
        if (missedClick)
        {
            numMissedClicks++;
        }
        if (IsInVolume())
        {
            //if (!currentlyInVolume)
            //{
            //    currentlyInVolume = true;
            //    inVolumeStartTime = Time.time;
            //}
            timeInVolume += Time.deltaTime;
        }
        else
        {
            //if (currentlyInVolume)
            //{
            //    currentlyInVolume = false;
            //    timeInVolume += Time.time - inVolumeStartTime;
            //}
        }

        wrongClick = false;
        missedClick = false;
        correctClick = 0f;
        File.AppendAllText(GetContFileName(), "\n"+contEntry);
    }

    public void WrapLogging(float time)
    {
        Debug.Log("Wrap final");
        string wrapEntry = hmdPos.ToString() + "," + leftPos.ToString() + "," + rightPos.ToString() + "," + hmdRot.ToString() + "," + leftRot.ToString()
            + "," + rightRot.ToString() + "," + numWrongClicks.ToString() + "," + timeInVolume.ToString()+","+time.ToString();

        switch (currentTask)
        {
            // special logging
            case TASK.FOLLOW:
                {
                    wrapEntry += "," + numMissedClicks.ToString();
                    break;
                }

            case TASK.TRACK:
                {
                    wrapEntry += "," + numMissedClicks.ToString();
                    break;
                }
            case TASK.FIND:
                {
                    wrapEntry += "," + TCT.ToString() + ","+correctQuadChosen;
                    break;
                }
            default:
                {
                    Debug.Log("What? Switch broke!");
                    break;
                }
                // general logging
        }
        File.AppendAllText(GetFinalFileName(), "\n" + wrapEntry);
    }

    void OnGUI()
    {
        GUI.backgroundColor = IsInVolume() ? Color.green : Color.red;
        GUI.Button(new Rect(600, 25, 200, 50),"");
    }
    public void CorrectClick(float timing)
    {
        correctClick = timing;
    }

    public void MissedClick()
    {

    }

    public void IncorrectClick()
    {

    }

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }

    bool IsInVolume()
    {
        if (target == null)
            return false;
        return volumeCheck.IsInVolume(target, input);
    }

    public void SetTCT(float newTCT)
    {
        TCT = newTCT;
    }

    private string GetContFileName()
    {
        return Application.persistentDataPath + "/" + currentPart.ToString() + "/" + currentCondition.ToString() +  ".csv";
    }
    private string GetFinalFileName()
    {
        return Application.persistentDataPath + "/" + currentPart.ToString() + "/" + currentCondition.ToString() +  "_final.csv";
    }
    private string GetTransformFileName()
    {
        return Application.persistentDataPath + "/" + currentPart.ToString() + "/" + currentCondition.ToString() + "_transforms.csv";
    }

    // Follow logging

    // continous
    // wrong click
    // when in volume
    // correct click  with time

    // singular

    // time in volume
    // wrong click

    // checkpoints missed



    // rotation total
    // motion total

    // Track logging

    // continous

    // when in volume

    // singular


    // time in volume
    // wrong click
    // correct click  with time
    // changes missed
    // rotation total
    // motion total

    // Find logging

    // continous

    // when in volume

    // singular


    // time in volume
    // first time in volume
    // wrong click
    // correct click  with time
    // changes missed
    // rotation total
    // motion total
}
