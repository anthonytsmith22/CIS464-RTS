using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BuildingController : MonoBehaviour
{

    public GameObject[] buildingPrefabs;
    public float CameraPanSpeed;
    
    GameObject buildGhost;
    int buildingType;

    Camera mainCam;
    SpriteRenderer ghostSprite;
    BoxCollider2D ghostCollider;

    List<GameObject> buildings;

    Collider2D[] overlapped = new Collider2D[30];

    // Start is called before the first frame update
    void Start()
    {
        buildingType = 0;
        mainCam = Camera.main;
        buildGhost = new GameObject("BuildGhost");
        ghostSprite = buildGhost.AddComponent<SpriteRenderer>();
        ghostCollider = buildGhost.AddComponent<BoxCollider2D>();
        ghostCollider.size = new Vector2(2.5f, 2.5f);
        ghostSprite.color = new Color(1.0f, 1.0f, 1.0f, 0.3f);
        ghostSprite.sprite = buildingPrefabs[buildingType].GetComponent<SpriteRenderer>().sprite;
        buildings = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 mousePosWorld = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mousePosWorld.z = 0;

        Vector3 gridSnappedPos = new Vector3();
        gridSnappedPos.x = Mathf.Floor(mousePosWorld.x);
        gridSnappedPos.y = Mathf.Floor(mousePosWorld.y);

        buildGhost.transform.position = gridSnappedPos;

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
                buildings.Add(Instantiate(buildingPrefabs[buildingType], position, Quaternion.identity));
            else
            {
                Debug.Log("Collision");
            }



        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            buildingType++;
            buildingType %= buildingPrefabs.Length;
            ghostSprite.sprite = buildingPrefabs[buildingType].GetComponent<SpriteRenderer>().sprite;
            // select building to place
        }

        updateCamera();

    }

    void updateCamera()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        mainCam.transform.position +=  Time.deltaTime * CameraPanSpeed * new Vector3(x, y, 0.0f);
    }
}
