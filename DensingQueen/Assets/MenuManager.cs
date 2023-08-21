using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Valve.VR;

public class MenuManager : MonoBehaviour
{
   
    public Panel currentPanel = null;
    //public SteamVR_Action_Boolean TriggerPressed; // flashlightclick
    //private bool IsTriggerPressed;

    private List<Panel> panelHistory = new List<Panel>();

    // Start is called before the first frame update
    void Start()
    {
        //TriggerPressed.AddOnChangeListener(OnTrigger, SteamVR_Input_Sources.RightHand);
        SetupPanels();
    }

    /*
    private void OnTrigger(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
    {
        IsTriggerPressed = newState;
    }
    */

    private void SetupPanels()
    {
        Panel[] panels = GetComponentsInChildren<Panel>();

        foreach (Panel panel in panels)
        {
            panel.Setup(this);
        }

        currentPanel.Show();
    }

    private void Update()
    {
        //Debug.Log("beamSizeActionPlus.state = " +TriggerPressed.state);


    }

    public void GoToPrevious()
    {

    }
    
    public void SetCurrentWithHistory(Panel newPanel)
    {

    }

    private void SetCurrent(Panel newPanel)
    {

    }
}
