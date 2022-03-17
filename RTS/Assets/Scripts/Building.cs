using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Building : MonoBehaviour
{
    public int FACTION;
    public BuildingStats Stats;
    public Slider HPBar;
    public GameObject BuildingUI;

    public UnityEvent OnSelected;
    private AudioSource explodeSfx;

    public bool Dead { get; private set; }

    void Awake()
    {
        explodeSfx = GetComponent<AudioSource>();
    }

    private bool selected;
    public bool Selected 
    { 
        get => selected; 
        set
        {
            //BuildingUI.SetActive(value);
            selected = value;
            if (value)
                OnSelected.Invoke();
        } 
    }

    // int hp;
    // public int HP { get => hp; set {
    //     hp = value;
    //     HPBar.value = ((float)hp / Stats.MaxHP);
    //     if (hp < 0)
    //         explode();
    // } }

    // Start is called before the first frame update
    public float CurrentHealth;
    void Start()
    {
        if (FACTION > 0)
        {
            var sprite = GetComponent<SpriteRenderer>();
            sprite.color = new Color(0.4f, 1.0f, 1.0f, 1.0f);
        }
        CurrentHealth = Stats.MaxHP;
        Dead = false;
        Selected = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void explode()
    {
        Dead = true;
        explodeSfx.Play();
    }

    public void TakeDamage(float damage){ // Damage logic for unit
        CurrentHealth -= damage;
        GetComponent<BuildingHealth>().DoDamage(damage); 
        if(CurrentHealth < 0f){
            CurrentHealth = 0f;
            
        }
    }
}
