using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadarObject
{
    public Image icon { get; set; }
    public GameObject  Player { get; set; }
    
}

public class Radar : MonoBehaviour {

    public GameObject player;
    public Transform playerPos;
    float mapScale = 2.0f;

    public static List<RadarObject> radObjects = new List<RadarObject>();

    Transform FindParent()
    {
        Transform pParent = player.GetComponentInParent<Transform>();
        return pParent;
    }

    public static void RegisterRadarObject(GameObject o, Image i)
    {
        Image image = Instantiate(i);
        radObjects.Add(new RadarObject(){ Player = o, icon = image});
    }

    public static void RemoveRadarObject(GameObject o)
    {
        List<RadarObject> newList = new List<RadarObject>();
        for (int i =0 ; i < radObjects.Count;i++)
        {
            if (radObjects[i].Player == o)
            {
                Destroy(radObjects[i].icon);
                continue;
            }
            else
                newList.Add(radObjects[i]);
        }

        radObjects.RemoveRange(0, radObjects.Count);
        radObjects.AddRange(newList);

    }

    void DrawRadarDots()
    {

        //Debug.Log("player Parent: " + player.transform.parent.name);
        // The comments are my edited lines which runs, but then gives me the issue of not assigned to a GameObject.
        foreach(RadarObject ro in radObjects)
        {

            
            //Vector3 radarPos = (ro.Player.transform.position - player.transform.parent.position);
            Vector3 radarPos = (ro.Player.transform.position - playerPos.transform.position);
            //Vector3 radarPos = (ro.Player.transform.position - FindParent().transform.position);

            //float distToObject = Vector3.Distance(player.transform.parent.position, ro.Player.transform.position) * mapScale;
            float distToObject = Vector3.Distance(playerPos.transform.position, ro.Player.transform.position) * mapScale;
            //float distToObject = Vector3.Distance(FindParent().transform.position, ro.Player.transform.position) * mapScale;

            //float deltay = Mathf.Atan2(radarPos.x, radarPos.z) * Mathf.Rad2Deg - 270 - player.transform.parent.eulerAngles.y;
            float deltay = Mathf.Atan2(radarPos.x, radarPos.z) * Mathf.Rad2Deg-270- playerPos.transform.eulerAngles.y;
            //float deltay = Mathf.Atan2(radarPos.x, radarPos.z) * Mathf.Rad2Deg - 270 - FindParent().transform.eulerAngles.y;

            radarPos.x = distToObject * Mathf.Cos(deltay * Mathf.Deg2Rad) * -1;
            radarPos.z = distToObject * Mathf.Sin(deltay * Mathf.Deg2Rad);

            ro.icon.transform.SetParent(this.transform);
            ro.icon.transform.position = new Vector3(radarPos.x, radarPos.z, 0) + this.transform.position;
        }
    }

	
	// Update is called once per frame
	void Update () {
       
        DrawRadarDots();	
	}
}
