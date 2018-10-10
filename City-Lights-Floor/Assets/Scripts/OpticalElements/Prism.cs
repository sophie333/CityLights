 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Prism : AbstractOpticalElement
{

    public Prism()
    {
        state = ElementState.ACTIVE;
        error = ErrorState.NONE;

        //Input and OutputLights
        inputList = new LinkedList<LightBeam>();
        outputList = new LinkedList<LightBeam>();
        max_lightInput = 1;

        //InputPlayers
        playerArray = new Player[3];
        max_PlayerNr = 3;
        overlappedElements = 0;
    }

    public override void Update()
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
         if (inputList.Count > 1 || inputList.First == null)
        {
            return false;
        }

        var node = inputList.First;
        while (node != null)
        {
            if (!( node.Value.GetColor() == cyan || node.Value.GetColor() == magenta || node.Value.GetColor() == yellow ))
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
        LightBeam input = inputList.ElementAt(0);
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

        // COLOR
        if (input.GetColor() == cyan)
        {
            // Erstelle blauen und grünen Strahl
            output1.SetColor(blue);
            output2.SetColor(green);
        }
        else if (input.GetColor() == yellow)
        {
            // Erstelle roten und grünen Strahl
            output1.SetColor(green);
            output2.SetColor(red); 
        }
        else if (input.GetColor() == magenta)
        {
            // Erstelle roten und blauen Strahl
            output1.SetColor(red);
            output2.SetColor(blue);
        }
        else
        {
            ChangeError(ErrorState.ERRORINPUT);
        }

        // DIRECTION
        output1.SetDirection(Quaternion.Euler(0, -60, 0) * input.GetRaycastHit().collider.transform.forward);
        output2.SetDirection(Quaternion.Euler(0, 60, 0) * input.GetRaycastHit().collider.transform.forward);

        // SET ACTIVE
        output1.Enable();
        output2.Enable();
    }       

    public int GetMaxLightInput()
    {
        return max_lightInput;
    }
}