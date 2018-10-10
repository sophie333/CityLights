using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationInteraction : MonoBehaviour {
    public Player player;
    public float floorHeight = 0f; //is used for Vector 3 after determining the objects
    private float timeLastMoved;
    public static float secondsToPlaceObject;
    public AbstractOpticalElement opticalElement;
    private bool objectPlaced = false;
    public InteractionManager interactionManager;
    private bool active;

    public bool GetObjectPlaced() { return objectPlaced; }

    public void Awake()
    {
        interactionManager = FindObjectOfType<InteractionManager>();
        timeLastMoved = Time.time;
        active = true;
    }

    public void SetProperties(Player player, AbstractOpticalElement element)
    {
        if (player == null) //error on creation
        {
            interactionManager.RemoveRotationInteraction(this);
        }
        else
        {
            this.player = player;
            player.isAvailable = false;
            opticalElement = element;
        }
    }

    void Update()
    {
        if (active) { 
            if (player == null) {
                interactionManager.RemoveRotationInteraction(this);
            }
            else
            {
                opticalElement.transform.SetPositionAndRotation(opticalElement.transform.position, CalculateRotation());

                if (player.IsMoving())
                {
                    // reset place animation
                    opticalElement.waitCircle.fillAmount = 0;

                    timeLastMoved = Time.time;

                    // TODO calculate and test new Vertices
                }
                else
                {
                    // place animation
                    opticalElement.waitCircle.fillAmount = (Time.time - timeLastMoved) / secondsToPlaceObject;

                    // place object
                    if ((Time.time - timeLastMoved) > secondsToPlaceObject)
                    {
                        interactionManager.RemoveRotationInteraction(this);
                    }
                }
            }
        }
    }

    public void OnDestroy()
    {
        if (player != null)
        {
            player.isAvailable = true;
            player.ResetFriendCounter();
        }
        else
        {
            Debug.LogWarning("Player in rotInt was NULL");
            //TODO handle null-player interactions
        }
    }

    public IEnumerator DelayedDestroy()
    {
        active = false;
        float delay = 3f;

        if (player != null)
        {
            StartCoroutine(player.DelayIsAvailableTrue());
            delay = player.GetDelayAvailable();
        }
        else
        {
            Debug.LogWarning("Player on RotationInteraction Destroy was NULL");
            //TODO handle null-player interactions
        }

        yield return new WaitForSeconds(delay);
        Destroy(this.gameObject);
    }

    private Quaternion CalculateRotation()
    {
        if (active) { 
            Vector3 pos = player.transform.position;
            Vector3 elementMid = opticalElement.transform.position;

            Vector3 lookTo = pos - elementMid;
            lookTo.y = 0;

            //TODO check AngleAxis in tests
            Quaternion result = Quaternion.LookRotation(lookTo, Vector3.up);
            return result;
        }
        return opticalElement.transform.rotation;
    }


    public bool PlaceObject()
    {
        bool overlap = opticalElement.GetError() == AbstractOpticalElement.ErrorState.ERROROVERLAP;
        Debug.Log("PLACE OBJECT with ErrorOverlap " + overlap);

        if (!overlap)
        {
            opticalElement.ChangeState(AbstractOpticalElement.ElementState.ACTIVE);
            objectPlaced = true;
        }
        else
        {
            Destroy(opticalElement.gameObject);
            objectPlaced = false;
        }
        return objectPlaced;
    }
}
