using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotion : MonoBehaviour {

    // Use this for initialization
    Vector3 oldPosition;
    float moveSpeed = 15f;
    public GameObject player;       //Public variable to store a reference to the player game object
    private Vector3 offset;         //Private variable to store the offset distance between the player and camera
    private Vector3 pos;
    private Vector3 playerPos;

    HexMap hexmap;

    MouseController mouse;

    void Start () {
 
        hexmap = HexMap.FindObjectOfType<HexMap>();

    }
    	
	// Update is called once per frame
	void Update () {

        //CLick and Drag Camera
        //WASD
        //Zoon in and out 

        checkIfCameraMoves();
    }

    public void PanToHex(Hex hex)
    {
        Vector3 temp = hex.Position();
        this.transform.Translate(temp * moveSpeed * Time.deltaTime, Space.World);
    }

    public void PanToAlpha()
    {

        mouse.selectedUnit.Hex.Position();
        transform.position = playerPos + offset;
        //this.transform.Translate(hexPos * moveSpeed * Time.deltaTime);
    }

    public void checkIfCameraMoves()
    {
        if(oldPosition != this.transform.position)
        {

            oldPosition = this.transform.position;

            //prob hexmap will have a dictionary of all these later.
            HexComponent[] hexes = GameObject.FindObjectsOfType<HexComponent>();
            if (hexes != null)
            {
                foreach (HexComponent hex in hexes)
                {
                    hex.updatePosition();
                }
            }
        }
    }
}
