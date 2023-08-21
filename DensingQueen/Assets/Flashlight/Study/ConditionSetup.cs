using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Valve.VR;
using Valve.VR.Extras;
using static Condition;

public class ConditionSetup : MonoBehaviour
{
    public static event Action<GameObject> OnConditionStarted;
    public static event Action<float> OnConditionFinished;

    public static event Action<float> OnCorrentClick;
    public static event Action OnIncorrectClick;
    public static event Action OnMissedClick;

    public static event Action OnStartChoosingQuadrant;
    public static event Action<bool,float> OnQuadrantChosen;

    public SteamVR_Action_Boolean actionMenu; // Grip Button // switchMode
    private bool IsMenu;

    public float BallRad = 0.05f;

    public GameObject Lamp;
    public GameObject CuttingPlaneL;
    public GameObject CuttingPlaneR;
    public GameObject CanvasMenu;

    public Material DecoyMaterial;
    public Material InterestMaterial;
    public Material FindMaterial;

    public Texture2D PlusTexture;

    public GameObject Checkpoints;
    public GameObject Walls;
    public GameObject DecoyObjects;
    private Checkpoint[] checkpoints;
    private int currentCheckpoint = 0;

    public int FindTrackDuration = 180;


    public float MinScale;
    public float MaxScale;


    private GameObject objectOfInterest;

    private TASK currentTask;
    public float conditionTime = 0.0f;

    public bool conditionTimePaused = false;

    
    [ReadOnly(true)]
    public bool conditionRunning = false;

    private bool conditionReady = false;

    public QuadrantChoice quadrantChoice;
    public GameObject[] Quadrants;

    
    public GameObject randomCheckpoint1;
    public GameObject randomCheckpoint2;

    // Start is called before the first frame update
    void Start()
    {
        ConditionSelection.OnConditionLoaded += ConditionSelection_OnConditionStarted;
        

        MovementCube.OnCorrentClick += (f) => { OnCorrentClick?.Invoke(f); };
        MovementCube.OnIncorrectClick += () => { OnIncorrectClick?.Invoke(); };
        MovementCube.OnMissedClick += () => { OnMissedClick.Invoke(); }; 

        QuadrantChoice.OnQuadChosen += QuadrantChoice_OnQuadrantChosen;

        Checkpoint.OnCheckpointLeft += (id) => { 
            OnMissedClick?.Invoke();
            NextCheckpoint();
        };

        MovementOfi.OnMovementFinished += StopCondition;

        //quadrantChoice = FindObjectOfType<QuadrantChoice>();

        //QuadrantChoice.OnQuadrantChosen += QuadrantChoice_OnQuadrantChosen;
        var cps = Checkpoints.GetComponentsInChildren<Checkpoint>();
        checkpoints = new Checkpoint[cps.Length];
        for(int i = 0; i < cps.Length; i++)
        {
            checkpoints[i] = cps[i];
            checkpoints[i].ID = i;
        }
        checkpoints[0].currentCheckpoint = true;
        checkpoints[0].GetComponent<MeshRenderer>().enabled = true;


        Shader.SetGlobalFloat("rad", 0.2f);
        actionMenu.AddOnChangeListener(OnMenu, SteamVR_Input_Sources.Any);
    }

    private void NextCheckpoint()
    {
        checkpoints[currentCheckpoint].currentCheckpoint = false;
        checkpoints[currentCheckpoint].GetComponent<MeshRenderer>().enabled = false;
        currentCheckpoint++;
        if (currentCheckpoint < checkpoints.Length)
        {
            checkpoints[currentCheckpoint].GetComponent<MeshRenderer>().enabled = true;
            checkpoints[currentCheckpoint].currentCheckpoint = true;
        }
        RandomCheckpiints();
    }
    private int iii = 0;
    private void RandomCheckpiints()
    {
        bool correct1 = false;
        bool correct2 = false;
        var ofi = FindObjectOfType<MovementOfi>();
        while (!correct1 || !correct2)
        {
            if (iii++ > 1000)
            {
                iii = 0;
                Debug.LogError("GAVE UP");
                break;
            }
                
            randomCheckpoint1.transform.localPosition = new Vector3(UnityEngine.Random.value-0.5f, UnityEngine.Random.value - 0.5f, UnityEngine.Random.value - 0.5f);
            randomCheckpoint2.transform.localPosition = new Vector3(UnityEngine.Random.value - 0.5f, UnityEngine.Random.value - 0.5f, UnityEngine.Random.value - 0.5f);
            correct1 = !ofi.IsInPath(randomCheckpoint1);
            correct2 = !ofi.IsInPath(randomCheckpoint2);
        }
        randomCheckpoint1.GetComponent<MeshRenderer>().enabled = true;
        randomCheckpoint2.GetComponent<MeshRenderer>().enabled = true;
    }
    
