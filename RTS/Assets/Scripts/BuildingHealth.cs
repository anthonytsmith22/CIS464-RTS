using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingHealth : MonoBehaviour
{
    [SerializeField] private GameObject HealthBar;
    [SerializeField] private GameObject Bar;
    [SerializeField] private Vector3 Offset;
    [SerializeField] Building buildingController;
    public float MaxHealth { get; private set; }
    [SerializeField] private float CurrentHealth;

    private void Start(){
        MaxHealth = buildingController.Stats.MaxHP;
        CurrentHealth = MaxHealth;
        HealthBar.transform.position = transform.position + Offset;
    }

    public void DoDamage(float Damage)
    {
        CurrentHealth -= Damage;
        if(CurrentHealth <= 0.0f){
            UpdateHealthBarNormalized(0f);
            OnDeath();
            Destroy(gameObject);
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
