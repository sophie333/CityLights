using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationInteraction : MonoBehaviour {
    public Player player;
    public float floorHeight = 0f; //is used for Vector 3 after determining the objects
    private float timeLastMoved;
    public static float secondsToPlaceObject;
    private AbstractOpticalElement opticalElement;
    private bool placedObject = false;
    public InteractionManager interactionManager;

    public bool GetPlacedObject() { return placedObject; }

    public void Awake()
    {
        interactionManager = FindObjectOfType<InteractionManager>();
        timeLastMoved = Time.time;
    }

    public void SetProperties(Player player, AbstractOpticalElement element)
    {
        this.player = player;
        player.isAvailable = false;
        opticalElement = element;
    }

    void Update()
    {
        if (player == null) {
            PlaceObject();
        }
        else
        {
            if (player.IsMoving())
            {
                timeLastMoved = Time.time;

                opticalElement.transform.SetPositionAndRotation(opticalElement.transform.position, CalculateRotation());
                //TODO calculate and test new Vertices
            }
            else
            {
                //start place animation
                if ((Time.time - timeLastMoved) > secondsToPlaceObject)
                {
                    PlaceObject();
                }
            }
        }
    }

    private Quaternion CalculateRotation()
    {
        Vector3 pos = player.transform.position;
        Vector3 elementMid = opticalElement.transform.position;

        Vector3 lookTo = pos - elementMid;
        lookTo.y = 0;

        //TODO check AngleAxis in tests
        Quaternion result = Quaternion.LookRotation(lookTo, Vector3.up);
        return result;
    }


    private void PlaceObject()
    {
        bool overlap = opticalElement.GetError() == AbstractOpticalElement.ErrorState.ERROROVERLAP;
        Debug.Log("PLACE OBJECT with ErrorOverlap " + overlap);

        if (!overlap)
        {
            opticalElement.ChangeState(AbstractOpticalElement.ElementState.ACTIVE);
            placedObject = true;
        }
        else
        {
            DestroyImmediate(opticalElement.gameObject);
            placedObject = false;
        }
        interactionManager.RemoveRotationInteraction(this);
    }
}
