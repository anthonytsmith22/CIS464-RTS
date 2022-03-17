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
        InitializeFactions();
        InitializeKeys();
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

    [SerializeField] GameObject KeyPrefab;
    private void InitializeKeys(){
        int numKeys = KeyGate.Instance.numKeys;
        int counter;
        for(counter = 0; counter < numKeys; counter++){       
            Instantiate(KeyPrefab, KeyPosition(counter % regions.Count), Quaternion.identity);
        }
    }
    [SerializeField] List<Transform> regions = new List<Transform>();
    private Vector3 KeyPosition(int regionIndex){ // Set mix, max X and Z
        Transform region = regions[regionIndex];
        
        float centerX = region.position.x;
        float centerY = region.position.y;

        float scaleX = region.localScale.x;
        float scaleY = region.localScale.y;

        float minX = centerX - (scaleX/2);
        float maxX = centerX + (scaleX/2);

        float minY = centerY - (scaleY/2);
        float maxY = centerY + (scaleY/2);

        float coordX = Random.Range(minX, maxX);
        float coordY = Random.Range(minY, maxY);
        Vector3 keyPosition = new Vector3(coordX, coordY, 0f);
        return keyPosition;
    }

    public void WIN(){
        Debug.Log("PLAYER WON");
    }

    public void LOSE(){
        Debug.Log("PLAYER LOST");
    }
}
