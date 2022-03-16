using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }
    void Awake()
    {
        if(instance != null && instance != this)
        {
            Debug.LogWarning("More than one instance of GameManager found!");
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    [SerializeField] int numFactions; 
    private void Start(){

    }

    [SerializeField] GameObject FactionPrefab;
    [SerializeField] List<Transform> FactionSpawnPoints;
    [SerializeField] GameObject PlayerPrefab;
    private void InitializeFactions(){
        GameObject player = Instantiate(PlayerPrefab, FactionSpawnPoints[0].position, Quaternion.identity);
        Player playerController = player.GetComponent<Player>();
        playerController.Setup(0);

        GameObject faction;
        for(int i = 0; i < numFactions; i++){
            faction = Instantiate(FactionPrefab, FactionSpawnPoints[i+1].position, Quaternion.identity);
            AIAgent factionAgent = faction.GetComponent<AIAgent>();
            factionAgent.Setup(i+1);
        }
    }

    
}
