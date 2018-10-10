using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Network_LampPort : MonoBehaviour {
    public string colorName;
    public string lampName = "Port1-1"; //"Port + level-lamp"
    public NetworkHelper networker;

    protected LinkedList<LightBeam> inputList;
    protected Material bottom;
    protected Material cap;
    protected Sprite icon;
    protected SpriteRenderer iconRenderer;
    protected Color color;

    public Sprite iconOne;
    public Sprite iconTwo;
    public Sprite iconThree;
    public Sprite iconError;

    public bool rightInputColor = false;

    void Awake()
    {
        networker = FindObjectOfType<NetworkHelper>();

        bottom = transform.Find("Bottom").GetComponent<MeshRenderer>().material;
        cap = transform.Find("Cap").GetComponent<SpriteRenderer>().material;
        iconRenderer = transform.Find("Icon").GetComponent<SpriteRenderer>();
        inputList = new LinkedList<LightBeam>();

        // set color
        switch (colorName)
        {
            case "red":
                color = AbstractOpticalElement.red;
                break;
            case "yellow":
                color = AbstractOpticalElement.yellow;
                break;
            case "green":
                color = AbstractOpticalElement.green;
                break;
            case "cyan":
                color = AbstractOpticalElement.cyan;
                break;
            case "blue":
                color = AbstractOpticalElement.blue;
                break;
            case "magenta":
                color = AbstractOpticalElement.magenta;
                break;
        }

        cap.color = color;

        // set icon number
        string number = lampName.Substring(6, 1);
        switch(number)
        {
            case "1":
                icon = iconOne;
                break;
            case "2":
                icon = iconTwo;
                break;
            case "3":
                icon = iconThree;
                break;
        }

        iconRenderer.sprite = icon;

    }


    // ADDS a light to the list of lights, which are entering the object
    public virtual void AddInputLight(LightBeam light)
    {
        // check wheather light already exists in the list
        var node = inputList.First;
        bool newInput = true;
        while (node != null)
        {
            var nextNode = node.Next;
            if (node.Value.GetID() == light.GetID())
            {
                newInput = false;
            }
            node = nextNode;
        }

        // if light isn't already in the list, add it and call CheckInput()
        if (newInput)
        {
            inputList.AddLast(light);
            Debug.Log("Added " + light.GetID() + " to " + this);
            CheckInput();
        }
    }


    // REMOVES a light from the list of lights, which are entering the object
    public virtual void RemoveInputLight(LightBeam light)
    {
        var node = inputList.First;
        while (node != null)
        {
            var nextNode = node.Next;
            if (node.Value.GetID() == light.GetID())
            {
                inputList.Remove(node);
                Debug.Log("Removed " + light.GetID() + " from " + this);
            }
            node = nextNode;
        }

        CheckInput();

    }


    public void CheckInput()
    {
        if (inputList.Count == 1)
        {
            if (inputList.ElementAt(0).GetColor() == color)
            {
                // set correct input appearance
                bottom.color = color;
                iconRenderer.sprite = icon;

                Debug.Log("Network Lampport on");
                networker.TurnLightOn(lampName);
                //networker.AllLightsOn();  //Function still possible if we need it, only deactivated for now

                rightInputColor = true;
            }
            else
            {
                // set wrong input appearance
                bottom.color = Color.white;
                iconRenderer.sprite = iconError;

                Debug.Log("Network Lampport off");
                networker.TurnLightOff(lampName);
                //networker.AllLightsOff(); //Function still possible if we need it, only deactivated for now

                rightInputColor = false;
            }
        }
        else
        {
            bottom.color = Color.white;
            iconRenderer.sprite = icon;
        }
        
    }
}
