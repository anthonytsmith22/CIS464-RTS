using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{

    public GameObject menu, buildUI;
    public static bool paused = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!paused)
                Pause();
            else
                Resume();
        }
    }

    public void Resume()
    {
        Time.timeScale = 1.0f;
        menu.SetActive(false);
        paused = false;
    }

    public void Pause()
    {
        menu.SetActive(true);
        Time.timeScale = 0.0f;
        paused = true;
    }

    public void Quit()
    {
        Resume();
        SceneManager.LoadScene("MainMenu");
    }
}
