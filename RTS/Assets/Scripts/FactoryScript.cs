using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryScript : MonoBehaviour
{
    int faction;
    public GameObject[] UnitPrefabs;

    public PowerManagement PowerManager;
    public GameObject FactoryUI;
    private Building building;

    int TargetUnit = 3;

    float productionTime = 5.0f;
    float productionTimer = 0.0f;

    private void Awake(){
        building = GetComponent<Building>();
        faction = building.FACTION;
    }

    // Start is called before the first frame update
    void Start()
    {
        PowerManager = transform.parent.GetComponent<PowerManagement>();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject productionTarget = UnitPrefabs[TargetUnit];
        // Unit u = productionTarget.GetComponent<Unit>().UnitStats.ProductionTime;
        productionTimer += Time.deltaTime * PowerManager.Satisfaction;
        if (productionTimer >= productionTime)
        {
            var unit = Instantiate(productionTarget, transform.position, transform.rotation);
            UnitControllerAPI unitController = unit.GetComponent<UnitControllerAPI>();
            unitController.FACTION = faction;
            unit.transform.parent = transform.parent;
            productionTimer = 0.0f;
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
