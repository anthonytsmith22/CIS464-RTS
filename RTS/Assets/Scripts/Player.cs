using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public string username;
    public float powerReserve;
    public int keysCollected;
    public float threatRating;
    public int unitID;

    
    
    
    // Start is called before the first frame update
    void Start()
    {
        BuildingController build = new BuildingController();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
