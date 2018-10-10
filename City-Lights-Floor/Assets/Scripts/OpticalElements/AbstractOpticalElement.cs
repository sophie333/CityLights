using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public abstract class AbstractOpticalElement : MonoBehaviour
{
    // State Management
    public enum ElementState { ACTIVE, INACTIVE, MOVE, WAIT, ROTATE };
    public enum ErrorState { NONE, ERRORINPUT, ERROROVERLAP };
    protected ElementState state;
    protected ErrorState error;

    // Light In- & Output
    protected LinkedList<LightBeam> inputList;
    protected LinkedList<LightBeam> outputList;
    protected int max_lightInput;

    // Player Input
    protected float logInTime = 2f;
    protected float timeEntered;

    protected int max_PlayerNr;
    protected int playerActive = 0;
    protected Player[] playerArray;
    
    protected int overlappedElements;

    // colors
    protected static float saturation = 0.75f;
    protected static float value = 1.0f;
    public static Color red = Color.HSVToRGB(0f, saturation, value);
    public static Color yellow = Color.HSVToRGB(1/6f, saturation, value);
    public static Color green = Color.HSVToRGB(2/6f, saturation, value);
    public static Color cyan = Color.HSVToRGB(3/6f, saturation, value);
    public static Color blue = Color.HSVToRGB(4/6f, saturation, value);
    public static Color magenta = Color.HSVToRGB(5/6f, saturation, value);
    public static Color errorColor = new Color(0.84f, 0.27f, 0.27f);

    // overlay graphics
    protected SpriteRenderer frame;
    protected SpriteRenderer icon;
    protected SpriteRenderer[] vertices;
    public Image waitCircle;

    protected Material bottom;
    protected Material body;
    protected Material top;

    public Sprite iconMove;
    public Sprite iconRotate;
    public Sprite iconError;

    public Material bottomPlaced;
    public Material bottomEdit;

    public Material objectPlaced;
    public Material objectEdit;

    public Material frostedPlaced;
    public Material frostedEdit;

    private InteractionManager interactionManager;
    private NetworkHelper networker;


    public virtual void Awake()
    {
        // find overlay elements
        frame = transform.Find("Frame").GetComponent<SpriteRenderer>();
        icon = transform.Find("Icon").GetComponent<SpriteRenderer>();
        vertices = transform.Find("Vertices").GetComponentsInChildren<SpriteRenderer>();

        // find material property of the renderers of the element's parts
        bottom = transform.Find("Bottom").GetComponent<MeshRenderer>().material;
        body = transform.Find("Body").GetComponent<MeshRenderer>().material;
        top = transform.Find("Top").GetComponent<MeshRenderer>().material;

        interactionManager = GameObject.Find("InteractionManager").GetComponent<InteractionManager>();
        networker = FindObjectOfType<NetworkHelper>();
    }

    public virtual void Update()
    {
        //ROTATE Mode: 1 Player
        if (playerActive == 1 && state != ElementState.ROTATE && !(this is Lens))
        {
            if(Time.time - timeEntered > logInTime)
            {
                Debug.Log("ROTATE Mode activated");
                Debug.Log("Player :" +playerArray[0] + " added.");
                ChangeState(ElementState.ROTATE);
                interactionManager.AddRotationInteraction(this, playerArray[0]);
                timeEntered = 0;
            }
        }
    }

    public virtual void OnTriggerEnter(Collider other)
    {/*
        Debug.Log("ontriggerenter");
        Debug.Log("OTHER ELEMENT OnTriggerEnter: " + other.gameObject);

        if (other.tag == "Overlap" && (state == ElementState.MOVE || state == ElementState.ROTATE))
        {
            overlappedElements++;
            ChangeError(ErrorState.ERROROVERLAP);
        }*/
        
        AbstractOpticalElement opticalElement = other.gameObject.GetComponent<AbstractOpticalElement>();
        Lampport lampPort = other.gameObject.GetComponent<Lampport>();
        StartLens startLens = other.gameObject.GetComponent<StartLens>();
       
        // Collision with another OpticalElement or Lampport
        if (opticalElement || lampPort || startLens)
        {
            if (overlappedElements == 0 && (state == ElementState.MOVE || state == ElementState.ROTATE))
            {
                overlappedElements++;
                ChangeError(ErrorState.ERROROVERLAP);
            }
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {/*
        Debug.Log("OTHER ELEMENT OnTriggerExit: " + other.gameObject);

        if (other.tag == "Overlap" && (state == ElementState.MOVE || state == ElementState.ROTATE))
        {
            overlappedElements--;
            if (overlappedElements == 0)
            {
                ChangeError(ErrorState.NONE);
            }
        }*/
        
        AbstractOpticalElement opticalElement = other.gameObject.GetComponent<AbstractOpticalElement>();
        Lampport lampPort = other.gameObject.GetComponent<Lampport>();
        StartLens startLens = other.gameObject.GetComponent<StartLens>();

        // Ended Collision with another OpticalElement or Lampport
        if (opticalElement || lampPort || startLens)
        {
            if (overlappedElements == 0 && (state == ElementState.MOVE || state == ElementState.ROTATE))
            {
                overlappedElements--;
                ChangeError(ErrorState.NONE);
            }
        }
    }

    #region STATES AND ERRORS

    public void ChangeState(ElementState state)
    {

        if (state == ElementState.ACTIVE)
        {
            frame.enabled = false;
            icon.enabled = false;
            waitCircle.enabled = false;

            bottom = bottomPlaced;
            body = objectPlaced;
            top = frostedPlaced;

            // play sound
            if (this.state != ElementState.WAIT)
            {
                if (networker.connected) networker.PlayAudio("F_Place");            
                else FindObjectOfType<AudioManagerFloor>().Play("Place");
            }
            
        }
        else if (state == ElementState.MOVE)
        {
            frame.enabled = true;

            bottom = bottomEdit;
            body = objectEdit;
            top = frostedEdit;
        }
        else if (state == ElementState.WAIT)
        {
            frame.enabled = true;
            waitCircle.enabled = true;

            bottom = bottomEdit;
            body = objectEdit;
            top = frostedEdit;
        }
        else if (state == ElementState.ROTATE)
        {
            frame.enabled = true;

            bottom = bottomEdit;
            body = objectEdit;
            top = frostedEdit;
        }

        this.state = state;
    }

    public void ChangeError(ErrorState error)
    {
        this.error = error;

        if (error == ErrorState.NONE)
        {
            frame.color = Color.white;
            icon.color = Color.white;

            if (state == ElementState.ACTIVE)
            {
                icon.enabled = false;
            }
            else if (state == ElementState.MOVE)
            {
                icon.sprite = iconMove;
                icon.enabled = true;
            }
            else if (state == ElementState.ROTATE)
            {
                icon.sprite = iconRotate;
                icon.enabled = true;
            }
        }
        else if (error == ErrorState.ERROROVERLAP)
        {
            frame.color = errorColor;
            frame.enabled = true;

            icon.sprite = iconError;
            icon.color = errorColor;
            icon.enabled = true;

            if (networker.connected) networker.PlayAudio("F_Error");
            else FindObjectOfType<AudioManagerFloor>().Play("Error");
        }
        else if (error == ErrorState.ERRORINPUT)
        {
            icon.sprite = iconError;
            icon.enabled = true;

            if (networker.connected) networker.PlayAudio("F_Error");
            else FindObjectOfType<AudioManagerFloor>().Play("Error");
        }
    }
    #endregion


    #region PlayerHandler

    //ADDS a player to the list of players, which are grabbing the object
    public virtual bool AddPlayer(Player player)
    {
        for (int i = 0; i < playerArray.Length; i++)
        {
            if (playerArray[i] == null)
            {
                Debug.Log("Player added");
                playerArray[i] = player;
                player.isAvailable = false;
                playerActive++;
                CheckPlayerNr();
                return true;
            }
        }
        return false;
    }


    //REMOVES a player from the list of players, which are grabbing the object
    public virtual bool RemovePlayer(int playerID)
    {
        for (int i = 0; i < playerArray.Length; i++)
        {
            if (playerArray[i] != null && playerArray[i].GetID() == playerID)
            {
                Debug.Log("Player removed");
                playerArray[i].isAvailable = true;
                playerArray[i] = null;
                playerActive--;
                CheckPlayerNr();
                return true;
            }
        }        
        return false;
    }


    //CHANGES STATES according to the number of Players, which are logged in
    public virtual void CheckPlayerNr()
    {
        //ROTATE Mode: 1 player
        if (playerActive == 1)
        {
            timeEntered = Time.time;
        }
        else
        {
            timeEntered = 0;
        }
        //WAIT Mode: too little player
        if (playerActive != 0 && playerActive < max_PlayerNr)
        {
            Debug.Log("WAIT Mode activated");
            ChangeState(ElementState.WAIT);
        }
        //MOVE Mode: maximum player
        else if (playerActive == max_PlayerNr)
        {
            ChangeState(ElementState.MOVE);
            interactionManager.AddMoveInteraction(this, playerArray);
            Debug.Log("MOVE State activated");
        }
        //ACTIVE Mode: 0 player
        else if (playerActive == 0)
        {
            ChangeState(ElementState.ACTIVE);
        }
    }
    #endregion


    #region LightHandler

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

        // if light isn't already in the list, add it
        if (newInput)
        {
            inputList.AddLast(light);
            //Debug.Log("Added " + light.GetID() + " to " + this);
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
    }


    // CHECKS for right or wrong number of lights entering the objects
    protected virtual void UpdateOutput()
    {

        // checks wheather 
        if (CorrectInput())
        {
            CalculateOutput();
            if (this.error != ErrorState.NONE)
            {
                ChangeError(ErrorState.NONE);
            }
            
        }
        else if (inputList.Count == 0)
        {
            DisableOutput();
            if (this.error != ErrorState.NONE)
            {
                ChangeError(ErrorState.NONE);
            }
        }
        else
        {
            DisableOutput();
            if (this.error != ErrorState.ERRORINPUT)
            {
                ChangeError(ErrorState.ERRORINPUT);
            }

        }
  
    }


    // CHECKS amount an color of input beams
    protected abstract bool CorrectInput();


    // CALCULATES output beam(s)
    protected abstract void CalculateOutput();


    // DISABLES all output rays
    protected virtual void DisableOutput()
    {

        if (outputList != null)
        {
            var node = outputList.First;
            while (node != null)
            {
                node.Value.Disable();
                node = node.Next;
            }
        }
    }

    #endregion


    public virtual ErrorState GetError()
    {
        return error;
    }

    public virtual ElementState GetState()
    {
        return state;
    }
}