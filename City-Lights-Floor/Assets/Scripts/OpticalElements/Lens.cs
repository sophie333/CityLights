using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Lens : AbstractOpticalElement {

    public Lens()
    {
        state = ElementState.ACTIVE;
        error = ErrorState.NONE;

        //Input and OutputLights
        inputList = new LinkedList<LightBeam>();
        outputList = new LinkedList<LightBeam>();
        max_lightInput = 2;

        //InputPlayers
        playerArray = new Player[3];
        max_PlayerNr = 3;
        overlappedElements = 0;

    }

    public new void Update()
    {
        base.Update();
        if (error != ErrorState.ERROROVERLAP) // changed - muss immer überprüft werden, da kein trigger mehr
        {
            UpdateOutput();
        }
    }


    // CHECKS amount and color of input beams
    protected override bool CorrectInput()
    {
        if (inputList.Count != 2)
        {
            return false;
        }

        var node = inputList.First;
        while (node != null)
        {
            if (!(node.Value.GetColor() == red || node.Value.GetColor() == green || node.Value.GetColor() == blue))
            {
                return false;
            }
            node = node.Next;
        }

        return true;
    }

    //CALCULATES output light(s)
    protected override void CalculateOutput()
    {
        LightBeam input1 = inputList.ElementAt(0);
        LightBeam input2 = inputList.ElementAt(1);
        LightBeam output;

        // if output light beams don't yet exist, instantiate them
        if (outputList.Count == 0)
        {
            GameObject tempObj = Instantiate(Resources.Load("LightBeam"), transform.position, transform.rotation, transform) as GameObject;
            output = tempObj.GetComponent<LightBeam>();

            outputList.AddFirst(output);

        }

        output = outputList.ElementAt(0);

        // COLOR
        if (input1.GetColor() == red && input2.GetColor() == green ||
            input2.GetColor() == red && input1.GetColor() == green)
        {
            output.SetColor(yellow);

        }
        else if (input1.GetColor() == red && input2.GetColor() == blue ||
                 input2.GetColor() == red && input1.GetColor() == blue)
        {
            output.SetColor(magenta);

        }
        else if (input1.GetColor() == green && input2.GetColor() == blue ||
                 input2.GetColor() == green && input1.GetColor() == blue)
        {
            output.SetColor(cyan);

        }
        else
        {
            ChangeError(ErrorState.ERRORINPUT);
        }

        // DIRECTION
        output.SetDirection(Vector3.Normalize(input1.GetDirection() + input2.GetDirection()));

        // SET ACTIVE
        output.Enable();
    }

    public int GetMaxLightInput()
    {
        return max_lightInput;
    }
}