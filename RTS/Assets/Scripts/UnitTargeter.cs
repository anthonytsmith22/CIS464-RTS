using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class UnitTargeter : MonoBehaviour
{
    CircleCollider2D range;
    [SerializeField] UnitControllerAPI controller;
    int faction;

    private void Start(){
        range = GetComponent<CircleCollider2D>();
        range.radius = controller.range;
        range.isTrigger = true;
        faction = controller.FACTION;
    }

    private void OnTriggerEnter2D(Collider2D other){
        string otherTag = other.tag;
        if(otherTag.Equals("Unit")){
            UnitControllerAPI otherUnit = other.transform.GetComponent<UnitControllerAPI>();
            int otherFaction = otherUnit.FACTION;
            if(faction != otherFaction){
                controller.AddTarget(otherUnit);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other){
        string otherTag = other.transform.tag;
        if(otherTag.Equals("Unit")){
            UnitControllerAPI otherUnit = other.transform.GetComponent<UnitControllerAPI>();
            if(controller.potentialTargets.Contains(otherUnit)){
                controller.potentialTargets.Remove(otherUnit);
            }
        }
    }
}
