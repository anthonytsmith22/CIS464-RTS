using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Units", menuName = "ScriptableObjects/UnitValues", order = 1)]
public class UnitValues : ScriptableObject
{
    public string prefabName; // Name of unit
    public float health; // Max health of unit
    public float healthRegen; // Health regen of unit
    public float healthRegenRate; // Health regen rate of unit
    public float movementSpeed; // Movement speed of unit
    public float attackSpeed; // Attack rate of unit; in attacks per minute
    public bool isProjectile; // If attack is projectile based or hit scan
    public GameObject projectile; // Projectile Prefab
    public float hitScanDamage; // Hit scan damage
    public Sprite visual; // Sprite of unit
    public float scale; // Scale of unit, accounts for x and y scaling
    public float level; // Level of unit
    public float range; // Range for unit detection and attack
}
