using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryScript : MonoBehaviour
{
    public int faction;
    public GameObject[] UnitPrefabs;
    public Player Controller;
    public PowerManagement PowerManager;
    public GameObject FactoryUI;
    private Building building;

    public int TargetUnit = 3;

    float productionTime = 5.0f;
    float productionTimer = 0.0f;

    private void Awake(){
        
    }

    // Start is called before the first frame update
    void Start()
    {
        PowerManager = transform.parent.GetComponent<PowerManagement>();
        building = GetComponent<Building>();
        faction = building.FACTION;
        Controller = transform.parent.GetComponent<Player>();
        transform.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        GameObject productionTarget = UnitPrefabs[TargetUnit];
        // Unit u = productionTarget.GetComponent<Unit>().UnitStats.ProductionTime;
        productionTimer += Time.deltaTime * PowerManager.Satisfaction;
        if (productionTimer >= productionTime)
        {
            productionTimer = 0.0f;
            GameObject unit = Instantiate(productionTarget, transform.position, transform.rotation);
            unit.transform.parent = null;
            UnitControllerAPI unitController = unit.GetComponent<UnitControllerAPI>();
            unitController.FACTION = faction;
            unit.transform.parent = null;
            Color factionColor = faction == 0 ? Color.white : new Color(1.0f, 0.6f, 0.6f, 1.0f);
            unit.GetComponent<SpriteRenderer>().color = factionColor;
            string unitName = unitController.unitName;
            if(unitName.Equals("KeyDrone")){
                Controller.keyDrones.Add(unitController);
            }
            else if(unitName.Equals("FastDrone")){
                Controller.fastDrones.Add(unitController);
            }
            else if(unitName.Equals("MediumDrone")){
                Controller.mediumDrones.Add(unitController);
            }
            else if(unitName.Equals("HeavyDrone")){
                Controller.heavyDrones.Add(unitController);
            }   
        }

    }

    public void SetProductionTarget(int target)
    {
        TargetUnit = target;
    }

    public void OnSelected()
    {
        Debug.Log("Factory Selected");

    }

}
