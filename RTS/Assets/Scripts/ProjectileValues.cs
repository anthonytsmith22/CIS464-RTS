using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Projectiles", menuName = "ScriptableObjects/ProjectileValues", order = 1)]
public class ProjectileValues : ScriptableObject
{
    public string projectileName;
    public float projectileDamage;
    public Sprite visual;
    public ForceMode2D forceMoce;
    public float force;
}
