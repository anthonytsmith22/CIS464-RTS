using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerManagement : MonoBehaviour
{

    public float powerGenerated = 0;
    public float powerConsumed = 0;
    private float excessCapacity = 100; //power that does not need to be supported by the distribution system
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public bool testExcess(GameObject building){
        if( powerGenerated + building.EnergyConsumption - building.EnergyProduction < excessCapacity ){
            powerConsumed += building.EnergyConsumption;
            powerGenerated += building.EnergyProduction;
            return true;
        }
        else return false;
    }

    
}
