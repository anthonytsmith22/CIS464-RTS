using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAgent : Player
{
    // An individual AI player
    private Player playerController;
    private List<Player> otherFactions; 
    private List<FactoryScript> Factories = new List<FactoryScript>();
    enum CurrentState
    {
        GrowPower,
        GrowArmy,
        Attacking,
        Defending
    };

    CurrentState State;
    Player CurrentTarget = null;

    private void Awake(){
        otherFactions.Add(playerController);
    }

    // Start is called before the first frame update
    void Start()
    {
        State = CurrentState.GrowPower;
    }

    // Update is called once per frame
    void Update()
    {
        

        // Determine who to attack
        foreach (Player otherFaction in otherFactions)
        {
            float threat = calcThreatLevel(otherFaction);
            if (threat > 1000.0f)
            {
                State = CurrentState.Attacking;
                CurrentTarget = otherFaction;
            }

        }

        // no point in attacking if the army is way too small, 1.5x buffer zone since the AI won't be nearly as intelligent as the player at unit manipulation
        if (GetNumDrones() < playerController.GetNumDrones() * 1.5f)
        {
            State = CurrentState.GrowArmy;
        }

        

        // power takes priority over units
        if (PowerManagement.Satisfaction < 1.0f)
        {
            State = CurrentState.GrowPower;
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

    void growPower()
    {
        // call upon BuildController to make new Power Plants and Power Towers

    }

    void growArmy()
    {
        // call upon BuildController to make new Factories
        // createFactories();
        Vector3 newFactoryPosition;
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
