using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerManagement : MonoBehaviour
{


    public Slider PowerBar;
    public Text PowerText;

    public float production;
    public float consumption;
    private static float excessCapacity = 100; //power that does not need to be supported by the distribution system

    public float Satisfaction { get => Mathf.Min(production / consumption, 1.0f); }

    public BuildingController buildingController;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        production = 0;
        consumption = 0;

        buildingController.RemoveDead();

        foreach (var building in buildingController.buildings)
        {
            Building buildComponent = building.GetComponent<Building>();
            production += buildComponent.Stats.EnergyProduction;
            consumption += buildComponent.Stats.EnergyConsumption;

            
        }

        PowerText.text = $"Power: {production} / {consumption} MW";

        if (consumption != 0)
            PowerBar.value = Mathf.Min(production / consumption, 1.0f);
        else
            PowerBar.value = 0;
    }

    // public bool addToPowerGrid(Object consumer)
    // {
    //     if ( consumer.power + powerConsumed - ( (powerGenerated >= excessCapacity) ? 0 : Mathf.Floor( -Mathf.Exp(powerConsumed/Mathf.Sqrt(10*excessCapacity) , 4 ) + excessCapacity) ) <= powerGenerated) //scaling difficulty allows building without dristributors early game
    //     {
    //         powerConsumed += consumer.power;

    //         Debug.Log("Added new building to grid, now consuming " + powerConsumed);
    //         return true;
    //     }

    //     else
    //     {
    //         Debug.Log("Adding building exceeded available grid space, did not connect to grid");
    //         return false;
    //     }
    // }

    // public void removeFromGrid(Object consumer)
    // {
    //     powerConsumed -= consumer.power;
    // }

    // public void removeFromDistribution(Object distributor)
    // {
    //     powerGenerated -= distributor.generated;
    // }

    // public void addToDistribution(Object distributor)
    // {
    //     powerGenerated += distributor.generated;
    // }

    // public float getProductionFactor()
    // {
    //     return Mathf.Min(powerGenerated / powerConsumed , 6.0f);
    // }

    // public static bool testExcess(GameObject building){
    //     if( powerGenerated + building.EnergyConsumption - building.EnergyProduction < excessCapacity ){
    //         powerConsumed += building.EnergyConsumption;
    //         powerGenerated += building.EnergyProduction;
    //         return true;
    //     }
    //     else return false;
    // }

    
}
