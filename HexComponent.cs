using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexComponent : MonoBehaviour {


    public Hex Hex;

    public HexMap HexMap;

   // public CameraMotion camMan;

    public void updatePosition()
    {
            //Debug.Log(Hex.);
            transform.position = Hex.PositionFromCamera(
            Camera.main.transform.position, // The error is here?! ... is it .. Hex?
            HexMap.NumRows,
            HexMap.NumColumns
            );
    }
}
