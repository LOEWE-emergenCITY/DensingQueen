using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ThreeTests : MonoBehaviour
{
    public int CuttingPlanesOrFlashlight; // 0 = one cutting plane per hand; 1 = one flashlight with SteamVR actions
    public int IdTest; // 0 1 or 2
    public int Trajectory; // only 0 for the moment // used in MovementOfi.cs on the Object of Interest
    public float MinDistanceToBorders; // [0, 0.5[
    public int NbMainPoints; // reasonnable values [4, 16]
    public float DistanceBetweenMainPoints; // example reasonnable values [0.1 : 0.4]
    public int NbDecoyBalls;
    public float MinSpeed;
    public float MaxSpeed;
    public float MinScale;
    public float MaxScale;
    public Material DecoyMaterial;
    public Material InterestMaterial;
    public GameObject Lamp;
    public GameObject CuttingPlaneL;
    public GameObject CuttingPlaneR;
    public GameObject CanvasMenu;

    private int initialNbDecoyBalls;
    private int initialIdTest;
    private int initialMode;
    private GameObject objectOfInterest;

    public SteamVR_Action_Boolean actionMenu; // Grip Button // switchMode
    private bool IsMenu;


    // Start is called before the first frame update
    void Start()
    {
        actionMenu.AddOnChangeListener(OnMenu, SteamVR_Input_Sources.RightHand);
        CanvasMenu.SetActive(false);
        initialMode = CuttingPlanesOrFlashlight;
        AdjustMode(initialMode);
        CreateDecoyBalls(NbDecoyBalls);
        CreateObjectOfInterest(IdTest);
    }

    // Update is called once per frame
    void Update()
    {
        //print("isMenu " + IsMenu);
        if (IsMenu) CanvasMenu.SetActive(!CanvasMenu.activeSelf);
        if (initialMode != CuttingPlanesOrFlashlight) AdjustMode(CuttingPlanesOrFlashlight);
        if (initialNbDecoyBalls != NbDecoyBalls) AdjustNbDecoyBalls();
        if (initialIdTest != IdTest) AdjustObjectOfInterest();
    }


    private void OnMenu(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
    {
        IsMenu = newState;
    }

    void AdjustMode(int newMode)
    {
        if (newMode == 0) // CuttingPlanes Mode
        {
            Lamp.SetActive(false);
            CuttingPlaneL.SetActive(true);
            CuttingPlaneR.SetActive(true);
        }
        if (newMode == 1) // Flashlight Mode
        {
            Lamp.SetActive(true);
            CuttingPlaneL.SetActive(false);
            CuttingPlaneR.SetActive(false);
        }
        initialMode = newMode;
    }

    void CreateDecoyBalls(int NbBallsToCreate)
    {
        int counter = 0;
        while(counter < NbBallsToCreate)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.parent = transform;
            sphere.transform.position = transform.position;
            sphere.transform.localScale = Random.Range(MinScale, MaxScale) * Vector3.one;
            sphere.GetComponent<MeshRenderer>().material = DecoyMaterial;
            sphere.AddComponent<MovementDecoyBall>();
            counter = counter + 1;
        }
        initialNbDecoyBalls = NbDecoyBalls;
    }

    void DestroyDecoyBalls(int NbBallsToDestroy)
    {
        int counter = 0;
        foreach (Transform child in transform)
        {
            if (counter < NbBallsToDestroy && child.transform.name == "Sphere")
            {
                GameObject.Destroy(child.gameObject);
                counter = counter + 1;
            }
        }
        initialNbDecoyBalls = NbDecoyBalls;
    }

    void AdjustNbDecoyBalls()
    {
        if (NbDecoyBalls > initialNbDecoyBalls) CreateDecoyBalls(NbDecoyBalls - initialNbDecoyBalls);
        else DestroyDecoyBalls(initialNbDecoyBalls - NbDecoyBalls);
    }

    void CreateObjectOfInterest(int IdTest)
    {
        if (IdTest == 0)
        {
            objectOfInterest = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            objectOfInterest.transform.position = transform.position;
            objectOfInterest.GetComponent<MeshRenderer>().material = InterestMaterial;
            objectOfInterest.AddComponent<MovementOfi>();

            foreach (Transform child in transform)
            {
                if (child.transform.name == "CheckPoints")
                {
                    child.gameObject.SetActive(true);
                }
            }


        }
        if (IdTest == 1)
        {
            objectOfInterest = GameObject.CreatePrimitive(PrimitiveType.Cube);
            objectOfInterest.transform.position = transform.position;
            objectOfInterest.GetComponent<MeshRenderer>().material = DecoyMaterial;
            objectOfInterest.AddComponent<MovementDecoyBall>();
        }
        if (IdTest == 2)
        {
            objectOfInterest = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            float Xcomponent = Random.Range(-0.5f, 0.5f);
            float Ycomponent = Random.Range(-0.5f, 0.5f);
            float Zcomponent = Random.Range(-0.5f, 0.5f);
            Vector3 position = new Vector3(Xcomponent, Ycomponent, Zcomponent);
            objectOfInterest.transform.position = transform.position + position;
            objectOfInterest.GetComponent<MeshRenderer>().material = InterestMaterial;
        }
        objectOfInterest.transform.parent = transform;
        objectOfInterest.transform.name = "ObjectOfInterest";
        objectOfInterest.transform.localScale = Random.Range(MinScale, MaxScale) * Vector3.one;
        initialIdTest = IdTest;
    }

    void AdjustObjectOfInterest()
    {
        foreach (Transform child in transform)
        {
            if (child.transform.name == "ObjectOfInterest")
            {
                GameObject.Destroy(child.gameObject);
            }

            if (child.transform.name == "CheckPoints")
            {
                child.gameObject.SetActive(false);
            }
        }
        CreateObjectOfInterest(IdTest);
    }
}
