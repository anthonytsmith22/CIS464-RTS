using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BuildingStats")]
public class BuildingStats : ScriptableObject
{
    // Energy cost in MJ to make a building
    public int BuildCost;
    public int MaxHP;

    // Energy consumption in MW
    public int EnergyConsumption;
    // Energy production in MW
    public int EnergyProduction;
}
