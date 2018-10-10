using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityTuio;

//only for the floor
public class GameManagerFloor : AManager<GameManagerFloor>
{
    [Header("Objects")]
    public Text DebugText;
    public Text FPSText;
    public Camera MainCam;
    //only for the floor:
    public GameObject TrackingManager;
    public GameObject pauseOverlay;

    float frameCount;
    float dt;
    float fps;
    float updateRate = 4;  // 4 updates per sec.

    public bool gamePaused = false;

    private void Awake()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }   // quit application
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadGame();
        } 

        frameCount++;
        dt += Time.deltaTime;
        if (dt > 1 / updateRate)
        {
            fps = frameCount / dt;
            frameCount = 0;
            dt -= 1 / updateRate;
        }
        FPSText.text = "FPS: " + fps.ToString("F0");
    }

    public void ReloadGame()
    {
        SceneManager.LoadScene("Level01");
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetSceneAt(0).name);
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
