using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileController : MonoBehaviour
{
    // Necessary values from ScriptableObject ScriptableObject/ProjectileValues
    [SerializeField] private ProjectileValues Values;
    public string projectileName;
    public float projectileDamage;
    private ForceMode2D forceMode;
    private float force;
    private float speed = 10;
    private Vector3 fireDirection;

    // Other attributes 
    Rigidbody2D rb;
    public CircleCollider2D ProjectileCollider;
    public int faction; // Faction that projectile was fired from

    private void Awake(){
        rb = GetComponent<Rigidbody2D>();
        ProjectileCollider = GetComponent<CircleCollider2D>();

        GetValues();
    }

    private void Update(){
        //transform.position += fireDirection * speed * Time.deltaTime;
    }

    // Called in UnitControllerAPI when projectile is instantiated
    public virtual void Setup(Vector2 fireDirection, int faction){
        this.faction = faction;
        this.fireDirection = fireDirection;
        Launch(fireDirection);
    }

    public virtual void Launch(Vector2 fireDirection){ // Physics based approach to moving the projectile
        rb.AddForce(fireDirection * force, forceMode);
    }

    public virtual void GetValues(){ // Set object values to those in ScriptableObject
        projectileName = Values.projectileName;
        projectileDamage = Values.projectileDamage;
        forceMode = Values.forceMoce;
        force = Values.force;
    }

    private void OnTriggerEnter2D(Collider2D other){
        string otherTag = other.gameObject.tag;
        UnitControllerAPI unitController;
        if(otherTag.Equals("Unit")){ // Logic regarding hitting drone/unit
            unitController = other.transform.GetComponent<UnitControllerAPI>();
            if(unitController.FACTION == faction){ // If attack hits drone of same faction, ignore
                return;
            }
            // Else
            DoDamage(unitController);
            return;
        }else if(otherTag.Equals("Building")){
            // Check building faction
            // Get building component
            // DoDamage(BuildingControllerAPI buildingController);
            Building otherBuilding = other.transform.GetComponent<Building>();
            if(otherBuilding.FACTION == faction){
                return;
            }
            otherBuilding.HP -= (int)projectileDamage;
            return;
        }
        else if(otherTag.Equals("Environment")){
            
        }
    }

    public virtual void OnColliderEnter2D(Collision2D other){  // Collision Management
        
    }

    public virtual void DoDamage(UnitControllerAPI unitController){ // Do damage to hit drone
        // Expand/override to add particle effects or AOE
        unitController.TakeDamage(projectileDamage);
        Destroy(gameObject);
    }

}
