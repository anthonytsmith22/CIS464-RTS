using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BuildingController : MonoBehaviour
{

    public int FACTION;

    private bool buildMode;
    public bool BuildMode 
    { 
        get => buildMode; 
        set 
        {
            if (FACTION == 0)
            {
                buildGhost.SetActive(value);
                buildOutline.SetActive(value);
            }
            buildMode = value;
        } 
    }

    public float CameraPanSpeed;
    
    public GameObject buildOutline;
    GameObject buildGhost, newBuildingPrefab;
    Camera mainCam;
    SpriteRenderer ghostSprite, outlineSprite;
    BoxCollider2D ghostCollider;
    public List<GameObject> buildings;
    Collider2D[] overlapped = new Collider2D[30];

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        buildGhost = new GameObject("BuildGhost");
        ghostSprite = buildGhost.AddComponent<SpriteRenderer>();
        ghostCollider = buildGhost.AddComponent<BoxCollider2D>();
        ghostCollider.size = new Vector2(2.5f, 2.5f);
        ghostSprite.color = new Color(1.0f, 1.0f, 1.0f, 0.3f);
        BuildMode = false;

        buildings = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!PauseMenu.paused)
        {
            Vector3 mousePosWorld = mainCam.ScreenToWorldPoint(Input.mousePosition);
            mousePosWorld.z = 0;

            Vector3 gridSnappedPos = new Vector3();
            gridSnappedPos.x = Mathf.Floor(mousePosWorld.x);
            gridSnappedPos.y = Mathf.Floor(mousePosWorld.y);
            gridSnappedPos.z = 0;

            if (BuildMode)
            {
                if (Input.GetMouseButtonDown(1))
                    BuildMode = false;
                buildingPlacement(gridSnappedPos);
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {

                    foreach (var b in buildings)
                    {
                        // deselect everything
                        b.GetComponent<Building>().Selected = false;

                        // if collision with mouse, select this building
                        if (b.GetComponent<BoxCollider2D>().OverlapPoint(mousePosWorld))
                        {
                            Building building = b.GetComponent<Building>();
                            building.Selected = true;
                        }
                    }
                }
            }

        }

    }

    public void RemoveDead()
    {
        List<GameObject> dead = buildings.FindAll(b => b.GetComponent<Building>().Dead);

        buildings.RemoveAll(b => b.GetComponent<Building>().Dead);

        foreach (var d in dead)
            Destroy(d);

    }

    public void CreateBuilding(GameObject prefab)
    {
        newBuildingPrefab = prefab;
        ghostSprite.sprite = prefab.GetComponent<SpriteRenderer>().sprite;
        ghostSprite.sortingLayerName = "Building";
        BuildMode = true;
    }

    int factoryType = 0;
    public bool SpawnBuilding(Vector3 position, GameObject prefab)
    {
        
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero, 0.1f, LayerMask.GetMask("Building"));
        RaycastHit2D hit2 = Physics2D.Raycast(position, Vector2.zero, 0.1f, LayerMask.GetMask("Water"));
        Debug.Log(hit2.transform == null);
        if (hit.transform == null && (hit2.transform != null))
        {       
            var newBuilding = Instantiate(prefab, position, Quaternion.identity);
            if(prefab.name.Equals("Factory")){
                Player controller = GetComponent<Player>();
                factoryType = controller.UnitTypeIndex;
                if(factoryType == 4){
                    factoryType = 0;
                    controller.UnitTypeIndex = 0;
                }
                FactoryScript factory = newBuilding.GetComponent<FactoryScript>();
                factory.TargetUnit = factoryType;
                controller.UnitTypeIndex++;
            }
            newBuilding.transform.parent = transform;
            Building building = newBuilding.GetComponent<Building>();
            building.FACTION = FACTION;
            buildings.Add(newBuilding); 
            return true;
        }
        else
        {
            Debug.Log("Collision");
            return false;
            // play sound
        }
    }

    void buildingPlacement(Vector3 gridSnappedPos)
    {
        buildGhost.transform.position = gridSnappedPos;
        buildOutline.transform.position = gridSnappedPos;

        ContactFilter2D filter = new ContactFilter2D();
        filter.layerMask = LayerMask.GetMask("Building");
        if (ghostCollider.OverlapCollider(filter, overlapped) == 0)
            ghostSprite.color = new Color(1.0f, 1.0f, 1.0f, 0.3f);
        else
            ghostSprite.color = new Color(1.0f, 0.4f, 0.4f, 0.3f);

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 position = gridSnappedPos;
            filter.useDepth = true;
            if (SpawnBuilding(position, newBuildingPrefab))
            {
                BuildMode = false;
                newBuildingPrefab = null;
            }
        }
    }

}
