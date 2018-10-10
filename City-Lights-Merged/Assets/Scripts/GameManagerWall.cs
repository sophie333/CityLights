using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
//using UnityTuio;

//only for the floor
public class GameManagerWall : AManager<GameManagerWall>
{
    [Header("Objects")]
    public Text DebugText;
    public Text FPSText;
    public Camera MainCam;
    public GameObject PauseScreen;
    public AudioMixer audiomixer;
    public NetworkHelper networker;
    //only for the floor:
    //public GameObject TrackingManager;

    float volumeMusic = 0;
    float volumeInteraction = 0;
    float volumeAmbient = 0;

    public bool startscreen = false;
    private AudioManagerWall audiomanager;

    float frameCount;
    float dt;
    float fps;
    float updateRate = 4;  // 4 updates per sec.

    private bool gamePaused = false;

    void Awake()
    {
        networker = FindObjectOfType<NetworkHelper>();
        audiomanager = (AudioManagerWall)GameObject.FindObjectOfType<AudioManagerWall>();
        Cursor.visible = false;
    }

    void Update()
    {
        // rotate sky
        // Sets the float value of "_Rotation", adjust it by Time.time and a multiplier.
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * 0.3f);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }   // quit application

        if (Input.GetKeyDown(KeyCode.R ))
        {
            SceneManager.LoadScene(SceneManager.GetSceneAt(0).name);
            networker.ReloadGame();
        }   // reload game

        if (Input.GetKeyDown(KeyCode.L))
        {
            networker.ReloadLevel();

            int level = FindObjectOfType<LevelManagerWall>().level;

            if (level < 5 && level > 0)
            {
                string lamp1 = "Port" + level + "-1";
                string lamp2 = "Port" + level + "-2";

                GameObject.Find(lamp1).GetComponent<Animator>().SetBool("lightOn", false);
                GameObject.Find(lamp2).GetComponent<Animator>().SetBool("lightOn", false);
            }

        }   // reload level

        if (Input.GetKeyDown(KeyCode.P))
        {
            gamePaused = !gamePaused;
            PauseScreen.GetComponent<Animator>().SetBool("pause", gamePaused);
            networker.GamePause(gamePaused);
            
        }   // P - Pause/Unpause

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            DebugText.enabled = !DebugText.enabled;
            FPSText.enabled = !FPSText.enabled;
            networker.ToggleDebug();
        }   // 0 - enable/disable Debug Text

        // SHORTCUTS FOR TESTING
        if (Input.GetKeyDown(KeyCode.N))
        {
            MainCam.GetComponent<Animator>().SetTrigger("nextLevel");
            GameObject.FindObjectOfType<LevelManagerWall>().NextLevel();
            networker.NextLevelServer();

        }   // start/stop camera animation
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Animator animator = GameObject.Find("Port1-1").GetComponent<Animator>();
            if (animator.GetBool("lightOn"))
            {
                animator.SetBool("lightOn", false);
            }
            else
            {
                animator.SetBool("lightOn", true);
            }

        }   // light 1-1 on/off
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Animator animator = GameObject.Find("Port1-2").GetComponent<Animator>();
            if (animator.GetBool("lightOn"))
            {
                animator.SetBool("lightOn", false);
            }
            else
            {
                animator.SetBool("lightOn", true);
            }

        }   // light 1-2 on/off
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Animator animator = GameObject.Find("Port2-1").GetComponent<Animator>();
            if (animator.GetBool("lightOn"))
            {
                animator.SetBool("lightOn", false);
            }
            else
            {
                animator.SetBool("lightOn", true);
            }

        }   // light 2-1 on/off
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Animator animator = GameObject.Find("Port2-2").GetComponent<Animator>();
            if (animator.GetBool("lightOn"))
            {
                animator.SetBool("lightOn", false);
            }
            else
            {
                animator.SetBool("lightOn", true);
            }

        }   // light 2-2 on/off
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Animator animator = GameObject.Find("Port3-1").GetComponent<Animator>();
            if (animator.GetBool("lightOn"))
            {
                animator.SetBool("lightOn", false);
            }
            else
            {
                animator.SetBool("lightOn", true);
            }

        }   // light 3-1 on/off
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            Animator animator = GameObject.Find("Port3-2").GetComponent<Animator>();
            if (animator.GetBool("lightOn"))
            {
                animator.SetBool("lightOn", false);
            }
            else
            {
                animator.SetBool("lightOn", true);
            }

        }   // light 3-2 on/off
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            Animator animator = GameObject.Find("Port4-1").GetComponent<Animator>();
            if (animator.GetBool("lightOn"))
            {
                animator.SetBool("lightOn", false);
            }
            else
            {
                animator.SetBool("lightOn", true);
            }

        }   // light 3-1 on/off
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            Animator animator = GameObject.Find("Port4-2").GetComponent<Animator>();
            if (animator.GetBool("lightOn"))
            {
                animator.SetBool("lightOn", false);
            }
            else
            {
                animator.SetBool("lightOn", true);
            }

        }   // light 3-2 on/off
        if (Input.GetKeyDown(KeyCode.X))  
        {

            Matrix4x4 matLeft = Camera.main.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
            Matrix4x4 matRight = Camera.main.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
            matLeft[1, 2] += 0.025f;
            matRight[1, 2] += 0.025f;
            Camera.main.SetStereoProjectionMatrix(Camera.StereoscopicEye.Left, matLeft);
            Camera.main.SetStereoProjectionMatrix(Camera.StereoscopicEye.Left, matRight);

        }   // vert. camera crustum obliqu. +0.025
        if (Input.GetKeyDown(KeyCode.D))
        {

            Matrix4x4 matLeft = Camera.main.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
            Matrix4x4 matRight = Camera.main.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
            matLeft[1, 2] -= 0.025f;
            matRight[1, 2] -= 0.025f;
            Camera.main.SetStereoProjectionMatrix(Camera.StereoscopicEye.Left, matLeft);
            Camera.main.SetStereoProjectionMatrix(Camera.StereoscopicEye.Left, matRight);

        }   // vert. camera crustum obliqu. -0.025
        if (Input.GetKeyDown(KeyCode.C))
        {
            MainCam.stereoConvergence -= 0.25f;
        }   // stereo convergence -0.25
        if (Input.GetKeyDown(KeyCode.F))
        {
            MainCam.stereoConvergence += 0.25f;
        }   // stereo convergence +0.25
        if (Input.GetKeyDown(KeyCode.V))
        {
            MainCam.stereoSeparation -= 0.002f;
        }   // stereo seperation -0.002
        if (Input.GetKeyDown(KeyCode.G))
        {
            MainCam.stereoSeparation += 0.002f;
        }   // stereo seperation +0.002

        if (Input.GetKey(KeyCode.M) && Input.GetKeyDown(KeyCode.UpArrow))
        {
            volumeMusic++;
            audiomixer.SetFloat("VolumeMusic", volumeMusic);
            Debug.Log("Increase Music Volume");
        }
        if (Input.GetKey(KeyCode.M) && Input.GetKeyDown(KeyCode.DownArrow))
        {
            volumeMusic--;
            audiomixer.SetFloat("VolumeMusic", volumeMusic);
            Debug.Log("Decrease Music Volume");
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            audiomixer.SetFloat("VolumeMusic", -60);
            Debug.Log("Mute Music");
        }

        if (Input.GetKey(KeyCode.I) && Input.GetKeyDown(KeyCode.UpArrow))
        {
            volumeMusic++;
            audiomixer.SetFloat("VolumeInteraction", volumeInteraction);
        }
        if (Input.GetKey(KeyCode.I) && Input.GetKeyDown(KeyCode.DownArrow))
        {
            volumeMusic--;
            audiomixer.SetFloat("VolumeInteraction", volumeInteraction);
        }

        if (Input.GetKey(KeyCode.A) && Input.GetKeyDown(KeyCode.UpArrow))
        {
            volumeMusic++;
            audiomixer.SetFloat("VolumeAmbient", volumeAmbient);
        }
        if (Input.GetKey(KeyCode.A) && Input.GetKeyDown(KeyCode.DownArrow))
        {
            volumeMusic--;
            audiomixer.SetFloat("VolumeAmbient", volumeAmbient);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            startscreen = true;
            audiomanager.PlayFloor("F_Create");
        }
        if (Input.GetKey(KeyCode.Return))
        {
            LevelManagerWall levelmanager = GameObject.FindObjectOfType<LevelManagerWall>();

            if (levelmanager.level == 0)
            {
                MainCam.GetComponent<Animator>().SetTrigger("nextLevel");
                levelmanager.NextLevel();
                levelmanager.Unpause();
            }
            
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
}