    private void QuadrantChoice_OnQuadrantChosen(int quad, float time)
    {
        var dis = Vector3.Distance(Quadrants[quad].transform.position, objectOfInterest.transform.position);
        for(int i = 0; i < Quadrants.Length; i++)
        {
            if (i == quad)
                continue;
            var d = Vector3.Distance(Quadrants[i].transform.position, objectOfInterest.transform.position);
            if(d < dis)
            {
                OnQuadrantChosen?.Invoke(false, time);
                StopCondition();
                return;
            }
        }
        OnQuadrantChosen?.Invoke(true, time);
        StopCondition();
    }

    private void ConditionSelection_OnConditionStarted(Condition condition, int part, int rep)
    {
        ResetCond();
        CreateDecoyBalls(condition.Density);
        CreateObjectOfInterest(condition.task);
        SetInput(condition.input);
        conditionReady = true;
        EnableSetup(false);
        checkpoints[0].currentCheckpoint = true;
        checkpoints[0].GetComponent<MeshRenderer>().enabled = true;
    }

    private void EnableSetup(bool b)
    {
        switch (currentTask)
        {
            case TASK.FOLLOW:
                objectOfInterest.GetComponent<MovementOfi>().enabled = b;
                break;
            case TASK.TRACK:
                objectOfInterest.GetComponent<MovementCube>().enabled = b;
                break;
            case TASK.FIND:
                objectOfInterest.SetActive(b);
                break;
        }
    }

