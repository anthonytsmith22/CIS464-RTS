using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Player : MonoBehaviour
{

    public static Player HumanPlayer;
    public bool IsPlayer = true;
    public int BuildingCount = 0;
    public int UnitTypeIndex = 0;
    public string username;
    public float powerReserve;
    public int keysCollected;
    public float threatRating;
    public int unitID;
    public int FACTION;
    public PowerManagement PowerManagement;
    public BuildingController BuildingController;

    public List<Key> HeldKeys = new List<Key>();
    public List<UnitControllerAPI> fastDrones = new List<UnitControllerAPI>();
    public List<UnitControllerAPI> mediumDrones = new List<UnitControllerAPI>();
    public List<UnitControllerAPI> heavyDrones = new List<UnitControllerAPI>();
    public List<UnitControllerAPI> keyDrones = new List<UnitControllerAPI>();
    
    
    void Awake()
    {
        if (FACTION == 0)
            HumanPlayer = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //BuildingController build = new BuildingController();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setup(int faction){
        FACTION = faction;
        PowerManagement.FACTION = faction;
        BuildingController.FACTION = faction;
        if(!IsPlayer){
            BuildingController.player = this;
        }
    }

    public virtual int GetNumDrones(){
        int count = 0;
        count += fastDrones.Count;
        count += mediumDrones.Count;
        count += heavyDrones.Count;
        count += keyDrones.Count;
        return count;
    }

    public virtual int GetNumDrones(UnitType type){
        switch(type){
            case UnitType.FAST:
                return fastDrones.Count;
            case UnitType.MEDIUM:
                return mediumDrones.Count;
            case UnitType.HEAVY:
                return heavyDrones.Count;
            case UnitType.KEY:
                return keyDrones.Count;     
        }
        return 0;
    }

    public void IncreaseBuildingCount(){
        BuildingCount++;
    }

    public void OnBuildingDestroyed(){
        BuildingCount--;
        GameManager.Instance.CheckBuildingCount(this);
    }
}
