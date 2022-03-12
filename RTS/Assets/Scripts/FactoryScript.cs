using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryScript : MonoBehaviour
{

    public GameObject[] UnitPrefabs;

    public PowerManagement PowerManager;

    int TargetUnit = 3;

    float productionTime = 5.0f;
    float productionTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        PowerManager = transform.parent.Find("PowerManager").GetComponent<PowerManagement>();
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
            unit.transform.parent = transform.parent;
            productionTimer = 0.0f;
        }

    }


    public void OnSelected()
    {
        Debug.Log("Factory Selected");
    }

}
