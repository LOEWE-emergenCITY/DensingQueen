using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.IO;

public class TransformLogging : MonoBehaviour
{
    public GameObject hmd;
    public GameObject left;
    public GameObject right;

    string fileName;
    int participant;

    DataClass dc;

    bool canLog;

    List<Quaternion> hmdQuaternionList = new List<Quaternion>();
    List<Quaternion> leftQuaternionList = new List<Quaternion>();
    List<Quaternion> rightQuaternionList = new List<Quaternion>();

    List<Vector3> hmdPositionList = new List<Vector3>();
    List<Vector3> leftPositionList = new List<Vector3>();
    List<Vector3> rightPositionList = new List<Vector3>();

    List<Vector3> targetPosList = new List<Vector3>();

    private GameObject target;


    // Start is called before the first frame update
    void Start()
    {

    }

    public void initiateLogging(int newParticipant, string newFilename, GameObject t)
    {
        fileName = newFilename;
        participant = newParticipant;
        dc = new DataClass();
        target = t;

        hmdQuaternionList = new List<Quaternion>();
        leftQuaternionList = new List<Quaternion>();
        rightQuaternionList = new List<Quaternion>();

        hmdPositionList = new List<Vector3>();
        leftPositionList = new List<Vector3>();
        rightPositionList = new List<Vector3>();

        targetPosList = new List<Vector3>();

        canLog = true;
    }


    public void wrapLogging()
    {
        canLog = false;

        dc.hmdRotation = hmdQuaternionList.ToArray();
        dc.leftRotation = leftQuaternionList.ToArray();
        dc.rightRotation = rightQuaternionList.ToArray();

        dc.hmdPos = hmdPositionList.ToArray();
        dc.leftPos = leftPositionList.ToArray();
        dc.rightPos = rightPositionList.ToArray();

        dc.targetPos = targetPosList.ToArray();

        var s = JsonUtility.ToJson(dc, true);
        Debug.Log(Application.persistentDataPath + "/" + participant.ToString() + "/");
        if (!Directory.Exists(participant.ToString()))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/" + participant.ToString() + "/");
        }
        File.WriteAllText(fileName, s);
    }

    void FixedUpdate()
    {
        if (canLog)
        {
            hmdQuaternionList.Add(hmd.transform.rotation);
            leftQuaternionList.Add(left.transform.rotation);
            rightQuaternionList.Add(right.transform.rotation);

            hmdPositionList.Add(hmd.transform.position);
            leftPositionList.Add(left.transform.position);
            rightPositionList.Add(right.transform.position);

            targetPosList.Add(target.transform.position);
        }
    }

    [Serializable]
    public class DataClass
    {
        public Quaternion[] hmdRotation;
        public Quaternion[] leftRotation;
        public Quaternion[] rightRotation;
        public Vector3[] hmdPos;
        public Vector3[] leftPos;
        public Vector3[] rightPos;

        public Vector3[] targetPos;

    }


}