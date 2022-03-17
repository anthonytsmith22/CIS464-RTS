using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class KeyCollector : MonoBehaviour
{
    public Key HeldKey;
    private CircleCollider2D pickUpRange;
    [SerializeField] private KeyDroneController keyDroneController;

    private void Awake(){
        pickUpRange = GetComponent<CircleCollider2D>();
    }
    private void SetupCollector(){
        pickUpRange.isTrigger = true;
        pickUpRange.radius = keyDroneController.range;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        string otherTag = other.tag;
        if(otherTag.Equals("Key")){
            Key key = other.GetComponent<Key>();
            if(key.owned){
                // Is owned by another drone from either same faction or other
                return; // Do nothing, key drones cannot attack so cannot take from other drone
            }
            // ELSE pickup the key
            if(HeldKey == null){ // Check if drone owns a key already
                PickupKey(key);
            }
        }
    }

    private void PickupKey(Key key){
        // Parent key to drone
        HeldKey = key;
        key.owned = true;
        key.transform.SetParent(transform);
        keyDroneController.HasKey = true;
        // disable key meshrender
        //key.GetComponent<SpriteRenderer>().enabled = false;
        keyDroneController.movementSpeed /= 2.5f;
        KeyGate.Instance.OnKeyHeldStatusChanged();
    }

    private void OnDestroy(){
        if(HeldKey != null){ // Check if drone has a key when destroyed
            // Unparent key to where drone was destroyed then enable its mesh renderer
            Vector3 position = transform.position;
            HeldKey.transform.parent = null;
            HeldKey.transform.position = position;
            HeldKey.GetComponent<Key>().owned = false;
            //HeldKey.GetComponent<SpriteRenderer>().enabled = true;
            HeldKey = null;
            KeyGate.Instance.OnKeyHeldStatusChanged();
        }
    }
}
