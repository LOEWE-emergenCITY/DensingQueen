using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParticipantInfoText : MonoBehaviour
{

    private Text text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        ConditionSelection.OnConditionLoaded += ConditionSelection_OnConditionLoaded;
        ConditionSetup.OnConditionStarted += ConditionSetup_OnConditionStarted;
        ConditionSetup.OnConditionFinished += ConditionSetup_OnConditionFinished;
        ConditionSetup.OnStartChoosingQuadrant += ConditionSetup_OnStartChoosingQuadrant;
    }


    private void ConditionSetup_OnStartChoosingQuadrant()
    {
        SetText("Choose in which quadrant the object of interest was");
    }

    private void ConditionSetup_OnConditionFinished(float time)
    {
        SetText("Condition finished. Wait...");
    }

    private void ConditionSetup_OnConditionStarted(GameObject obj)
    {
        SetText("Condition running");
    }

    private void ConditionSelection_OnConditionLoaded(Condition arg1, int arg2, int arg3)
    {
        SetText("Condition loaded. Pull Trigger to start");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetText(string s)
    {
        text.text = s;
    }
}
