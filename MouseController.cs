using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour
{

    GameObject HexSelect;
    HexMap hexMap;

    void Start()
    {
        Update_CurrentFunc = Update_DetectModeStart;

        hexMap = GameObject.FindObjectOfType<HexMap>();

        lineRenderer = transform.GetComponentInChildren<LineRenderer>();

        HexSelect = GameObject.FindGameObjectWithTag("HexSelect");

        selectedUnit = hexMap.StartUnit;

        UnitActions.SetActive(false);

        UnitSelectionPanel.SetActive(true);
        RPortrait.enabled = true;
        BPortrait.enabled = false;
        YPortrait.enabled = false;
    }

    // Unit movement
    Unit __selectedUnit = null;
    public Unit selectedUnit
    {
        get { return __selectedUnit; }
        set
        {
            __selectedUnit = value;
            Debug.Log("found! " + __selectedUnit.Name);
            if (__selectedUnit.Team == "RedTeam")
            {
                RPortrait.enabled = true; 
                BPortrait.enabled = false;
                YPortrait.enabled = false;
             
            }
            else if (__selectedUnit.Team == "BlueTeam")
            {
                RPortrait.enabled = false;
                BPortrait.enabled = true;
                YPortrait.enabled = false;
             
            }
            else if(__selectedUnit.Team == "YellowTeam")
            {
                RPortrait.enabled = false;
                BPortrait.enabled = false;
                YPortrait.enabled = true;
            }
        }
    }

    public GameObject UnitSelectionPanel;
    public GameObject UnitActions;

    public Image RPortrait;
    public Image YPortrait;
    public Image BPortrait;
    // Generic bookkeeping variables


    public Hex hexUnderMouse;
    Hex hexLastUnderMouse;
    Vector3 lastMousePosition;  // From Input.mousePosition

    public GameObject Navi;


    // Camera Dragging bookkeeping variables
    int mouseDragThreshold = 1; // Threshold of mouse movement to start a drag
    Vector3 lastMouseGroundPlanePosition;
    Vector3 cameraTargetOffset;


    public Hex[] hexPath;
    LineRenderer lineRenderer;

    delegate void UpdateFunc();
    UpdateFunc Update_CurrentFunc;

    SelectedUnitManager sum;
    float howLong;

    public LayerMask LayerIDForHexTiles;


    void Update() 
    {
        if (EventSystem.current.IsPointerOverGameObject())  
        {
            //Debug.Log(EventSystem.current);
            if (EventSystem.current.name == "Wait")// NOT REALLY WORKING 
            {
                sum.StandSentryHover();   
            }
            return;
        }

    if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            foreach (Hex h in hexLastUnderMouse.GetNeighbours(1)) //selectedUnit.vision
            {
                GameObject hexGO = hexMap.GetHexGO(h);
                MeshRenderer mr = hexGO.GetComponentInChildren<MeshRenderer>();
                if (mr.material.color == Color.red)
                {
                    mr.material.color = Color.black;
                    h.Elevation = 999;
                }
                else
                    mr.material.color = Color.blue+ (Color.white / 2);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            foreach (Hex h in hexLastUnderMouse.GetNeighbours(2)) //selectedUnit.vision
            {
                GameObject hexGO = hexMap.GetHexGO(h);
                MeshRenderer mr = hexGO.GetComponentInChildren<MeshRenderer>();
                if(mr.material.color==Color.red)
                {
                    mr.material.color = Color.black;
                    h.Elevation = 999;
                }
                else
                mr.material.color = Color.blue + (Color.white / 2);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            foreach (Hex h in hexLastUnderMouse.GetNeighbours(3)) //selectedUnit.vision
            {
                GameObject hexGO = hexMap.GetHexGO(h);
                MeshRenderer mr = hexGO.GetComponentInChildren<MeshRenderer>();
                if (mr.material.color == Color.red)
                {
                    mr.material.color = Color.black;
                    h.Elevation = 999;
                }
                else
                    mr.material.color = Color.blue + (Color.white / 2);
            }
        }

        hexUnderMouse = MouseToHex();
    
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            selectedUnit = null;
            CancelUpdateFunc();
        }

        Update_CurrentFunc();

        // Always do camera zooms (check for being over a scroll UI later)
        //Update_ScrollZoom();

        lastMousePosition = Input.mousePosition;
        if(hexUnderMouse != null && hexUnderMouse != hexLastUnderMouse)
        {
            UnitActions.SetActive(false);
            HexSelect.transform.SetParent(hexUnderMouse.HexComonent.transform);
            HexSelect.transform.localScale = Vector3.one;
            HexSelect.transform.localPosition = Vector3.zero;
            // reposition
        }
        hexLastUnderMouse = hexUnderMouse;

        if (selectedUnit != null)
        {
            DrawPath((hexPath != null) ? hexPath : selectedUnit.GetHexPath());
        }
        else
        {
            DrawPath(null);   // Clear the path display
        }
        selectedHex();
        }


    public void descriptionBox()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        //int layerMask = LayerIDForHexTiles.value;

        if (Physics.Raycast(mouseRay, out hitInfo, Mathf.Infinity))
        {
            // Something got hit
            //Debug.Log( hitInfo.collider.name );
        
        }

    }

    void selectedHex()
    {
        GameObject h = hexMap.GetHexGO(hexUnderMouse);

        if (h != null)
        Navi.transform.position = h.transform.position;
    }

    void DrawPath(Hex[] hexPath)
    {
        if (hexPath == null || hexPath.Length == 0)
        {
            lineRenderer.enabled = false;
            return;
        }
        lineRenderer.enabled = true;

        Vector3[] ps = new Vector3[hexPath.Length];

        for (int i = 0; i < hexPath.Length; i++)
        {
            GameObject hexGO = hexMap.GetHexGO(hexPath[i]);
            ps[i] = hexGO.transform.position + (Vector3.up * 0.1f);
        }

        lineRenderer.positionCount = ps.Length;
        lineRenderer.SetPositions(ps);
    }

   public void CancelUpdateFunc()
    {
        Update_CurrentFunc = Update_DetectModeStart;
       // Navi.GetComponent<MeshRenderer>().material.color = Color.white;
        // Also do cleanup of any UI stuff associated with modes.
        hexPath = null;
        deactivateActions();

    }
    //Event e = Event.current;
    Button buttonToFire;
    void Update_DetectModeStart()
    {

        // ExecuteEvents.Execute(buttonToFire.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);

        if (Input.GetMouseButtonDown(0))// NOTHING REALLY
        {

            // IF LeftClick is down for > 1 second, UnitActions.SetActive(true);

            // Left mouse button just went down.
            // This doesn't do anything by itself, really.
            //Debug.Log("MOUSE DOWN");
        }
        else if (Input.GetMouseButtonUp(0))//CLICKED -WAS THERE A UNIT THERE?
        {
            Debug.Log("MOUSE UP -- click!");
            howLong = 0;

            Unit[] us = hexUnderMouse.Units();

            // TODO: Implement cycling through multiple units in the same tile

            if (us.Length > 0)
            {
                selectedUnit = us[0];
                Debug.Log("Selected Unit: " + selectedUnit.Name);

                // NOTE: Selecting a unit does NOT change our mouse mode

                //Update_CurrentFunc = Update_UnitMovement;
            }

        }
        else if (selectedUnit != null && Input.GetMouseButtonDown(1))//HEXPATH MOVEMENT
        {
            // We have a selected unit, and we've pushed down the right
            // mouse button, so enter unit movement mode.
            //Debug.Log("NAvi: " + Navi);
            switch (selectedUnit.Team)
            {
                case "RedTeam":
                    Navi.GetComponent<MeshRenderer>().material.color = Color.red;
                    break;
                case "YellowTeam":
                    Navi.GetComponent<MeshRenderer>().material.color = Color.yellow;
                    break;
                case "BlueTeam":
                    Navi.GetComponent<MeshRenderer>().material.color = Color.blue;
                    break;
            }
                        
            Update_CurrentFunc = Update_UnitMovement;

        }
        else if (Input.GetMouseButton(0) && selectedUnit != null)
        {
            howLong += Time.deltaTime;
            Debug.Log(howLong);
            if(howLong>.4f)
            {
                try
                {
                    if (selectedUnit == hexLastUnderMouse.Units()[0])
                    {
                       // Debug.Log("Mouse held and unit selected - INICIATE THE DEATH RAY");
                        UnitActions.SetActive(true);

                        //   Update_CurrentFunc();
                        return;
                    }
                }
                catch
                {

                }
            }

  }
        else if (Input.GetMouseButton(0) &&
            Vector3.Distance(Input.mousePosition, lastMousePosition) > mouseDragThreshold&&(UnitActions==null))
        {

            //Can we do a check to make sure the mouse is down?  currently not registering properly
            if (Input.GetMouseButtonUp(0) == true)
            {
                Debug.Log("Mouse is not down.");
                return;
            }
               
            // Left button is being held down AND the mouse moved? That's a camera drag!
            Update_CurrentFunc = Update_CameraDrag;
            lastMouseGroundPlanePosition = MouseToGroundPlane(Input.mousePosition);
            Update_CurrentFunc();
        }
        else if (selectedUnit != null && Input.GetMouseButton(1))
        {

            // We have a selected unit, and we are holding down the mouse
            // button.  We are in unit movement mode -- show a path from
            // unit to mouse position via the pathfinding system.
        }

    }

    public void Popupdisplay()
    {
        //CHECK THE HEX.UNITS or HEX.RESOURCE or W/e TO Then Call upon a pop up ... use a dictionary? i dont think so. but check. 
    }

    Hex MouseToHex()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        int layerMask = LayerIDForHexTiles.value;

        if (Physics.Raycast(mouseRay, out hitInfo, Mathf.Infinity, layerMask))
        {
            // Something got hit
            //Debug.Log( hitInfo.collider.name );

            // The collider is a child of the "correct" game object that we want.
            GameObject hexGO = hitInfo.rigidbody.gameObject;

            if (hexLastUnderMouse == hexLastUnderMouse)
            {
                howLong += Time.deltaTime;
                if(howLong>.5f)
                {
                    Popupdisplay();
                }
            }
            //  I duno, how do we check if the hex under mouse is the same 
            return hexMap.GetHexFromGameObject(hexGO);
        }
        
        //Debug.Log("Found nothing.");
        return null;
    }

    Vector3 MouseToGroundPlane(Vector3 mousePos)
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(mousePos);
        // What is the point at which the mouse ray intersects Y=0
        if (mouseRay.direction.y >= 0)
        {
            //Debug.LogError("Why is mouse pointing up?");
            return Vector3.zero;
        }
        float rayLength = (mouseRay.origin.y / mouseRay.direction.y);
        return mouseRay.origin - (mouseRay.direction * rayLength);
    }

    void Update_UnitMovement()
    {

        if (UnitActions)
        { UnitActions.SetActive(false); }

        if (Input.GetMouseButtonUp(1) || selectedUnit == null)
        {
            Debug.Log("Complete unit movement.");

            if (selectedUnit != null)
            {
                selectedUnit.SetHexPath(selectedUnit, hexPath);
            }

            CancelUpdateFunc();
            return;
        }

        // We have a selected unit

        // Look at the hex under our mouse

        // Is this a different hex than before (or we don't already have a path)
        if (hexPath == null || hexUnderMouse != hexLastUnderMouse)
        {
            // Do a pathfinding search to that hex
            hexPath = QPath.QPath.FindPath<Hex>(hexMap, selectedUnit, selectedUnit.Hex, hexUnderMouse, Hex.CostEstimate);
        }
    }
    public void deactivateActions()
    {
        if (UnitActions) {
            try
            {
                UnitActions.SetActive(false);
            }
            catch
            {
            }
        }

        // Trying to force mouse button to be up.. for some reason i have a hard time focusing on this.
        //Event e = Event.current;
        /*
        if (e.type == EventType.MouseUp)
        {
            Debug.Log("Mouse Up!");
        }
        */
        //CancelUpdateFunc();
        Update_CurrentFunc = Update_DetectModeStart;
    }

    void Update_CameraDrag()
    {

        deactivateActions();

        if(UnitActions)
        {
            Debug.Log("UnitActions is active");
        }
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Cancelling camera drag.");
            CancelUpdateFunc();
            return;
        }

        if(Input.GetKey(KeyCode.Escape))
        {
            Debug.Log("Cancelling camera drag.");
            CancelUpdateFunc();
            return;
        }

        // Right now, all we need are camera controls

        Vector3 hitPos = MouseToGroundPlane(Input.mousePosition);

        Vector3 diff = lastMouseGroundPlanePosition - hitPos;
        Camera.main.transform.Translate(diff, Space.World);

        lastMouseGroundPlanePosition = hitPos = MouseToGroundPlane(Input.mousePosition);



    }

    void Update_ScrollZoom()
    {
        if (UnitActions)
        { UnitActions.SetActive(false); }

        // Zoom to scrollwheel
        float scrollAmount = Input.GetAxis("Mouse ScrollWheel");
        float minHeight = 2;
        float maxHeight = 20;
        // Move camera towards hitPos
        Vector3 hitPos = MouseToGroundPlane(Input.mousePosition);
        Vector3 dir = hitPos - Camera.main.transform.position;

        Vector3 p = Camera.main.transform.position;

        // Stop zooming out at a certain distance.
        // TODO: Maybe you should still slide around at 20 zoom?
        if (scrollAmount > 0 || p.y < (maxHeight - 0.1f))
        {
            cameraTargetOffset += dir * scrollAmount;
        }
        Vector3 lastCameraPosition = Camera.main.transform.position;
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, Camera.main.transform.position + cameraTargetOffset, Time.deltaTime * 5f);
        cameraTargetOffset -= Camera.main.transform.position - lastCameraPosition;


        p = Camera.main.transform.position;
        if (p.y < minHeight)
        {
            p.y = minHeight;
        }
        if (p.y > maxHeight)
        {
            p.y = maxHeight;
        }
        Camera.main.transform.position = p;

        // Change camera angle
        Camera.main.transform.rotation = Quaternion.Euler(
            Mathf.Lerp(30, 75, Camera.main.transform.position.y / maxHeight),
            Camera.main.transform.rotation.eulerAngles.y,
            Camera.main.transform.rotation.eulerAngles.z
        );


    }

}