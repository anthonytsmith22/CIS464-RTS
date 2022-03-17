using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class KeyGate : MonoBehaviour
{
    #region Singleton
    private static KeyGate instance;
    public static KeyGate Instance { get { return instance; } }
    void Awake()
    {
        if(instance != null && instance != this)
        {
            Debug.LogWarning("More than one instance of KeyGate found!");
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    [SerializeField] List<KeyDroneController> keyDrones = new List<KeyDroneController>();
    [SerializeField] List<KeyDroneController> keyDronesWithKey = new List<KeyDroneController>();
    public int numKeys = 5;


    private void OnTriggerEnter2D(Collider2D other){
        Debug.Log("Entered Gate");
        if(other.tag.Equals("Unit")){
            KeyDroneController keyDrone = other.GetComponent<KeyDroneController>();
            if(keyDrone == null){
                return;
            }
            Debug.Log("KeyDrone entered gate");
            if(keyDrone.HasKey){
                keyDronesWithKey.Add(keyDrone);
                OnKeyHeldStatusChanged();
            }
            else{
                keyDrones.Add(keyDrone);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other){
        if(other.name.Equals("KeyDrone")){
            KeyDroneController keyDrone = other.GetComponent<KeyDroneController>();
            if(keyDrones.Contains(keyDrone)){
                keyDrones.Remove(keyDrone);
            }
            if(keyDronesWithKey.Contains(keyDrone)){
                keyDronesWithKey.Remove(keyDrone);
                if(winCountDownRunning){
                    StopCoroutine(WinRoutine);
                }
            }
        }
    }

    Coroutine WinRoutine;
    public void OnKeyHeldStatusChanged(){
        // Stop win countdown if one is running
        if(winCountDownRunning){
            StopCoroutine(WinRoutine);
        }
        // Iterate through keyDrones to see if they have a key
        foreach(KeyDroneController keyDrone in keyDrones){
            if(keyDrone.HasKey){
                keyDronesWithKey.Add(keyDrone);
                keyDrones.Remove(keyDrone);
            }
        }
        // Iterate through keyDronesWithKey to see if they are all of the same faction
        if(keyDronesWithKey.Count < numKeys){
            // If number of keyDrones with keys is less than number of keys, not all  keys have been brought
            return;
        }
        int faction = keyDronesWithKey[0].FACTION;
        foreach(KeyDroneController keyDrone in keyDronesWithKey){
            if(keyDrone.FACTION != faction){
                return;
            }
        }
        // If have not returned, all keydrones with key is of same faction, start win countdown
        WinRoutine = StartCoroutine(StartWinCountdown(faction));
    }

    private bool winCountDownRunning = false;
    private float winCountDownTime = 5f;
    private IEnumerator StartWinCountdown(int winningFaction){
        winCountDownRunning = true;
        yield return new WaitForSeconds(winCountDownTime);
        winCountDownRunning = false;
        if(winningFaction == 0){
            // Player won
            GameManager.Instance.WIN();
        }else{
            GameManager.Instance.LOSE();
        }
    }
}
