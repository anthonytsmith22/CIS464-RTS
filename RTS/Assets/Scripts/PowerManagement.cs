using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerManagement : MonoBehaviour
{

    public int FACTION;
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

    
}
