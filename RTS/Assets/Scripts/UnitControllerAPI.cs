using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class UnitControllerAPI : MonoBehaviour
{
    // Necessary values from ScriptableObject ScriptableObject/UnitValues
    [SerializeField] UnitValues Values;
    public string unitName;
    public float maxHealth;
    public float healthRegen;
    public float movementSpeed;
    public float attackSpeed;
    public bool isProjectile;
    public GameObject Projectile;
    public Sprite visual;
    public float scale;
    public float level;
    public float range;
    public float hitScanDamage;

    // Other attributes
    public int FACTION; // VAVLUES = 0...N, N being number of factions-1, 0 = Player
    public float CurrentHealth;
    public bool isEngaged;
    [SerializeField] Transform firePoint; // Where unit fires from

    // Unit's current unit target
    public UnitControllerAPI UnitTarget;
    public Transform UnitTargetTransform;

    // Unit's current position/location target
    public Vector3 PositionTarget;

    // Movement clamp values / Keep it in confined to the map
    [SerializeField] private Transform Map; 
    private float minX, maxX, minZ, maxZ;

    // Should the unit be moveing, also acts as an isMoving check
    private bool shouldMove = false;
    // Is unit colliding with building/obstacle
    private bool isColliding = false;

    // Unit RigidBody2D
    private Rigidbody2D rb;

    // Unit's sprite visual
    SpriteRenderer unitVisual;

    // Unit's projectile
    [SerializeField] GameObject ProjectilePrefab;

    // Unit's Range
    [SerializeField] private CircleCollider2D UnitRange;

    // List of targets
    private List<UnitControllerAPI> potentialTargets = new List<UnitControllerAPI>();

    public virtual void Awake(){
        // Set values to those in ScriptableObject
        GetValues();

        unitVisual = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true; // RB only responds to scripts
        rb.gravityScale = 0f;   // RB doesn't use gravity

        // Setup other attributes such as CurrentHealth
        Setup();
    }

    public virtual void FixedUpdate(){
        if(shouldMove){ // If unit should be moving
            MoveUnit();
        }
    }

    public virtual void Setup(){
        CurrentHealth = maxHealth;
        isEngaged = false;

        // Set visual
        unitVisual.sprite = visual;
        // Set range
        UnitRange.radius = range;
        UnitRange.isTrigger = true;

        SetupClamp();
    }

    private void SetupClamp(){ // Set mix, max X and Z
        float centerX = Map.position.x;
        float centerZ = Map.position.z;

        float scaleX = Map.localScale.x;
        float scaleZ = Map.localScale.z;

        minX = centerX - (scaleX/2);
        maxX = centerX + (scaleX/2);

        minZ = centerZ - (scaleZ/2);
        maxZ = centerZ + (scaleZ/2);
    }

    public virtual void GetValues(){  // Set object values to those in ScriptableObject
        unitName = Values.prefabName;
        maxHealth = Values.health;
        healthRegen = Values.healthRegen;
        movementSpeed = Values.movementSpeed;
        attackSpeed = Values.attackSpeed;
        unitVisual.sprite = Values.visual;
        isProjectile = Values.isProjectile;
        if(isProjectile){ // If unit is projectile based, get projectile prefab
            Projectile = Values.projectile;
        }
        scale = Values.scale;
        level = Values.level;
        range = Values.range;
        hitScanDamage = Values.hitScanDamage;
    }

    public virtual void Shoot(){  // Shoot logic for unit
        if(UnitTarget == null){
            ResetCombat(); // If unit has no target, in the case the target was destroyed, reset combat which will check if there is a new target to attack
        }
        Vector2 fireDirection = UnitTargetTransform.position - firePoint.position; // Get direction to fire
        if(isProjectile){ // If attack is of type projectile
            ProjectileShoot(fireDirection);
        }else{ // Is of type hit scan
            HitScanShoot(fireDirection);
        }
    }

    public virtual void ProjectileShoot(Vector2 fireDirection){
        fireDirection = fireDirection.normalized;
        float angle = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg; // Get angle to fire in
        Vector3 newRotation = new Vector3(0f, angle, 0f); // Set new angle for firePoint
        firePoint.eulerAngles = newRotation;
        GameObject projectile = Instantiate(Projectile, firePoint.position, firePoint.rotation);
        ProjectileController projectileController = projectile.GetComponent<ProjectileController>(); // Get projectile controller
        projectileController.Setup(fireDirection, FACTION); // Pass fire direction and this unit's faction to disable friendly fire
    }

    public virtual void HitScanShoot(Vector2 fireDirection){
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, fireDirection); // Fire raycast at target

            if(hit.collider != null){ // Check for hit
                // Check if it is a unit or building and faction
                if(hit.transform.tag.Equals("Building")){
                    // BuildingController building = hit.transform.GetComponent<BuildingController>(); // Get building controller
                    // int faction = building.FACTION;  // get faction
                    // if(faction != FACTION){ // if building is not from this faction, damage it
                        // building.DoDamage(damage);
                    // }
                }else if(hit.transform.tag.Equals("Unit")){
                    UnitControllerAPI unitController = hit.transform.GetComponent<UnitControllerAPI>();
                    int faction = unitController.FACTION;
                    if(faction != FACTION){
                        unitController.TakeDamage(hitScanDamage);
                    }
                }
            }
    }

    public virtual void HitScanTracer(Vector2 fireDirection){
        fireDirection = fireDirection.normalized;
        LineRenderer line = Instantiate(new LineRenderer(), firePoint, this);
        
    }

    public virtual void EngageCombat(){ // Use to initiate combat
        if(isEngaged){ Debug.LogWarning("Tried to engage when already engaged"); return; } // If already engaged, do not proceed with action
        if(UnitTarget == null){ Debug.LogWarning("Tried to engage with no target set"); return; } // If no target set, do not proceed with action
        float attackRate = 60 / attackSpeed; // How often in seconds the unit attacks

        InvokeRepeating("Shoot", 0f, attackRate);
        isEngaged = true;
    }

    public virtual void DisengageCombat(){ // Use to cancel combat
        if(!isEngaged){ Debug.LogWarning("Tried to disengaged when not engaged"); return; }
        
        CancelInvoke("Shoot");
        isEngaged = false;
    }

    public virtual void ResetCombat(){ // Use when attack params change such as attack speed;
        DisengageCombat();
        if(UnitTarget == null){ // If unit target is null, check for new target in potentialTargets
            if(potentialTargets[0] != null){ // Check for viable target
                UnitTarget = potentialTargets[0];
            }else{ // No new target to engage in
                return;
            }
        }
        EngageCombat();
    }

    public virtual void TakeDamage(float damage){ // Damage logic for unit
        CurrentHealth -= damage;
        if(CurrentHealth < 0f){
            CurrentHealth = 0f;

            Death();
        }
    }

    public virtual void MoveUnit(){  // Move unit to specified Vector3
        Vector3 currentPosition = transform.position;
        if(PositionTarget == currentPosition){ // Check if unit is at target position
            shouldMove = false; // If so, disable movement
            return;
        }
        Vector3 differenceVector = (PositionTarget - currentPosition).normalized; // Get the difference between unit's current position and targetLocation

        Vector3 newVelocity = differenceVector * movementSpeed;
        rb.velocity = newVelocity;
    }

    public virtual void SetUnitTarget(UnitControllerAPI target){ // Sets target to specified unit
        int faction = target.FACTION;
        if(faction == this.FACTION){ Debug.LogWarning("Target set to friendly unit, action aborted."); return; }
        UnitTarget = target;
        UnitTargetTransform = target.transform;
    }

    public void SetPositionTarget(Vector3 target){
        PositionTarget = target;
        shouldMove = true;
    }

    public virtual void RemoveTarget(){ // Removes current target
        UnitTarget = null;
    }

    public virtual void Death(){  // Unit death logic
        DisengageCombat();

        Destroy(this.gameObject);
    }

    public virtual void OnCollisionEnter2D(Collision2D other){ // Check for collisions with obstacles and buildings
        string otherTag = other.gameObject.tag;
        if(otherTag.Equals("Building") || otherTag.Equals("Obstacle")){ // 
            isColliding = true;
        }else{
            isColliding = false;
        }
    }

    public virtual void OnTriggerEnter2D(Collider2D other){
        string otherTag = other.tag;
        if(otherTag.Equals("UnitRange")){ // Trigger caused by other unit's range collider, ignore
            return;
        }else if(otherTag.Equals("Unit")){ // Trigger caused by unit, determine faction and auto target if has no active target
            UnitControllerAPI unitController = other.GetComponent<UnitControllerAPI>();
            int faction = unitController.FACTION;
            if(faction != this.FACTION){ // Check if unit belongs to other faction
                if(UnitTarget == null){ // Check if unit doesn't have an active target
                    UnitTarget = unitController;
                    EngageCombat();  // Engage in combat
                }else{
                    potentialTargets.Add(unitController);
                }
            }
        }
    }

    public virtual void OnTriggerExit2D(Collider2D other){
        string otherTag = other.tag;
        if(otherTag.Equals("UnitRange")){ // If caused by unit range, ignore
            return;
        }else if(otherTag.Equals("Unit")){ // If caused by unit, check if its a potential target
            UnitControllerAPI unitController = other.GetComponent<UnitControllerAPI>();
            if(potentialTargets.Contains(unitController)){ // If it does, remove it from potential targets
                potentialTargets.Remove(unitController);
            }
        }
    }

}
