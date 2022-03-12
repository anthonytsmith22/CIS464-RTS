using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BuildingController : MonoBehaviour
{

    private bool buildMode;
    public bool BuildMode 
    { 
        get => buildMode; 
        set 
        {
            buildGhost.SetActive(value);
            buildOutline.SetActive(value);
            buildMode = value;
        } 
    }

    public float CameraPanSpeed;
    
    public GameObject buildOutline;

    GameObject buildGhost, newBuildingPrefab;
    Camera mainCam;
    SpriteRenderer ghostSprite, outlineSprite;
    public LineRenderer lineRenderer;
    BoxCollider2D ghostCollider;

    public List<GameObject> buildings;
    List<Vector3> powerLinePositions;
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
        //powerLines.forceRenderingOff = true;

        powerLinePositions = new List<Vector3>();

    }

    // Update is called once per frame
    void Update()
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

        updateCamera();

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
        BuildMode = true;
    }

    void buildingPlacement(Vector3 gridSnappedPos)
    {
        buildGhost.transform.position = gridSnappedPos;
        buildOutline.transform.position = gridSnappedPos;

        ContactFilter2D filter = new ContactFilter2D();
        if (ghostCollider.OverlapCollider(filter, overlapped) == 0)
            ghostSprite.color = new Color(1.0f, 1.0f, 1.0f, 0.3f);
        else
            ghostSprite.color = new Color(1.0f, 0.4f, 0.4f, 0.3f);

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 position = gridSnappedPos;
            filter.useDepth = true;
            if (ghostCollider.OverlapCollider(filter, overlapped) == 0)
            {
                var newBuilding = Instantiate(newBuildingPrefab, position, Quaternion.identity);
                newBuilding.transform.parent = transform.parent;
                buildings.Add(newBuilding);
                BuildMode = false;
                newBuildingPrefab = null;
            }
            else
            {
                Debug.Log("Collision");
                overlapped[0].GetComponent<Building>().HP -= 5;
            }

        }
    }

    void updateCamera()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        mainCam.transform.position +=  Time.deltaTime * CameraPanSpeed * new Vector3(x, y, 0.0f);
    }
}
