using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedUnitManager : MonoBehaviour
{
    MouseController mouseController;
    HexMap hexmap;
    Vector3 offset;

    Camera mainCam;

    // Use this for initialization
    void Start()
    {
        mouseController = GameObject.FindObjectOfType<MouseController>();
        mainCam = Camera.main;
      
        if (player == null) Debug.Log("playerNotSet!!!!");


        Debug.Log("");
        hexmap = HexMap.FindObjectOfType<HexMap>();

       // mouseController.selectedUnit = hexmap.RUnit;
    }

    public Text PortraitName;
    public Text PortraitTeam;
    //public Text Bot_Deets_UnitName;

    //public Text Bot_Description;
    //public Text Bot_DescriptionCD;

    public Text Planet_Name;
    public Text Planet_Anomaly;
    public Text Planet_Ruins;
    public Text Planet_Hostility;
    //public Text Planet_;

    public Button Harvest;
   // public Button Sentry;
   
    public GameObject player;
         
    // Update is called once per frame
    void Update()
    {
        // Debug.Log("mouse-selectedunit:" + mouseController.selectedUnit.Name);
        try
        {
            PortraitName.text = mouseController.selectedUnit.Name;
            PortraitTeam.text = mouseController.selectedUnit.Team;
        }
        catch { Debug.LogError("slectedUnit==null..probably"); };
        if (hexmap != null)
        {
            Planet_Name.text = "New Caprica";
            Planet_Anomaly.text = "Anomalies Studied: 0/10";
            Planet_Ruins.text = "Ruins Discovered: 0/10";
            //Planet_Hostility.text = "Dosile/Terrirorial";
        }

        /* THE FOLLOWING IS FOR TurnNumbers ON PINK PATH
        if (mouseController.selectedUnit.GetHexPath() == null)
            return;
        Hex[] hexPath = mouseController.selectedUnit.GetHexPath();
        if (hexPath == null)
            TurnsLeft.text = "0";
        else if (hexPath.Length < 4)
            TurnsLeft.text = string.Format(
            "ONLY {0} turns left!",
            hexPath.Length.ToString());// not correct number. 
        else 
            TurnsLeft.text = string.Format(
            "{0} turns left",
            hexPath.Length.ToString());// not correct number. 

        //hexPath.text = hexPath == null ? "0" : hexPath.Length.ToString();
        */
    }

    public void camToUnit()
    {
        Vector3 newPos = mouseController.selectedUnit.Hex.Position();
        //Debug.LogFormat("Pos:{0} Offset:{1}, camera:{2}", newPos.ToString(), offset.ToString(), Camera.main.transform.position.ToString());
        Camera.main.transform.position =  (newPos+new Vector3(0.0f,10.2f,-7.0f)); 
        //Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, newPos + new Vector3(0.0f, 10.2f, -7.0f), Time.deltaTime * 5f);
        
    }

    //FOUND OUT HOW TO CALL GAMEOBJECT> HOW DO WE GET THE UNIT.
    // SOLVED UNIT BY MAKING R/B/Y Unit in HexMap

    public void camToRed()
    {
        //NoLogerNeed to find GameObject
       // GameObject red = GameObject.Find("Red2(Clone)");
        mouseController.selectedUnit = hexmap.RUnit;
        Debug.Log("SelectingUnit: " + mouseController.selectedUnit);
        camToUnit();
    }

    public void camToYellow()
    {

        mouseController.selectedUnit = hexmap.YUnit;
        Debug.Log("SelectingUnit: " + mouseController.selectedUnit);
        camToUnit();

    }

    public void camToBlue()
    {
        //GameObject blu = GameObject.Find("Blue(Clone)"); // NO NEED TO FIND GO 
        mouseController.selectedUnit = hexmap.BUnit;
        Debug.Log("SelectingUnit: " + mouseController.selectedUnit);
        camToUnit();

        //Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, newPos + new Vector3(0.0f, 10.2f, -7.0f), Time.deltaTime * 5f);
    }
// Not yet working...
    public void enableGather()
    {
        Debug.Log("Enable Gather");
        if (Harvest.enabled == false)
        {
            Harvest.enabled = true;
            Harvest.GetComponentInParent<CanvasGroup>().alpha=1;
        }else
            Harvest.enabled = false;
            Harvest.GetComponentInParent<CanvasGroup>().alpha =.5f;

    }

    public void StandSentry()
    {
        mouseController.selectedUnit.ClearHexPath();
       // UpdateHexVisuals();
        //Call update in some way. the pink hex path stays visible for a moment too long
    }
    public void StandSentryHover()
    {
       // Bot_Description.text = "Canceled Units current path. Unit will passivly survey their surroundings.";
       // Bot_DescriptionCD.text = "0 turn cooldown";
    }


}
