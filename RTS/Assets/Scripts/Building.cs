using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Building : MonoBehaviour
{

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
            BuildingUI.SetActive(value);
            selected = value;
            if (value)
                OnSelected.Invoke();
        } 
    }

    int hp;
    public int HP { get => hp; set {
        hp = value;
        HPBar.value = ((float)hp / Stats.MaxHP);
        if (hp < 0)
            explode();
    } }

    // Start is called before the first frame update
    void Start()
    {
        HP = Stats.MaxHP;
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
}
