using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Mirror : AbstractOpticalElement
{

    public Mirror()
    {
        state = ElementState.ACTIVE;
        error = ErrorState.NONE;

        //Input and OutputLights
        inputList = new LinkedList<LightBeam>();
        outputList = new LinkedList<LightBeam>();
        max_lightInput = 20;

        //InputPlayers
        playerArray = new Player[2];
        max_PlayerNr = 2;
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

    // CHECKS amount and color of input beams
    protected override bool CorrectInput()
    {
        if (inputList.First == null)
        {
            return false;
        }

        return true;
    }

    //CALCULATES output light(s)
    protected override void CalculateOutput()
    {
        // iterate over all inputs
        for (int i = 0; i < inputList.Count; i++)
        {
            LightBeam input = inputList.ElementAt(i);

            // if there are too little beams in the output list, create one more
            if (outputList.Count == 0 || outputList.Count < i + 1)
            {
                GameObject tempObj = Instantiate(Resources.Load("LightBeam"), transform.position, transform.rotation, transform) as GameObject;
                outputList.AddLast(tempObj.GetComponent<LightBeam>());
            }

            // hit point
            outputList.ElementAt(i).transform.position = new Vector3(input.GetRaycastHit().point.x, 0, input.GetRaycastHit().point.z);

            // color
            outputList.ElementAt(i).SetColor(input.GetColor());

            // direction
            Vector3 mirrorAxis = Vector3.Dot(transform.right, input.GetDirection()) / Mathf.Sqrt(transform.right.magnitude) * transform.right;
            Vector3 mirrorNormal = Vector3.Dot(transform.forward, input.GetDirection()) / Mathf.Sqrt(transform.forward.magnitude) * transform.forward;

            outputList.ElementAt(i).SetDirection(mirrorAxis - mirrorNormal);

            // set active
            outputList.ElementAt(i).Enable();
        }

        if (outputList.Count >= inputList.Count)
        {
            for(int i = inputList.Count; i < outputList.Count; i++)
            {
                outputList.ElementAt(i).Disable();
            }
                
        }
    }


    public int GetMaxLightInput()
    {
        return max_lightInput;
    }
}