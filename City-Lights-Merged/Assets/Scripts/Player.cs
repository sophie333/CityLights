using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;



public class Player : ATrackingEntity
{
    public Vector3[] positions;

    public static float PlayerObjectThreshold; //Settings.txt
    public static float PlayerPlayerThreshold; //Settings.txt
    public static float MoveThreshold; //Settings.txt

    public Collider innerCollider;
    public Collider outerCollider;

    public InteractionManager GIManager;

    public bool canCollideWithPlayer = true;
    public bool isAvailable = true;
    public static float DelayAvailable; //Settings.txt
    public bool isCreating;

    public LineRenderer line;
    private bool delayCooldown = false;

    private Vector3 oldPosition;
    private Vector3 deltaPosition; //for calculating movement

    private Vector3 connectionPoint;

    private static int ID;
    private int idCounter = 0;

    private ParticleSystem particleSys;

    private bool paused = true;

    public int GetID()
    {
        return ID;
    }

    public float GetDelayAvailable()
    {
        return DelayAvailable;
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
        isCreating = false;

        SphereCollider sInnerCollider = (SphereCollider)innerCollider;
        sInnerCollider.radius = PlayerObjectThreshold;

        SphereCollider sOuterCollider = (SphereCollider)outerCollider;
        sOuterCollider.radius = PlayerPlayerThreshold;

        //TODO ParticleSystem testing
        particleSys = transform.Find("PS_Player").Find("Particles").GetComponent<ParticleSystem>();
        paused = FindObjectOfType<GameManagerFloor>().gamePaused;
    }

    public void EnableParticles(Color col)
    {
        Debug.Log("SET PARTICLES");
        ParticleSystem.MainModule pM = particleSys.main;
        pM.startColor = col;
        particleSys.Play();
    }

    public void DisableParticles()
    {
        particleSys.Stop();
    }

    void Update()
    {
        /*Game Pause Handling**/
        bool newPausedState = FindObjectOfType<GameManagerFloor>().gamePaused;
        Debug.Log("Paused state: " + newPausedState);

        if (newPausedState != paused)
        {
            Debug.Log("Paused state changed from " + paused + " to " + newPausedState);
            paused = newPausedState;
            foreach (Collider c in GetComponentsInChildren<Collider>())
            {
                c.enabled = !paused; //if game is paused true -> disable collider via enabled = !true
            }
        }

        /*Player Movement Tracking**/
        deltaPosition = transform.position - oldPosition;
        oldPosition = transform.position;

        if ((!isAvailable && !delayCooldown) || isCreating)
        {
            //draw line from this.transform.position to connectionPoint
            line.SetPosition(0, this.transform.position);
            line.SetPosition(1, connectionPoint);
        }
        else
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
            if (player.GetInnerCollider() != GetInnerCollider())
            {
                if (player.isAvailable && isAvailable)
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

    public void OnTriggerExit(Collider other)
    {
        //check if other colllider is from a player
        Player player = other.gameObject.GetComponent<Player>();
        if (player && player.isAvailable && this.isAvailable)
        {
            //if THIS is the first to be handled, execute the group interaction
            if (player.canCollideWithPlayer)
            {
                //check which collider of the player has collided
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

    public bool IsMoving()
    {
        //calcualtion makes no sense. just for the sake of clarity
        float deltaPositionLength = deltaPosition.sqrMagnitude * 10000;
        //Debug.Log("Delta Position Length from Player " + this + " Squared * 10000: " + deltaPositionLength);

        if (deltaPositionLength > MoveThreshold)
        {
            return true;
        }
        return false;
    }

    public IEnumerator DelayIsAvailableTrue()
    {
        delayCooldown = true;
        yield return new WaitForSeconds(DelayAvailable);
        delayCooldown = false;
        isAvailable = true;
        ResetFriendCounter();
    }
}