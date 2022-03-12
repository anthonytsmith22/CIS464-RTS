using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHealth : MonoBehaviour
{
    [SerializeField] private GameObject HealthBar;
    [SerializeField] private GameObject Bar;
    [SerializeField] private Vector3 Offset;
    [SerializeField] UnitControllerAPI unitController;
    public float MaxHealth { get; private set; }
    [SerializeField] private float CurrentHealth;

    private void Awake(){
        
    }

    private void Start(){
        MaxHealth = unitController.maxHealth;
        CurrentHealth = MaxHealth;
    }

    private void Update(){
        HealthBar.transform.position = transform.position + Offset;
    }

    public void DoDamage(float Damage)
    {
        CurrentHealth -= Damage;
        if(CurrentHealth <= 0.0f){
            UpdateHealthBarNormalized(0f);
            OnDeath();
        }else{
            float normalized = CurrentHealth/MaxHealth;
            UpdateHealthBarNormalized(normalized);
        }
    }

    private void UpdateHealthBarNormalized(float health){
        Bar.transform.localScale = new Vector3(health, 1f);
    }

    private void OnDeath(){}
}
