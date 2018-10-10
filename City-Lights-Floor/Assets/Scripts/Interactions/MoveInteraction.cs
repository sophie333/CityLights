using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveInteraction : GroupInteraction {
    private bool placedObject = false;
    private AbstractOpticalElement opticalElement;

    public bool GetPlacedObject() { return placedObject; }

    public override void Awake()
    {
        base.Awake();
    }

    public void OnDestroy()
    {
        if (placedObject)
        {
            foreach (Player p in players)
            {
                if (p != null)
                {
                    StartCoroutine(p.DelayIsAvailableTrue());
                }
                else
                {
                    Debug.LogWarning("Some Player on GroupInteraction Destroy was NULL");
                    //TODO handle null-player interactions
                }
            }
        }
        else
        {
            SetAllPlayersAvailable(true);
            ResetAllPlayersFriendCounters();
        }
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
        this.SetAllPlayersAvailable(false);
        this.opticalElement = opticalElement;
    }

    public override void Update () {
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
            PlaceObject();
        }
        else
        {
            base.Update();

            //connection to midpoint here, vertices at object group
            foreach (Player p in players)
            {
                p.SetConnectionPoint(new Vector3(midpoint.x, floorHeight, midpoint.y));
            }

            if (groupIsMoving)
            {
                // reset place animation
                opticalElement.waitCircle.fillAmount = 0;

                timeLastMoved = Time.time;

                //calculate rotation and position of object
                Quaternion rot = CalculateElementRotation(playersPoints, midpoint);
                Vector3 pos = new Vector3(midpoint.x, floorHeight, midpoint.y);
                opticalElement.transform.SetPositionAndRotation(pos, rot);

                //TODO testing Vertices
                /*Vector3[] elementVertices = CalculateElementVertices(rot, midpoint);
                for (int i = 0; i < players.Length; i++)
                {
                    players[i].SetConnectionPoint(elementVertices[i]);
                }
                */
            }
            //ELSE no member of group is moving
            else
            {
                // place animation
                opticalElement.waitCircle.fillAmount = (Time.time - timeLastMoved) / secondsToPlaceObject;

                // place object
                if ((Time.time - timeLastMoved) > secondsToPlaceObject)
                {
                    // reset place animation
                    opticalElement.waitCircle.fillAmount = 0;

                    PlaceObject();
                }
            }
        }
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
        interactionManager.RemoveMoveInteraction(this);
    }
}
