using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Settings : AManager<Settings>
{
    [System.Serializable]
    public class GameData
    {
        public string ServerIp;
        public int Port;
        public string NetworkType;
        public int ResolutionX;
        public int ResolutionY;
        public string FullScreen;
        public int TargetFrameRate;
        public int RefreshRate;
        public float WallFOV;
        public string WallPos;
        public string WallRot;

        // CITY LIGHTS FLOOR SETTINGS
        public float PlayerPlayerThreshold;
        public float PlayerObjectThreshold;
        public float MoveThreshold;
        public float SecondsToPlaceObject;

        // CITY LIGHTS WALL SETTINGS
        public float VerticalObliqueness;
        public float HorizontalObliqueness;
    }

    public GameData LoadedData;
    private string gameDataFileName = "/settings.txt";
    public Vector3 WallPos;
    public Vector3 WallRot;
    public Text output;

    // FPS Count
    private float deltaTime = 0.0f;

    void Awake()
    {
        LoadGameData();
    }

    private void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        PrintText();
    }

    private void LoadGameData()
    {

        // Path.Combine combines strings into a file path
        // Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build
        string filePath = Application.streamingAssetsPath + gameDataFileName;


        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            // Pass the json to JsonUtility, and tell it to create a GameData object from it
            //LoadGameData loadedData = JsonUtility.FromJson<GameData>(dataAsJson);
            LoadedData = JsonUtility.FromJson<GameData>(dataAsJson);
            WallPos = GetVector(LoadedData.WallPos);
            WallRot = GetVector(LoadedData.WallRot);
            Screen.SetResolution(LoadedData.ResolutionX, LoadedData.ResolutionY, GetBool(LoadedData.FullScreen), LoadedData.RefreshRate);
            Application.targetFrameRate = LoadedData.TargetFrameRate;
            Invoke("PrintText", 2f);

            // CITY LIGHTS SETTINGS
            if (LoadedData.NetworkType.Equals("Server"))
            {
                Player.PlayerObjectThreshold = LoadedData.PlayerObjectThreshold;
                Player.PlayerPlayerThreshold = LoadedData.PlayerPlayerThreshold;
                Player.moveThreshold = LoadedData.MoveThreshold;
                GroupInteraction.secondsToPlaceObject = LoadedData.SecondsToPlaceObject;
                RotationInteraction.secondsToPlaceObject = LoadedData.SecondsToPlaceObject;
            }
            else
            {
                Matrix4x4 matLeft = Camera.main.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
                Matrix4x4 matRight = Camera.main.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
                matLeft[0, 2] = LoadedData.HorizontalObliqueness;
                matRight[0, 2] = LoadedData.HorizontalObliqueness;
                matLeft[1, 2] = LoadedData.VerticalObliqueness;
                matRight[1, 2] = LoadedData.VerticalObliqueness;
                Camera.main.SetStereoProjectionMatrix(Camera.StereoscopicEye.Left, matLeft);
                Camera.main.SetStereoProjectionMatrix(Camera.StereoscopicEye.Left, matRight);
            }

        }
        else
        {
            output.text += "Can't log game data. Please check config file.";
            Debug.LogError("Can't log game data. Please check config file.");
        }
    }

    void PrintText()
    {
        output.text = "HELLO";
        output.text += "\nConnected as: " + LoadedData.NetworkType;
        output.text += "\n" + LoadedData.ServerIp + ": " + LoadedData.Port;
        output.text += "\n" + Screen.width + "x" + Screen.height + " " + Screen.currentResolution.refreshRate + "Hz " + Application.targetFrameRate + "FPS";
        output.text += "\nCam Pos " + WallPos;
        output.text += "\nCam Rot " + WallRot;

        if (LoadedData.NetworkType.Equals("Server"))
        {
            output.text += "\nPlayer Object Threshold: " + Player.PlayerObjectThreshold;
            output.text += "\nPlayer Player Threshold: " + Player.PlayerPlayerThreshold;
            output.text += "\nPlayer Move Threshold (squared * 10000): " + Player.moveThreshold;
            output.text += "\nSeconds to place object: " + LoadedData.SecondsToPlaceObject;
        }
        else
        {
            Matrix4x4 matLeft = Camera.main.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
            output.text += "\nCamera Frustum Vert. Obl.: " + matLeft[1, 2];
            if (Camera.main.stereoEnabled)
            {
                output.text += "\nStereo Convergence: " + Camera.main.stereoConvergence;
                output.text += "\nStereo Seperation: " + Camera.main.stereoSeparation;
            }
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            output.text += "\nFPS: " + string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        }


    }
    private Vector3 GetVector(string input)
    {
        string[] vectors = input.Split(',');
        float parsedX;
        float parsedY;
        float parsedZ;
        float.TryParse(vectors[0], out parsedX);
        float.TryParse(vectors[1], out parsedY);
        float.TryParse(vectors[2], out parsedZ);
        return new Vector3(parsedX, parsedY, parsedZ);
    }

    private bool GetBool(string input)
    {
        if (input == "true")
            return true;
        else return false;
    }
}