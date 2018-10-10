using System.Collections;
using UnityEngine;
using UnityEngine.Networking;



public class Player : ATrackingEntity
{
    public Vector3[] positions;
    //public float currentDistance;
    //public float oldDistance;

    public static float PlayerObjectThreshold;
    public static float PlayerPlayerThreshold;
    public static float moveThreshold; //TODO: check and adjust threshold

    public Collider innerCollider;
    public Collider outerCollider;

    public InteractionManager GIManager;

    public bool canCollideWithPlayer = true;
    public bool isAvailable = true;
    public float delayAvailable = 3f;

    public LineRenderer line;
    private bool delayCooldown = false;

    private Vector3 oldPosition;
    private Vector3 deltaPosition; //for calculating movement

    private Vector3 connectionPoint;

    private static int ID;
    private int idCounter = 0;


    public int GetID()
    {
        return ID;
    }

    public void SetConnectionPoint(Vector3 point)
    {
        connectionPoint = point;
    }

    #region friendCounterFunctions

    private int friendCounter = 0;

    public int GetFriendCounter()
    {
        return friendCounter;
    }

    public void IncreaseFriendCounter()
    {
        friendCounter++;
    }

    public void DecreaseFriendCounter()
    {
        friendCounter--;
    }

    public void ResetFriendCounter()
    {
        friendCounter = 0;
    }

    #endregion

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        oldPosition = transform.position;
        ID = idCounter;
        idCounter++;
        GIManager = FindObjectOfType<InteractionManager>();
        isAvailable = true;

        SphereCollider sInnerCollider = (SphereCollider)innerCollider;
        sInnerCollider.radius = PlayerObjectThreshold;

        SphereCollider sOuterCollider = (SphereCollider)outerCollider;
        sOuterCollider.radius = PlayerPlayerThreshold;

    }

    void OnDestroy()
    {
        //TODO handle NULL at player disappearance in Create,Move,Rotate Interactions
    }

    void Update()
    {
        deltaPosition = transform.position - oldPosition;
        oldPosition = transform.position;

        if (!isAvailable && !delayCooldown)
        {
            //draw line from this.transform.position to connectionPoint
            line.SetPosition(0, this.transform.position);
            line.SetPosition(1, connectionPoint);
        } else
        {
            //keep the Line Render active
            line.SetPosition(0, this.transform.position);
            line.SetPosition(1, this.transform.position);
        }
    }
    
    public override void SetPosition(Vector2 theNewPosition)
    {
        transform.position = new Vector3(theNewPosition.x, .5f, theNewPosition.y);
    }

    public Collider GetInnerCollider()
    {
        return innerCollider;
    }

    public Collider GetOuterCollider()
    {
        return outerCollider;
    }

    void OnTriggerEnter(Collider other)
    {
        //check if other colllider is from a player
        Player player = other.gameObject.GetComponent<Player>();
        if (player)
        {
            if (player.GetInnerCollider() != this.GetInnerCollider())
            {
                //Debug.Log("Player " +  this + " and " + player + " collided.");
            
                if (player.isAvailable)
                {
                    //if THIS is the first to be handled, execute the group interaction
                    if (player.canCollideWithPlayer)
                    {
                        //check again which collider of the player has collided
                        if (player.GetOuterCollider() == other)
                        {
                            canCollideWithPlayer = false;
                            GIManager.GroupInteractionOnPlayerCollisionEnter(this, player);
                        }
                    }
                    //if the other player has already executed the group interaction, do nothing, but turn on his collisions again
                    else
                    {
                        player.canCollideWithPlayer = true;
                    }
                }
            }
        }
    }

    //TODO
    /*
    public void OnTriggerExit(Collider other)
    {
        //check if other colllider is from a player
        Player player = other.gameObject.GetComponent<Player>();
        if (player && player.isAvailable)
        {
            //if THIS is the first to be handled, execute the group interaction
            if (player.canCollideWithPlayer)
            {
                //check again which collider of the player has collided
                if (player.GetOuterCollider() == other)
                {
                    canCollideWithPlayer = false;
                    GIManager.GroupInteractionOnPlayerCollisionExit(this, player);
                }
            }
            //if the other player has already executed the group interaction, do nothing, but turn on his collisions again
            else
            {
                player.canCollideWithPlayer = true;
            }
        }
    }
    */

    public bool IsMoving()
    {
        //calcualtion makes no sense. just for the sake of clarity
        float deltaPositionLength = deltaPosition.sqrMagnitude * 10000;
        //Debug.Log("Delta Position Length from Player " + this + " Squared * 10000: " + deltaPositionLength);
       
        if (deltaPositionLength > moveThreshold)
        {
            return true;
        }
        return false;
    }

    public IEnumerator DelayIsAvailableTrue()
    {
        delayCooldown = true;
        yield return new WaitForSeconds(delayAvailable);
        delayCooldown = false;
        isAvailable = true;
        ResetFriendCounter();
    }
}