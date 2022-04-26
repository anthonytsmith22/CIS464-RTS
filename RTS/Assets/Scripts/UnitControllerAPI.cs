using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class UnitControllerAPI : MonoBehaviour
{
    // Necessary values from ScriptableObject ScriptableObject/UnitValues
    [SerializeField] UnitValues Values;
    public string unitName;
    public float maxHealth;
    public float healthRegen;
    public float movementSpeed;
    public bool canAttack;
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
    [SerializeField] List<Transform> firePoints; // Where unit fires from

    // Unit's current unit target
    public UnitControllerAPI UnitTarget;
    public Transform UnitTargetTransform;

    // Unit's current position/location target
    public Vector3 PositionTarget;

    // Movement clamp values / Keep it in confined to the map
    [SerializeField] private Transform Map; 
    private float minX, maxX, minY, maxY;

    // Should the unit be moveing, also acts as an isMoving check
    private bool shouldMove = false;
    private bool moveAtUnit = false;
    // Is unit colliding with building/obstacle
    private bool isColliding = false;

    // Unit RigidBody2D
    private Rigidbody2D rb;

    // Unit's sprite visual
    SpriteRenderer unitVisual;

    // Unit's projectile
    [SerializeField] GameObject ProjectilePrefab;

    // List of targets
    public List<UnitControllerAPI> potentialTargets = new List<UnitControllerAPI>();


    // list of building targets
    public List<Building> potentialBuildingTargets = new List<Building>();
    public Building buildingTarget;

    // Healthbar controller
    [SerializeField] UnitHealth unitHealthController;
    // Own Collider
    CircleCollider2D unitCollider;
    // Line Renderer for hit scan attacks
    [SerializeField] LineRenderer lineRenderer;
    // Target Layer
    LayerMask TargetLayer;

    public virtual void Awake(){
        // Set values to those in ScriptableObject
        GetValues();

        unitCollider = GetComponent<CircleCollider2D>();
        unitVisual = GetComponent<SpriteRenderer>();
        // rb = GetComponent<Rigidbody2D>();
        // rb.isKinematic = true; // RB only responds to scripts
        // rb.gravityScale = 0f;   // RB doesn't use gravity

        // Setup other attributes such as CurrentHealth
        Setup();
    }

    public virtual void Start(){
        SetDefaultStartPosition();
    }

    public virtual void Update(){
        if(shouldMove){
            if(!moveAtUnit){
                MoveUnit();
            }else{
                MoveUnitToTarget();
            }
        }
    }

    public virtual void Setup(){
        CurrentHealth = maxHealth;
        isEngaged = false;
        numFirePoints = firePoints.Count;
        // Set visual
        //unitVisual.sprite = visual;

        //DisableNestedColliderCollision();
        SetupClamp();
        LayerMaskSetup();
    }

    void DisableNestedColliderCollision(){
        Collider2D rangeCollider = transform.Find("Range").gameObject.GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(unitCollider, rangeCollider);
    }

    public virtual void LayerMaskSetup(){
        // Set unit layer and target layer
        int[] factions;
        switch(FACTION){
            case 0: 
                gameObject.layer = LayerMask.NameToLayer("UnitFaction0");
                factions = new int[]{1,2,3};
                SetTargetLayerMask(factions);
                break;
            case 1:
                gameObject.layer = LayerMask.NameToLayer("UnitFaction1");
                factions = new int[]{0,2,3};
                SetTargetLayerMask(factions);
                break;
            case 2:
                gameObject.layer = LayerMask.NameToLayer("UnitFaction2");
                factions = new int[]{0,1,3};
                SetTargetLayerMask(factions);
                break;
            case 3: 
                gameObject.layer = LayerMask.NameToLayer("UnitFaction3");
                factions = new int[]{0,1,2};
                SetTargetLayerMask(factions);
                break;
            default:
                Debug.LogError("Unit has no faction!");
                break;
        }
    }

    private void SetTargetLayerMask(int[] factions){
        int count = factions.Length;
        string[] layers = new string[count*2];
        StringBuilder sb = new StringBuilder();
        int i;
        // Add unit factions
        for(i = 0; i < count; i++){
            sb.Clear();
            sb.Append("UnitFaction");
            sb.Append(factions[i]);
            layers[i] = sb.ToString();
        }
        // Add building factions
        for(i = 0; i < count; i++){
            sb.Clear();
            sb.Append("BuildingFaction");
            sb.Append(factions[i]);
            layers[count + i] = sb.ToString();
        }
        TargetLayer = LayerMask.GetMask(layers);
        layers = null;
    }

    private void SetupClamp(){ // Set mix, max X and Z
        Map = GameObject.Find("map").transform;
        
        float centerX = Map.position.x;
        float centerY = Map.position.y;

        float scaleX = Map.localScale.x;
        float scaleY = Map.localScale.y;

        minX = centerX - (scaleX/2);
        maxX = centerX + (scaleX/2);

        minY = centerY - (scaleY/2);
        maxY = centerY + (scaleY/2);
    }

    public virtual void GetValues(){  // Set object values to those in ScriptableObject
        unitName = Values.prefabName;
        maxHealth = Values.health;
        healthRegen = Values.healthRegen;
        movementSpeed = Values.movementSpeed;
        canAttack = Values.canAttack;        attackSpeed = Values.attackSpeed;
        //unitVisual.sprite = Values.visual;
        isProjectile = Values.isProjectile;
        if(isProjectile){ // If unit is projectile based, get projectile prefab
            Projectile = Values.projectile;
        }
        scale = Values.scale;
        level = Values.level;
        range = Values.range;
        hitScanDamage = Values.hitScanDamage;
    }

    int firePointIndex;
    int numFirePoints;
    private void SelectNextFirePoint(){
        int lastFirePointIndex = firePointIndex;
        if(lastFirePointIndex + 1 == numFirePoints){
            firePointIndex = 0;
        }else{
            firePointIndex++;
        }
    }

    public virtual void Shoot(){  // Shoot logic for unit
        if(UnitTarget == null || UnitTargetTransform == null){
            ResetCombat(); // If unit has no target, in the case the target was destroyed, reset combat which will check if there is a new target to attack
            return;
        }
        Transform firePoint = firePoints[firePointIndex];
        Vector3 fireDirection = UnitTargetTransform.position - firePoint.position; // Get direction to fire
        if(isProjectile){ // If attack is of type projectile
            ProjectileShoot(fireDirection, firePoint);
        }else{ // Is of type hit scan
            HitScanShoot(fireDirection, firePoint);
        }
        SelectNextFirePoint();
    }

    public virtual void ProjectileShoot(Vector3 fireDirection, Transform firePoint){
        fireDirection = fireDirection.normalized;
        float angle = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg; // Get angle to fire in
        Vector3 newRotation = new Vector3(0f, 0f, angle+90); // Set new angle for firePoint
        firePoint.eulerAngles = newRotation;
        GameObject projectile = Instantiate(Projectile, firePoint.position, firePoint.rotation);
        projectile.transform.eulerAngles = newRotation;
        CircleCollider2D projectileCollider = projectile.GetComponent<CircleCollider2D>();
        Physics2D.IgnoreCollision(unitCollider, projectileCollider);
        ProjectileController projectileController = projectile.GetComponent<ProjectileController>(); // Get projectile controller
        projectileController.Setup(fireDirection, FACTION); // Pass fire direction and this unit's faction to disable friendly fire
    }

    public virtual void HitScanShoot(Vector2 fireDirection, Transform firePoint){
        Debug.Log("HitScan");
        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, fireDirection, 300, TargetLayer); // Fire raycast at target
        
        if(hit.collider != null){ // Check for hit
            // Check if it is a unit or building and faction
            if(hit.transform.tag.Equals("Building")){
                Building building = hit.transform.GetComponent<Building>(); // Get building controller
                int faction = building.FACTION;  // get faction
                if(faction != FACTION){ // if building is not from this faction, damage it
                    building.TakeDamage(hitScanDamage);
                }
            }else if(hit.transform.tag.Equals("Unit")){
                UnitControllerAPI unitController = hit.transform.GetComponent<UnitControllerAPI>();
                int faction = unitController.FACTION;
                if(faction != FACTION){
                    unitController.TakeDamage(hitScanDamage);
                }
            }
        }
        StartCoroutine(HitScanTracer(fireDirection, hit.point, firePoint));
    }

    public virtual IEnumerator HitScanTracer(Vector2 fireDirection, Vector3 hitPosition, Transform firePoint){
        LineRenderer newLineRenderer = Instantiate(lineRenderer, firePoint, this);
        fireDirection = fireDirection.normalized;
        if(hitPosition != null){
            newLineRenderer.SetPosition(0, firePoint.position);
            newLineRenderer.SetPosition(1, hitPosition);
        }else{
            newLineRenderer.SetPosition(0, firePoint.position);
            newLineRenderer.SetPosition(1, firePoint.position + firePoint.eulerAngles * 100);
        }
        newLineRenderer.enabled = true;
        Debug.Log(true);
        yield return new WaitForSeconds(.05f);
        newLineRenderer.enabled = false;
        Destroy(newLineRenderer);
    }

    public virtual void EngageCombat(){ // Use to initiate combat
        if(isEngaged){ Debug.LogWarning("Tried to engage when already engaged"); return; } // If already engaged, do not proceed with action
        if(UnitTarget == null){ Debug.LogWarning("Tried to engage with no target set"); return; } // If no target set, do not proceed with action
        UnitTargetTransform = UnitTarget.transform;
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
            if(potentialTargets.Count > 0){ // Check for viable target
                UnitTarget = potentialTargets[0];
                SetUnitPositionTarget(UnitTarget.transform);
            }
            else if(potentialBuildingTargets.Count > 0){
                SetBuildingTarget(potentialBuildingTargets[0]);
            }
            else{ // No new target to engage in
                SetPositionTarget(transform.position);
                Debug.Log("Reset position");
                shouldMove = false;
            }
        }
        EngageCombat();
    }

    public virtual void TakeDamage(float damage){ // Damage logic for unit
        CurrentHealth -= damage;
        unitHealthController.DoDamage(damage); 
        if(CurrentHealth < 0f){
            CurrentHealth = 0f;
            Death();
        }
    }

    private void ClampPosition(){
        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);
        transform.position = position;
    }

    private float positionMargin = 1f;
    public virtual void MoveUnit(){  // Move unit to specified Vector3
        Vector3 differenceVector = PositionTarget - transform.position; // Get the difference between unit's current position and targetLocation
        if(differenceVector.magnitude <= positionMargin){ // Check if unit is at target position
            shouldMove = false; // If so, disable movement
            //rb.velocity = Vector3.zero;
            return;
        }
        // Vector3 newVelocity = differenceVector.normalized * movementSpeed * Time.fixedDeltaTime;
        // rb.velocity = newVelocity;
        Vector3 movement = differenceVector.normalized * movementSpeed * Time.deltaTime;
        transform.Translate(movement, Space.World);
        ClampPosition();
    }

    private float distanceOffset = 1.5f;
    public virtual void MoveUnitToTarget(){
        Vector3 differenceVector = PositionTarget - transform.position;
        float distanceToUnit = Mathf.Abs(differenceVector.magnitude) + distanceOffset;
        if(distanceToUnit > range && UnitTargetTransform != null){
            shouldMove = true;
        }else{
            shouldMove = false;
            //rb.velocity = Vector3.zero;
            return;
        }
        // Vector3 newVelocity = differenceVector.normalized * movementSpeed * Time.fixedDeltaTime;
        // rb.velocity = newVelocity;
        Vector3 movement = differenceVector.normalized * movementSpeed * Time.deltaTime;
        transform.Translate(movement, Space.World);
        ClampPosition();
    }

    public virtual void SetUnitTarget(UnitControllerAPI target){ // Sets target to specified unit
        int faction = target.FACTION;
        if(faction == this.FACTION){ Debug.LogWarning("Target set to friendly unit, action aborted."); return; }
        UnitTarget = target;
        SetUnitPositionTarget(target.transform);
    }  

    private void SetUnitPositionTarget(Transform unitTarget){ // Use to set unit target position
        if (ReferenceEquals(unitTarget, transform))
            return;
        UnitTargetTransform = unitTarget;
        PositionTarget = UnitTargetTransform.position;
        if(buildingTarget != null){
            potentialBuildingTargets.Add(buildingTarget);
            buildingTarget = null;
        }
        shouldMove = true;
        moveAtUnit = true;
    }

    public void SetPositionTarget(Vector3 target){ // Use to set a non-unit target position
        target.z = 0;
        PositionTarget = target;
        shouldMove = true;
        moveAtUnit = false;
    }

    public virtual void SetDefaultStartPosition(){
        PositionTarget = transform.position;
    }

    public void SetBuildingTarget(Building building){
        if(ReferenceEquals(building, transform)){
            return;
        }
        this.buildingTarget = building;
        PositionTarget = building.transform.position;
        if(UnitTarget != null){
            potentialTargets.Add(UnitTarget);
            UnitTargetTransform = null;
        }
        shouldMove = true;
        moveAtUnit = false;
    }

    public virtual void RemoveTarget(){ // Removes current target
        UnitTarget = null;
        buildingTarget = null;
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
        }
    }

    public void AddTarget(UnitControllerAPI otherUnit){
        if(otherUnit.FACTION == FACTION || potentialTargets.Contains(otherUnit)){ return; }
        if(UnitTarget == null){ // Check if unit doesn't have an active target
            SetUnitTarget(otherUnit);
            EngageCombat();  // Engage in combat
        }else if(UnitTarget == otherUnit){
            ResetCombat();
        }else{
            potentialTargets.Add(otherUnit);
        }
    }

    public void AddTarget(Building otherBuilding){
        if(otherBuilding.FACTION == FACTION || potentialBuildingTargets.Contains(otherBuilding)){ return; }
        if(buildingTarget == null && UnitTarget == null){
            SetBuildingTarget(otherBuilding);
            EngageCombat();
        }
        else{
            potentialBuildingTargets.Add(buildingTarget);
        }
    }
    public virtual void OnTriggerExit2D(Collider2D other){
        string otherTag = other.tag;
        if(otherTag.Equals("UnitRange")){ // If caused by unit range, ignore
            return;
        }
    }

    private void OnDestroy(){
        StopAllCoroutines();
    }
}
