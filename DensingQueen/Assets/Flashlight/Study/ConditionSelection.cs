using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using static Condition;

public class ConditionSelection : MonoBehaviour
{
    private static int[,] BLS = {
        { 1, 2, 9, 3, 8, 4, 7, 5, 6 },
        { 2, 3, 1, 4, 9, 5, 8, 6, 7 },
        { 3, 4, 2, 5, 1, 6, 9, 7, 8 },
        { 4, 5, 3, 6, 2, 7, 1, 8, 9 },
        { 5, 6, 4, 7, 3, 8, 2, 9, 1 },
        { 6, 7, 5, 8, 4, 9, 3, 1, 2 },
        { 7, 8, 6, 9, 5, 1, 4, 2, 3 },
        { 8, 9, 7, 1, 6, 2, 5, 3, 4 },
        { 9, 1, 8, 2, 7, 3, 6, 4, 5 },
        { 6, 5, 7, 4, 8, 3, 9, 2, 1 },
        { 7, 6, 8, 5, 9, 4, 1, 3, 2 },
        { 8, 7, 9, 6, 1, 5, 2, 4, 3 },
        { 9, 8, 1, 7, 2, 6, 3, 5, 4 },
        { 1, 9, 2, 8, 3, 7, 4, 6, 5 },
        { 2, 1, 3, 9, 4, 8, 5, 7, 6 },
        { 3, 2, 4, 1, 5, 9, 6, 8, 7 },
        { 4, 3, 5, 2, 6, 1, 7, 9, 8 },
        { 5, 4, 6, 3, 7, 2, 8, 1, 9 }  };

    private static int[,] INNER_BLS = {
        {0,1,2},
        {1,2,0},
        {2,0,1},
        {2,1,0},
        {0,2,1},
        {1,0,2}
    };

    public static event Action<Condition,int, int> OnConditionLoaded;

    public ConditionSetup Setup;

    public int ParticipantID = -1;

    public float[] Densities;
    public TASK[] Tasks;
    public INPUT[] Inputs;

    [SerializeField]
    public Condition[] conditions;

    public int Repetitions = 3;
    private int currentRep = 0;
    private Condition currentCond;

   

    private int currentCondition = 0;


    // Start is called before the first frame update
    void Start()
    {
        var merged = Densities.SelectMany(x => Inputs, (x, y) => new { x, y });
        int i = 0;
        conditions = new Condition[9];
        foreach (var m in merged)
        {
            int ii = i / 3;
            conditions[i] = new Condition()
            {
                Density = m.x,
                input = m.y
            };
            i++;
            //conditions[i++] = new Condition()
            //{
            //    Density = m.x,
            //    input = m.y,
            //    task = i % 3 == 0 ? TASK.FIND : i % 3 == 1 ? TASK.FOLLOW : TASK.TRACK
            //};
            //conditions[i++] = new Condition()
            //{
            //    Density = m.x,
            //    input = m.y,
            //    task = i % 3 == 0 ? TASK.FIND : i % 3 == 1 ? TASK.FOLLOW : TASK.TRACK
            //};
        }
        //ConditionSetup.OnConditionFinished += ConditionSetup_OnConditionFinished;
    }

    private void ConditionSetup_OnConditionFinished(float time)
    {
        StartCoroutine(WaitThenNextCond());
    }

    private IEnumerator WaitThenNextCond()
    {
        yield return new WaitForSeconds(0.5f);
        NextCondition();
        BuildCondition();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void NextCondition()
    {
        if (Setup.conditionRunning)
        {
            Debug.LogError("Condition still running");
            return;
        }
        if (currentCondition >= conditions.Length)
        {
            Debug.Log("Study Over");
            return;
        }
        if (++currentRep > Repetitions)
        {
            currentCondition++;
            currentRep = 1;
        }
        if (currentCondition >= conditions.Length)
        {
            Debug.Log("Study Over");
            return;
        }
        

        var condID = BLS[ParticipantID, currentCondition];
        condID -= 1;
        //condID *= 3;
        Condition condition = conditions[condID];
        condition.task = Tasks[INNER_BLS[currentCondition%6,currentRep-1]];
        currentCond = condition;

        
    }
    private void BuildCondition()
    {
        if (Setup.conditionRunning)
        {
            Debug.LogError("Condition still running");
            return;
        }
        OnConditionLoaded?.Invoke(currentCond, ParticipantID, 0);
    }

    void OnGUI()
    {
        
        var partIDS = GUI.TextField(new Rect(50, 50, 100, 30), ParticipantID.ToString());
        ParticipantID = int.Parse(partIDS);

        if (GUI.Button(new Rect(200, 50, 100, 25), "Next"))
        {
            NextCondition();
        }
        if (GUI.Button(new Rect(300, 50, 100, 25), "Build"))
        {
            BuildCondition();
        }
        if (GUI.Button(new Rect(500, 50, 100, 25), "Reset"))
        {
            if (!Setup.conditionRunning)
            {
                currentCondition = 0;
                currentRep = 0;
            }
        }
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.red;
        style.fontSize = 20;
        GUI.Label(new Rect(50, 100, 100, 25), (currentCond !=null ? currentCond.ToString() : "None"), style);
        GUI.Label(new Rect(50, 125, 100, 25), "Cond: " + currentCondition + "  "+ currentRep + "/" + Repetitions, style);

        GUI.Label(new Rect(50, 300, 100, 25), "Time: " + Setup.conditionTime, style);
    }

   
}

[Serializable]
public class Condition
{
    public enum INPUT
    {
        LAMP, PLANES, NONE
    }
    public enum TASK
    {
        FOLLOW, TRACK, FIND
    }
    public float Density = 500;
    public INPUT input;
    public TASK task;

    public override string ToString()
    {
        return task+"_"+input + "_" + Density;
    }
}
