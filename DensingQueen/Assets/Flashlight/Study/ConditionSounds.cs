using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionSounds : MonoBehaviour
{

    public AudioSource conditionFinishedSound;
    // Start is called before the first frame update
    void Start()
    {
        ConditionSetup.OnConditionFinished += ConditionSetup_OnConditionFinished;
    }

    private void ConditionSetup_OnConditionFinished(float t)
    {
        conditionFinishedSound.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
