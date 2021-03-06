using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject BuildMenu;
    public GameObject FactoryPanel;
    public UnitSelector selector;

    #region Singleton
    private static UIManager instance;
    public static UIManager Instance { get { return instance; } }
    void Awake()
    {
        if(instance != null && instance != this)
        {
            Debug.LogWarning("More than one instance of UIManager found!");
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    private void Start(){
        selector = GameObject.Find("Player").transform.GetComponent<UnitSelector>();
    }

    public void OpenFactoryUI(){
        FactoryPanel.SetActive(true);
        BuildMenu.SetActive(false);
    }

    public void CloseFactoryUI(){
        FactoryPanel.SetActive(false);
        BuildMenu.SetActive(true);
    }

    public void SelectUnitType(int type){
        Debug.Log("check");
        selector.selectedFactory.SetProductionTarget(type);
    }
}