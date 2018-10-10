using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Splitter : AbstractOpticalElement
{

    //comment from Malene:
    //must have an array for the vertices, otherwise, no correct quadrilateral can be drawn inside, it will mess up.
    //also the connecting lines from player to vertex work with those values
    //so [0-3] are like A, B, C and D for quadrilaterals
    //public Vector3[] vertices = new Vector3[4]; //I will set these in my code, at the point where I find out the correct order

    // blocked side is forward (positive z axis)

    public Splitter()
    {
        state = ElementState.ACTIVE;
        error = ErrorState.NONE;

        //Input and OutputLights
        inputList = new LinkedList<LightBeam>();
        outputList = new LinkedList<LightBeam>();
        max_lightInput = 1;

        //InputPlayers
        playerArray = new Player[4];
        max_PlayerNr = 4;
        overlappedElements = 0;
    }

    public new void Update()
    {
        base.Update();
        if (error != ErrorState.ERROROVERLAP)
        {
            UpdateOutput();
        }
    }

    // CHECKS amount of input beams
    protected override bool CorrectInput()
    {
        if (inputList.Count > 1 || inputList.First == null)
        {
            return false;
        }

        return true;
    }

    //CALCULATES output light(s)
    protected override void CalculateOutput()
    {
        LightBeam input = inputList.ElementAt(0);
        Debug.Log(inputList.ElementAt(0));
        LightBeam output1;
        LightBeam output2;

        // if output light beams don't yet exist, instantiate them
        if (outputList.Count == 0)
        {
            GameObject tempObj = Instantiate(Resources.Load("LightBeam"), transform.position, transform.rotation, transform) as GameObject;
            output1 = tempObj.GetComponent<LightBeam>();

            tempObj = Instantiate(Resources.Load("LightBeam"), transform.position, transform.rotation, transform) as GameObject;
            output2 = tempObj.GetComponent<LightBeam>();

            outputList.AddFirst(output1);
            outputList.AddLast(output2);

        }

        output1 = outputList.ElementAt(0);
        output2 = outputList.ElementAt(1);

        // color
        output1.SetColor(input.GetColor());
        output2.SetColor(input.GetColor());

        // direction
        // blocked side is forward (positive z axis)
        if (input.GetRaycastHit().collider.name == "LightColliderLeft")
        {
            output1.SetDirection(transform.right);
            output2.SetDirection(transform.forward * (-1));
        }
        else if (input.GetRaycastHit().collider.name == "LightColliderRight")
        {
            output1.SetDirection(transform.right * (-1));
            output2.SetDirection(transform.forward * (-1));
        }
        else if (input.GetRaycastHit().collider.name == "LightColliderBack")
        {
            output1.SetDirection(transform.right);
            output2.SetDirection(transform.right * (-1));
        }

        // set active
        output1.Enable();
        output2.Enable();

    }

    public int GetMaxLightInput()
    {
        return max_lightInput;
    }
}
