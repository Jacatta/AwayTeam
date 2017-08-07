using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QPath;

public class HexMap : MonoBehaviour, IQPathWorld
{

    // Use this for initialization
    void Start()
    {
        GenerateMap();
    }

    void Update()
    {
        
        // TESTING: Hit spacebar to advance to next turn
        if (Input.GetKeyDown(KeyCode.Space))
        {
           // Debug.Log("Do turn")
            if (units != null)
            {
                takeTurn();
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {

            Debug.Log("Move using dummy path func");
            if (units != null)
            {
                foreach (Unit u in units)
                {
                    u.DUMMY_PATHING_FUNCTION();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            if (units != null)
            {
                Debug.Log("Spawn Blue");
                Unit Blue = new Unit();
                Blue.setDeets("BlueTeam");
                SpawnUnitAt(Blue, OmegaTeam, 36, 14);
            }

        }
    }
    public Unit StartUnit;
    public Unit RUnit;
    public Unit BUnit;
    public Unit YUnit;
    MouseController ms;

    public GameObject HexPrefab;

    public Mesh MeshWater;
    public Mesh[] MeshFlat;
    public Mesh[] MeshHill;
    public Mesh[] MeshMountain;
    public Mesh[] Desert;

    public GameObject ForestPrefab;
    public GameObject JunglePrefab;

    public Material MatOcean;
    public Material MatPlains;
    public Material[] MatGrasslands;
    public Material MatHill;
    public Material[] MatMountains;
    public Material MatDesert;

    //public GameObject AlphaTeam;
    public GameObject AlphaTeam;
    public GameObject OmegaTeam;
    public GameObject ZetaTeam;

    public int TerraIndex = 0;

    // Tiles with height above whatever, is a whatever
    [System.NonSerialized]
    public float HeightMountain = 1f;
    [System.NonSerialized]
    public float HeightHill = 0.6f;
    [System.NonSerialized]
    public float HeightFlat = 0.0f;

    [System.NonSerialized]
    public float MoistureJungle = 0.5f;
    [System.NonSerialized]
    public float MoistureForest = 0.2f;
    [System.NonSerialized]
    public float MoistureGrasslands = 0f;
    [System.NonSerialized]
    public float MoisturePlains = -0.55f;

    [System.NonSerialized]
    public int NumRows = 30;
    [System.NonSerialized]
    public int NumColumns = 50;

    // TODO: Link up with the Hex class's version of this
    [System.NonSerialized]
    public bool AllowWrapEastWest = true;
    [System.NonSerialized]
    public bool AllowWrapNorthSouth = false;

    private Hex[,] hexes;
    private Dictionary<Hex, GameObject> hexToGameObjectMap;
    private Dictionary<GameObject, Hex> gameObjectToHexMap;

    public HashSet<Unit> units;
    private Dictionary<Unit, GameObject> unitToGameObjectMap;
    public List<Unit> UnitList;

    //public enum HEXState { DARK, LIGHT }
    //public HEXState currentState { get; set; }


    public Hex GetHexAt(int x, int y)
    {
        if (hexes == null)
        {
          //  Debug.LogError("Hexes array not yet instantiated.");
            return null;
        }

        if (AllowWrapEastWest)
        {
            x = x % NumColumns;
            if (x < 0)
            {
                x += NumColumns;
            }
        }
        if (AllowWrapNorthSouth)
        {
            y = y % NumRows;
            if (y < 0)
            {
                y += NumRows;
            }
        }
        try
        {
            return hexes[x, y];
        }
        catch
        {
            Debug.LogError("GetHexAt: " + x + "," + y);
            return null;
        }
    }

    public Hex GetHexFromGameObject(GameObject hexGO)
    {
        if (gameObjectToHexMap.ContainsKey(hexGO))
        {
            return gameObjectToHexMap[hexGO];
        }
        return null;
    }

    public GameObject GetHexGO(Hex h)
    {
        if (h != null)
        {
            if (hexToGameObjectMap.ContainsKey(h))
            {
                return hexToGameObjectMap[h];
            }
        }
        return null;
    }

    public Vector3 GetHexPosition(int q, int r)
    {
        Hex hex = GetHexAt(q, r);

        return GetHexPosition(hex);
    }

    public Vector3 GetHexPosition(Hex hex)
    {
        return hex.PositionFromCamera(Camera.main.transform.position, NumRows, NumColumns);
    }

    virtual public void GenerateMap()
    {
        // Generate a map filled with ocean
        Debug.Log("Generate Map");
        hexes = new Hex[NumColumns, NumRows];
        hexToGameObjectMap = new Dictionary<Hex, GameObject>();
        gameObjectToHexMap = new Dictionary<GameObject, Hex>();

        for (int column = 0; column < NumColumns; column++)
        {
            for (int row = 0; row < NumRows; row++)
            {
                // Instantiate a Hex
                Hex h = new Hex(this, column, row);
                h.Elevation = -0.5f;

                hexes[column, row] = h;

                Vector3 pos = h.PositionFromCamera(
                    Camera.main.transform.position,
                    NumRows,
                    NumColumns
                );

                GameObject hexGO = (GameObject)Instantiate(
                    HexPrefab,
                    pos,
                    Quaternion.identity,
                    this.transform
                );

                //hexToGameObjectMap[h] = hexGO;// think this is the same as - hexToGameObjectMap.Add(h,hexGO);
                hexToGameObjectMap.Add(h, hexGO);
                gameObjectToHexMap[hexGO] = h;

                hexGO.name = string.Format("Hex: {0}, {1}", column, row);
                var hexcomponent = hexGO.GetComponent<HexComponent>();
                h.HexComonent = hexcomponent;
                hexcomponent.Hex = h;
                hexcomponent.HexMap = this;
                if(hexcomponent.Hex == null)
                {
                    Debug.Log("crap");
                }

            }
        }

        UpdateHexVisuals();

        //StaticBatchingUtility.Combine( this.gameObject );
    }

    public void UpdateHexVisuals()
    {
        for (int column = 0; column < NumColumns; column++)
        {
            for (int row = 0; row < NumRows; row++)
            {
                Hex h = hexes[column, row];
                GameObject hexGO = hexToGameObjectMap[h];

                MeshRenderer mr = hexGO.GetComponentInChildren<MeshRenderer>();
                MeshFilter mf = hexGO.GetComponentInChildren<MeshFilter>();


                if (h.Elevation >= HeightFlat && h.Elevation < HeightMountain)
                {
                    if (h.Moisture >= MoistureJungle)
                    {

                        if (TerraIndex >= MeshMountain.Length)
                        { TerraIndex = 0; }
                           
                        mr.material = MatGrasslands[TerraIndex];
                        h.TerrainType = Hex.TERRAIN_TYPE.GRASSLANDS;
                        h.FeatureType = Hex.FEATURE_TYPE.RAINFOREST;

                        // Spawn trees
                        Vector3 p = hexGO.transform.position;
                        if (h.Elevation >= HeightHill)
                        {
                            p.y += 0.25f;
                        }

                        GameObject.Instantiate(JunglePrefab, p, Quaternion.identity, hexGO.transform);
                    }
                    else if (h.Moisture >= MoistureForest)
                    {
                        if (TerraIndex >= MeshMountain.Length)
                            TerraIndex = 0;
                        mr.material = MatGrasslands[TerraIndex];
                        h.TerrainType = Hex.TERRAIN_TYPE.GRASSLANDS;
                        h.FeatureType = Hex.FEATURE_TYPE.FOREST;

                        // Spawn trees
                        Vector3 p = hexGO.transform.position;
                        if (h.Elevation >= HeightHill)
                        {
                            p.y += 0.25f;
                        }
                        GameObject.Instantiate(ForestPrefab, p, Quaternion.identity, hexGO.transform);
                    }
                    else if (h.Moisture >= MoistureGrasslands)
                    {
                        if (TerraIndex >= MeshMountain.Length)
                            TerraIndex = 0;
                        mr.material = MatGrasslands[TerraIndex];

                        h.TerrainType = Hex.TERRAIN_TYPE.GRASSLANDS;
                    }
                    else if (h.Moisture >= MoisturePlains)
                    {
                        mr.material = MatPlains;
                        h.TerrainType = Hex.TERRAIN_TYPE.PLAINS;
                    }
                    else
                    {
                        mr.material = MatDesert;
                        h.TerrainType = Hex.TERRAIN_TYPE.DESERT;
                    }
                }

                if (h.Elevation >= HeightMountain)
                {
                    if (TerraIndex >= MatMountains.Length)
                        TerraIndex = 0;

                    mr.material = MatMountains[TerraIndex];
                    mf.mesh = MeshMountain[TerraIndex];
                    TerraIndex++;

                    h.ElevationType = Hex.ELEVATION_TYPE.MOUNTAIN;
                }
                else if (h.Elevation >= HeightHill)
                {

                    if (TerraIndex >= MeshHill.Length)
                        TerraIndex = 0;
                    h.ElevationType = Hex.ELEVATION_TYPE.HILL;
                    mf.mesh = MeshHill[TerraIndex];
                    mr.material = MatHill;
                    TerraIndex++;
                }
                else if (h.Elevation >= HeightFlat)
                {
                    if (TerraIndex >= MeshFlat.Length)
                        TerraIndex = 0;
                    h.ElevationType = Hex.ELEVATION_TYPE.FLAT;
                   // Debug.Log(TerraIndex);
                    mf.mesh = MeshFlat[TerraIndex];
                    TerraIndex++;
                    if (h.Moisture < MoisturePlains)
                    {
                        if (TerraIndex >= MeshHill.Length)
                            TerraIndex = 0;

                        mf.mesh = Desert[TerraIndex];
                        TerraIndex++;
                    }

                }
                else
                {
                    h.ElevationType = Hex.ELEVATION_TYPE.WATER;
                    mr.material = MatOcean;
                    mf.mesh = MeshWater;
                }

                // hexGO.GetComponentInChildren<TextMesh>().text =
                // string.Format("{0},{1}\n{2}", column, row, h.BaseMovementCost(false, false, false));
                // h.SetCurrentState(Hex.HEXState.DARK);
            }
        }
    }

    public Hex[] GetHexesWithinRangeOf(Hex centerHex, int range)
    {
        List<Hex> results = new List<Hex>();

        for (int dx = -range; dx < range - 1; dx++)
        {
            for (int dy = Mathf.Max(-range + 1, -dx - range); dy < Mathf.Min(range, -dx + range - 1); dy++)
            {
                results.Add(GetHexAt(centerHex.Q + dx, centerHex.R + dy));
            }
        }

        return results.ToArray();
    }

    public void SpawnUnitAt(Unit unit, GameObject prefab, int q, int r)
    {

    
        if (units == null)
        {
            units = new HashSet<Unit>();
            Debug.Log("New hashset units made");
            unitToGameObjectMap = new Dictionary<Unit, GameObject>();
        }

        Hex myHex = GetHexAt(q, r);
        Debug.Log(myHex+": Ele"+myHex.Elevation);
        if (myHex.Elevation <= 0)
        {
            Debug.Log("starting water tile - redo");
            q = Random.Range(0,NumColumns);//(0,NumColumns)
            r = Random.Range(0, NumRows);
            SpawnUnitAt(unit, prefab, q, r);
            return;
        }

        GameObject myHexGO = hexToGameObjectMap[myHex];
        unit.SetHex(myHex);

        Vector3 p = myHexGO.transform.position;
      
        GameObject unitGO = (GameObject)Instantiate(prefab, p, Quaternion.identity, myHexGO.transform);
       // Debug.Log(unit.OnUnitMoved());
        unit.OnUnitMoved += unitGO.GetComponent<UnitView>().OnUnitMoved;

        units.Add(unit);
        unitToGameObjectMap.Add(unit, unitGO);

        if ( StartUnit == null)
                StartUnit = unit;
        Debug.Log("StartUnit " + StartUnit);

        Debug.Log("unit.Team " + unit.Team.ToString());
        if (unit.Team=="RedTeam")
        {
            StartUnit= unit;
            RUnit = unit;
            Debug.Log("RUnit " + RUnit.ToString());
        }
        else if (unit.Team == "BlueTeam")
        {
            BUnit = unit;
            Debug.Log("BUnit " + BUnit.ToString());
        }
        else if (unit.Team == "YellowTeam")
        {
            YUnit = unit;
            Debug.Log("YUnit " + YUnit.ToString());
        }

    }
    
    public void takeTurn()
    {
        foreach (Unit u in units)
        {
            u.DoTurn(u);
            Debug.Log("U.Name = " + u.Name);
        }
    }


    
}