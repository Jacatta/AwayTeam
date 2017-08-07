using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GetHarvisted : MonoBehaviour {


    SelectedUnitManager selectedUnit;
   // int health = 100;

    void OnTriggerEnter(Collider other)
    {
        /*
        selectedUnit.enableGather();
        Debug.Log("On trigger happened");
        Destroy(this.gameObject);
        // ResourceProxy = true;
    */
    }

    void death()
    {
      //  health -= 20;
        
        selectedUnit.enableGather(); //Deactivated GatherResourceButton
        Destroy(this.gameObject);
    }

}
