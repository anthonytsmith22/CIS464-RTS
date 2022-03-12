using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryScript : MonoBehaviour
{

    static GameObject[] UnitPrefabs;

    int TargetUnit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GameObject productionTarget = UnitPrefabs[TargetUnit];
        // Unit u = productionTarget.GetComponent<Unit>().UnitStats.ProductionTime;
        

    }


    public void OnSelected()
    {
        Debug.Log("Factory Selected");
    }

}
