using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLens : MonoBehaviour {

    private LightBeam red;
    private LightBeam blue;
    private LightBeam green;

    private LightBeam white;


    // Use this for initialization
    void Awake () {
        GameObject tempObj = Instantiate(Resources.Load("LightBeam"), transform.position, Quaternion.Euler(0, -45, 0) * transform.rotation, transform) as GameObject;
        red = tempObj.GetComponent<LightBeam>();
        red.SetColor(AbstractOpticalElement.blue);

        tempObj = Instantiate(Resources.Load("LightBeam"), transform.position, transform.rotation, transform) as GameObject;
        green = tempObj.GetComponent<LightBeam>();
        green.SetColor(AbstractOpticalElement.green);

        tempObj = Instantiate(Resources.Load("LightBeam"), transform.position, Quaternion.Euler(0, 45, 0) * transform.rotation, transform) as GameObject;
        blue = tempObj.GetComponent<LightBeam>();
        blue.SetColor(AbstractOpticalElement.red);

        tempObj = Instantiate(Resources.Load("LightBeam"), transform.position, Quaternion.Euler(0, 180, 0) * transform.rotation, transform) as GameObject;
        white = tempObj.GetComponent<LightBeam>();
        white.SetColor(Color.white);
    }
}
