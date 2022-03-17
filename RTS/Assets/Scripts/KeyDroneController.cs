using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyDroneController : UnitControllerAPI
{
    [SerializeField] private GameObject KeyHolder; // Child gameObject where key will be placed
    [SerializeField] private KeyCollector collector;
    Key keyTarget;
    public bool HasKey;
    public override void Awake()
    {
        base.Awake();
    }

    private void Start(){
        HasKey = false;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void EngageCombat()
    {
        return;
    }

    public void SetKeyTarget(Transform target){
        if(HasKey){ return; } // If drone already has key, don't try to get another
        base.SetPositionTarget(target.position);
    }

    
}
