using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanetSideStatus : MonoBehaviour {
    GameObject Rost;
    Image unitDetails;
    Text atk;
    Text mov;
    Text vis;

    GameObject PlanetDeets;
    Image PlanetDImg;
    Unit u;
    MouseController mouse;
    HexMap hexMap;
    // Use this for initialization
    void Start () {
        //Rost = GameObject.Find("TeamInfo");
        //Roster=  GameObject.Find("TeamRosPan").GetComponent<Image>();   //GetComponentInChildren<Image>();
        PlanetDeets = GameObject.Find("PlanetDropDown");//.GetComponent<Image>();
        PlanetDImg = GameObject.Find("PlanetDeetsImg").GetComponent<Image>();
        unitDetails = GameObject.Find("UnitDetails").GetComponent<Image>();
        hexMap = HexMap.FindObjectOfType<HexMap>();
        //atk= GameObject.Find("atk").GetComponent<Text>();
        //mov = GameObject.Find("mov").GetComponent<Text>();
        //vis = GameObject.Find("vis").GetComponent<Text>();

        PlanetDeets.SetActive(false);
        unitDetails.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
        //atk.ToString() = mouse.selectedUnit.Attack;

	}

    public void SelectedUnitDetails()
    {
        //Call animation once i figure that out again. but for now
        if (unitDetails.enabled == true)
        {
            unitDetails.enabled = false;
            //atk.enabled = false;
            //mov.enabled = false;
            //vis.enabled = false;
        }
        else
        {
            unitDetails.enabled = true;
            //atk.enabled = true;
            //mov.enabled = true;
            //vis.enabled = true;
        }

    }
    /*
    public void activateCrewRostUI()
    {
        if(Roster.isActiveAndEnabled)
            Rost.SetActive(false);
        else
            Rost.SetActive(true);
    }
    */
    public void activatePlanetDeetsUI()
    {
        if (PlanetDImg.isActiveAndEnabled)
            PlanetDeets.SetActive(false);
        else
            PlanetDeets.SetActive(true);
    }

    public void standSentry()
    {
        Debug.Log("Called stand");
        u.ClearHexPath();
        mouse.CancelUpdateFunc();
    }

}
