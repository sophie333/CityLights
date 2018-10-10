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

    float frameCount;
    float dt;
    float fps;
    float updateRate = 4;  // 4 updates per sec.

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }   // quit application
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetSceneAt(0).name);
        }   // reload scene

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
}
