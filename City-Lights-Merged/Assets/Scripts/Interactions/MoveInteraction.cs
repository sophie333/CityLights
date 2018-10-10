using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveInteraction : GroupInteraction {
    private bool objectPlaced = false;
    public AbstractOpticalElement opticalElement;
    private bool active;

    public bool GetObjectPlaced() { return objectPlaced; }

    public override void Awake()
    {
        base.Awake();
        active = true;
    }

    public void OnDestroy()
    {
        SetAllPlayersAvailable(true);
        ResetAllPlayersFriendCounters();
    }

    public IEnumerator DelayedDestroy()
    {
        active = false;
        float delay = 3f;
        
        foreach (Player p in players)
        {
            if (p != null)
            {
                StartCoroutine(p.DelayIsAvailableTrue());
                delay = p.GetDelayAvailable();
            }
            else
            {
                Debug.LogWarning("Some Player on GroupInteraction Destroy was NULL");
                //TODO handle null-player interactions
            }
        }

        yield return new WaitForSeconds(delay);
        Destroy(this.gameObject);
    }

    public override void RemovePlayer(Player player)
    {
        //player left the group in move state - delete the whole group
        interactionManager.RemoveMoveInteraction(this);

        //TODO: handle the object
        //Sophie: is object automatically in wait mode?
    }

    public override void SetProperties(Player[] newPlayers, AbstractOpticalElement opticalElement)
    {
        players = newPlayers;
        SetAllPlayersAvailable(false);
        this.opticalElement = opticalElement;
        foreach (Player p in players)
        {
            p.SetConnectionPoint(new Vector3(midpoint.x, floorHeight, midpoint.y));
        }
    }

    public override void Update () {
        if (active)
        {
            bool intact = true;
            foreach (Player p in players)
            {
                if (p == null)
                {
                    intact = false;
                }
            }

            if (!intact)
            {
                interactionManager.RemoveMoveInteraction(this);
            }
            else
            {
                base.Update();

                //calculate rotation and position of object
                Quaternion rot = CalculateElementRotation(playersPoints, midpoint);
                Vector3 pos = new Vector3(midpoint.x, floorHeight, midpoint.y);
                opticalElement.transform.SetPositionAndRotation(pos, rot);

                if (groupIsMoving)
                {
                    // reset place animation
                    opticalElement.waitCircle.fillAmount = 0;

                    timeLastMoved = Time.time;


                    //TODO testing Vertices
                    /*Vector3[] elementVertices = CalculateElementVertices(rot, midpoint);
                    for (int i = 0; i < players.Length; i++)
                    {
                        players[i].SetConnectionPoint(elementVertices[i]);
                    }
                    */
                }
                //ELSE no member of group is active
                else
                {
                    // place animation
                    opticalElement.waitCircle.fillAmount = (Time.time - timeLastMoved) / secondsToPlaceObject;

                    // place object
                    if ((Time.time - timeLastMoved) > secondsToPlaceObject)
                    {
                        interactionManager.RemoveMoveInteraction(this);
                    }
                }
            }
        }
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
            DestroyImmediate(opticalElement.gameObject);
            objectPlaced = false;
        }

        return objectPlaced;
    }
}
