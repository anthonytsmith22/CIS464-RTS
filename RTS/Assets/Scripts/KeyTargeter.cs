using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class KeyTargeter : MonoBehaviour
{
    CircleCollider2D range;
    [SerializeField] KeyDroneController controller;
    [SerializeField] KeyCollector collector;

    private void Awake(){
        range = GetComponent<CircleCollider2D>();
        range.radius = 5f;
        range.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other){
        if(other.tag.Equals("Key")){
            if(collector.HeldKey == null){
                controller.SetKeyTarget(other.transform);
            }
        }
    }

}
