using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using UnityTuio;

//only for the floor
public class GameManager : AManager<GameManager>
{
    [Header("Objects")]
    public Text DebugText;
    public Text FPSText;
    public Camera MainCam;
    //only for the floor:
    //public GameObject TrackingManager;

    float frameCount;
    float dt;
    float fps;
    float updateRate = 4;  // 4 updates per sec.

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            SceneManager.LoadScene(SceneManager.GetSceneAt(0).name);
        }

        // SHORTCUTS FOR TESTING
        if (Input.GetKeyDown(KeyCode.A))
        {
            MainCam.GetComponent<Animator>().enabled = !MainCam.GetComponent<Animator>().enabled;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GameObject lamp = GameObject.Find("Port1-1");
            // enable the colored lamp mesh
            lamp.transform.Find("body_on").GetComponent<MeshRenderer>().enabled = !lamp.transform.Find("body_on").GetComponent<MeshRenderer>().enabled;
            // enable the associated light source
            lamp.transform.Find("light").GetComponent<Light>().enabled = !lamp.transform.Find("light").GetComponent<Light>().enabled;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GameObject lamp = GameObject.Find("Port1-2");
            // enable the colored lamp mesh
            lamp.transform.Find("body_on").GetComponent<MeshRenderer>().enabled = !lamp.transform.Find("body_on").GetComponent<MeshRenderer>().enabled;
            // enable the associated light source
            lamp.transform.Find("light").GetComponent<Light>().enabled = !lamp.transform.Find("light").GetComponent<Light>().enabled;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GameObject lamp = GameObject.Find("Port2-1");
            // enable the colored lamp mesh
            lamp.transform.Find("body_on").GetComponent<MeshRenderer>().enabled = !lamp.transform.Find("body_on").GetComponent<MeshRenderer>().enabled;
            // enable the associated light source
            lamp.transform.Find("light").GetComponent<Light>().enabled = !lamp.transform.Find("light").GetComponent<Light>().enabled;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            GameObject lamp = GameObject.Find("Port2-2");
            // enable the colored lamp mesh
            lamp.transform.Find("body_on").GetComponent<MeshRenderer>().enabled = !lamp.transform.Find("body_on").GetComponent<MeshRenderer>().enabled;
            // enable the associated light source
            lamp.transform.Find("light").GetComponent<Light>().enabled = !lamp.transform.Find("light").GetComponent<Light>().enabled;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Matrix4x4 mat = Camera.main.projectionMatrix;
            mat[1, 2] += 0.025f;
            Camera.main.projectionMatrix = mat;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Matrix4x4 mat = Camera.main.projectionMatrix;
            mat[1, 2] -= 0.025f;
            Camera.main.projectionMatrix = mat;
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            MainCam.stereoConvergence -= 0.25f;
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            MainCam.stereoConvergence += 0.25f;
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            MainCam.stereoSeparation -= 0.002f;
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            MainCam.stereoSeparation += 0.002f;
        }

        frameCount++;
        dt += Time.deltaTime;
        if (dt > 1 / updateRate)
        {
            fps = frameCount / dt;
            frameCount = 0;
            dt -= 1 / updateRate;
        }
        //FPSText.text = "FPS: " + fps.ToString("F0");
    }
}