    private void OnMenu(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
    {
        if(newState)
            OnClick();
    }

    void OnClick()
    {
        if (!conditionRunning)
        {
            if(conditionReady)
                StartConciditon();
            return;
        }
        switch (currentTask)
        {
            case TASK.FOLLOW:
                if(currentCheckpoint >= checkpoints.Length)
                {
                    OnIncorrectClick?.Invoke();
                }
                else if(checkpoints[currentCheckpoint].currentCheckpoint && checkpoints[currentCheckpoint].entered)
                {
                    OnCorrentClick?.Invoke(checkpoints[currentCheckpoint].timeSinceEnter);
                    checkpoints[currentCheckpoint].OnClicked();
                    NextCheckpoint();
                }
                else
                {
                    OnIncorrectClick?.Invoke();
                }
                break;
            case TASK.TRACK:
                objectOfInterest.GetComponent<MovementCube>().OnClick();
                break;
            case TASK.FIND:
                if (quadrantChoice.choosing)
                {
                    //QuadrantChoice_OnQuadrantChosen(quadrantChoice.currentQuad, quadrantChoice.time);
                    //quadrantChoice.Reset();
                }
                else
                {
                    conditionTimePaused = true;
                    quadrantChoice.StartChoosing(Quadrants,conditionTime);
                    OnStartChoosingQuadrant?.Invoke();
                    objectOfInterest.SetActive(false);
                    DecoyObjects.SetActive(false);
                    CuttingPlaneL.SetActive(false);
                    CuttingPlaneR.SetActive(false);
                    Walls.SetActive(false);
                }

                break;
        }
    }

    public void StartConciditon()
    {
        Debug.Log("Starting Condition");
        conditionRunning = true;
        conditionReady = false;
        conditionTimePaused = false;
        EnableSetup(true);
        OnConditionStarted?.Invoke(objectOfInterest);
    }

    public void StopCondition()
    {
        if (!conditionRunning)
        {
            Debug.Log("Cant stop condition when none is running!");
            return;
        }
        Debug.Log("Stopping Condition");
        conditionRunning = false;
        conditionTimePaused = false;

        OnConditionFinished?.Invoke(conditionTime);
        currentCheckpoint = 0;
        conditionTime = 0.0f;
        randomCheckpoint1.GetComponent<MeshRenderer>().enabled = false;
        randomCheckpoint2.GetComponent<MeshRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Shader.SetGlobalFloat("rad", 0.2f);
        if (conditionRunning)
        {
            if(!conditionTimePaused)
                conditionTime += Time.deltaTime;
            switch (currentTask)
            {
                case TASK.FOLLOW:
                    break;
                case TASK.TRACK:
                    if (conditionTime > FindTrackDuration)
                    {
                        StopCondition();
                    }
                    break;
                case TASK.FIND:
                    if (conditionTime > FindTrackDuration)
                    {
                        StopCondition();
                    }
                    break;
            }
        
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnClick();
        }
    }

    public void SetInput(INPUT input)
    {
        switch (input)
        {
            case INPUT.LAMP:
                Lamp.SetActive(true);
                CuttingPlaneL.SetActive(false);
                CuttingPlaneR.SetActive(false);
                break;
            case INPUT.PLANES:
                Lamp.SetActive(false);
                CuttingPlaneL.SetActive(true);
                CuttingPlaneR.SetActive(true);
                break;
            case INPUT.NONE:
                Lamp.SetActive(false);
                CuttingPlaneL.SetActive(false);
                CuttingPlaneR.SetActive(false);
                Shader.SetGlobalFloat("lamp",20.0f);
                break;
        }
    }

    public void CreateDecoyBalls(float density)
    {
        float currentVolume = 0.0f;
        int counter = 0;
        while (currentVolume < density)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.parent = DecoyObjects.transform;
            sphere.transform.position = transform.position;
            sphere.transform.localScale = BallRad * Vector3.one;
            sphere.GetComponent<MeshRenderer>().material = DecoyMaterial;
            sphere.AddComponent<MovementDecoyBall>();
            sphere.name = "Decoy";
            currentVolume += (4.0f / 3.0f) * Mathf.PI * Mathf.Pow(BallRad/2, 3);
            counter++;
        }
        Debug.Log("#Balls: " + counter);
        Walls.SetActive(true);
        DecoyObjects.SetActive(true);
        return;


        
        while (counter < density)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.parent = transform;
            sphere.transform.position = transform.position;
            sphere.transform.localScale = UnityEngine.Random.Range(MinScale, MaxScale) * Vector3.one;
            sphere.GetComponent<MeshRenderer>().material = DecoyMaterial;
            sphere.AddComponent<MovementDecoyBall>();
            sphere.name = "Decoy";
            counter = counter + 1;
        }
    }

    public void CreateObjectOfInterest(TASK task)
    {
        currentTask = task;
        switch (task)
        {
            case TASK.FOLLOW:
                objectOfInterest = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                objectOfInterest.transform.position = transform.position;
                objectOfInterest.GetComponent<MeshRenderer>().material = InterestMaterial;
                objectOfInterest.AddComponent<MovementOfi>().Checkpoints = checkpoints;
                objectOfInterest.GetComponent<MovementOfi>().Init();
                objectOfInterest.GetComponent<Collider>().isTrigger = true;
                Checkpoints.gameObject.SetActive(true);
                RandomCheckpiints();
                foreach (Transform child in transform)
                {
                    if (child.transform.name == "CheckPoints")
                    {
                        child.gameObject.SetActive(true);
                    }
                }
                break;
            case TASK.TRACK:
                objectOfInterest = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                objectOfInterest.transform.position = transform.position;
                objectOfInterest.GetComponent<MeshRenderer>().material = InterestMaterial;
                objectOfInterest.GetComponent<MeshRenderer>().material.color = Color.blue;
                objectOfInterest.AddComponent<MovementCube>().PlusTexture = PlusTexture;
                break;
            case TASK.FIND:
                objectOfInterest = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                float Xcomponent = UnityEngine.Random.Range(-0.3f, 0.3f);
                float Ycomponent = UnityEngine.Random.Range(-0.3f, 0.3f);
                float Zcomponent = UnityEngine.Random.Range(-0.3f, 0.3f);
                Vector3 position = new Vector3(Xcomponent, Ycomponent, Zcomponent);
                objectOfInterest.transform.position = transform.position + position;
                objectOfInterest.GetComponent<MeshRenderer>().material = DecoyMaterial;
                break;
        }
        objectOfInterest.transform.parent = transform;
        objectOfInterest.transform.name = "ObjectOfInterest";
        objectOfInterest.transform.localScale = Vector3.one * BallRad;//UnityEngine.Random.Range(MinScale, MaxScale) * Vector3.one;
        if(task == TASK.FIND)
        {
            objectOfInterest.transform.localScale = Vector3.one * BallRad * 2;
        }
    }

    public void ResetCond()
    {
        DestroyDecoyBalls();
        Checkpoints.SetActive(false);
    }

    void DestroyDecoyBalls()
    {
        foreach (Transform child in transform)
        {           
            if(child.gameObject.name == "Decoy" || child.gameObject.name == "ObjectOfInterest")
                GameObject.Destroy(child.gameObject);
        }
        foreach(Transform child in DecoyObjects.transform)
        {
            if (child.gameObject.name == "Decoy" || child.gameObject.name == "ObjectOfInterest")
                GameObject.Destroy(child.gameObject);
        }
    }
}
