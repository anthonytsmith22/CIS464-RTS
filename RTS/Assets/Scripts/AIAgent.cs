using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAgent : Player
{
    // An individual AI player
    public Player playerController;
    private List<Player> otherFactions = new List<Player>(); 
    private List<FactoryScript> Factories = new List<FactoryScript>();

    public List<GameObject> BuildingPrefabs;

    private float buildTimer = 0f;

    public enum CurrentState
    {
        GrowPower,
        GrowArmy,
        Attacking,
        Defending
    };

    public CurrentState State;
    Player CurrentTarget = null;

    private void Awake(){
        IsPlayer = false;
        playerController = GameObject.Find("Player").GetComponent<Player>();
    }

    // Start is called before the first frame update
    void Start()
    {
        otherFactions.Add(playerController);
        State = CurrentState.GrowPower;
    }

    // Update is called once per frame
    float retargetWaitTime = 15f;
    float retargetTime = 0f;
    void Update()
    {
        

        // Determine who to attack
        // foreach (Player otherFaction in otherFactions)
        // {
        //     float threat = calcThreatLevel(otherFaction);
        //     if (threat > 1000.0f)
        //     {
        //         State = CurrentState.Attacking;
        //         CurrentTarget = otherFaction;
        //     }
        //     threatRating
        // }
        bool isAttacking;
        threatRating = calcThreatLevel(playerController);
        if(threatRating > 10000.0f){
            State = CurrentState.Attacking;
            CurrentTarget = playerController;
            isAttacking = true;
        }else{
            isAttacking = false;
        }

        if(isAttacking){
            retargetTime += Time.deltaTime;
            if(retargetTime >= retargetWaitTime){
                attack();
            }
        }else{
            retargetTime = 0.0f;
        }

        // no point in attacking if the army is way too small, 1.5x buffer zone since the AI won't be nearly as intelligent as the player at unit manipulation
        if (GetNumDrones() < playerController.GetNumDrones() * 1.5f && playerController.GetNumDrones() != 0)
        {
            State = CurrentState.GrowArmy;
        }

        buildTimer += Time.deltaTime;

        // power takes priority over units
        if (PowerManagement.consumption == 0.0f)
        {
            State = CurrentState.GrowArmy;
        }
        else if (PowerManagement.Satisfaction < 1.0f)
        {
            State = CurrentState.GrowPower;
        }
        else
        {
            State = CurrentState.GrowArmy;
        }

        

        switch (State)
        {
        case CurrentState.GrowPower:
            growPower();
            break;
        case CurrentState.GrowArmy:
            growArmy();
            break;
        case CurrentState.Attacking:
            attack();
            break;
        case CurrentState.Defending:
            defend();
            break;
        }

        
    }

    void createBuilding(int id)
    {
        if (buildTimer > 5)
        {
            buildTimer = 0f;
            Vector3 position = Vector3.zero;

            const float range = 10f; 

            position.x = Mathf.Floor(Random.Range(-range, range));
            position.y = Mathf.Floor(Random.Range(-range, range));


            BuildingController.SpawnBuilding(transform.position + position, BuildingPrefabs[id]);
        }
    }

    void growPower()
    {
        // call upon BuildController to make new Power Plants and Power Towers
        createBuilding(1);
    }

    void growArmy()
    {
        // call upon BuildController to make new Factories
        // createFactories();
        createBuilding(0);
        // queueUnits();
    }

    void attack()
    {
        /*
            foreach (var unit in Units)
            {
                unit.QueueCommand(CurrentTarget.SpawnPos);
            }

        */
        foreach(UnitControllerAPI unit in fastDrones){
            unit.SetPositionTarget(playerController.transform.position);
        }
        foreach(UnitControllerAPI unit in mediumDrones){
            unit.SetPositionTarget(playerController.transform.position);
        }
        foreach(UnitControllerAPI unit in heavyDrones){
            unit.SetPositionTarget(playerController.transform.position);
        }
    }

    void defend()
    {

    }

    // computes the threat level of a given player / AI agent
    float calcThreatLevel(Player otherFaction)
    {
        float threat = 0.0f;

        // them is a generic Player component to be passed in as a parameter, could be another AI or a player

        // prioritize closer players
        // threat += 1.0f / (them.SpawnPos - this.SpawnPos).magnitude * 100.0f;
        threat += 1.0f / (otherFaction.transform.position - transform.position).magnitude * 100.0f;
        // threat += them.KeyCount * 1000.0f;
        threat += otherFaction.HeldKeys.Count * 1000.0f;
        // threat += them.UnitCount * 300.0f;
        threat += otherFaction.GetNumDrones() * 300.0f;
        // threat += them.PowerProduction * 200.0f;
        threat += otherFaction.PowerManagement.production * 200.0f;

        return threat;
    }

}
