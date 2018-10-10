using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateInteraction : GroupInteraction {
    public AbstractOpticalElement mirrorPrefab;
    public AbstractOpticalElement prismPrefab;
    public AbstractOpticalElement splitterPrefab;
    public AbstractOpticalElement lensPrefab;

    public PreviewObject previewPrefab;
    private Animator anim;

    private PreviewObject preview;
    private int previousPlayerLength;

    public override void Awake ()
    {
        base.Awake();
        preview = Instantiate(previewPrefab, new Vector3(), Quaternion.AngleAxis(90, Vector3.right));
        anim = preview.GetComponent<Animator>();
        Debug.Log("Preview " + preview + ", Animator " + anim);
        Debug.Log("clip " + anim.GetNextAnimatorClipInfo(0));
        previousPlayerLength = 0;
        preview.SetElement(PreviewObject.Element.MIRROR);
    }

    public override void Update () {
        foreach (Player p in players)
        {
            if (p == null)
            {
                RemovePlayer(p);
            }
        }
        if (players.Length <= 1)
        {
            interactionManager.RemoveCreateInteraction(this);
        }

        base.Update();

        Vector3 position = new Vector3(midpoint.x, floorHeight, midpoint.y);
        Quaternion rotation = CalculateElementRotation(playersPoints, midpoint);

        //detect player amount change
        if (previousPlayerLength != players.Length)
        {
            Debug.Log("Playerchange: " + previousPlayerLength + " -> " + players.Length);
            PlayerChange();
        }

        //connection to midpoint here, vertices at object group
        foreach (Player p in players)
        {
            p.SetConnectionPoint(new Vector3(midpoint.x, floorHeight, midpoint.y));
        }

        preview.transform.SetPositionAndRotation(position, rotation * Quaternion.AngleAxis(90, Vector3.right));

        if (groupIsMoving)
        {
            timeLastMoved = Time.time;
            anim.StopPlayback();
        }
        //ELSE no member of group is moving
        else
        {
            //TODO: start create animation that always is interupted as soon as anyone starts moving again
            anim.Play("Wait");

            if ((Time.time - timeLastMoved) > secondsToPlaceObject && previousPlayerLength == players.Length)
            {
                anim.Play("CreateOpticalElement");

                if ((Time.time - timeLastMoved) > secondsToPlaceObject + 0.2)// + anim["CreateOpticalElement"].length - 0.1)
                {
                    //timeLastMoved = Time.time; //reset for the editing mode
                
                    AbstractOpticalElement opticalElement;
                    switch (preview.GetElement())
                    {
                        case PreviewObject.Element.MIRROR:
                            opticalElement = Instantiate(mirrorPrefab, position, rotation);
                            break;
                        case PreviewObject.Element.PRISM:
                            opticalElement = Instantiate(prismPrefab, position, rotation);
                            break;
                        case PreviewObject.Element.LENS:
                            opticalElement = Instantiate(lensPrefab, position, rotation);
                            break;
                        case PreviewObject.Element.SPLITTER:
                            opticalElement = Instantiate(splitterPrefab, position, rotation);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("Defect CreateInteraction");
                    }

                    opticalElement.ChangeState(AbstractOpticalElement.ElementState.MOVE);
                    ResetAllPlayersFriendCounters();
                    interactionManager.AddMoveInteraction(opticalElement, players);
                    interactionManager.RemoveCreateInteraction(this);
                }
            }
        }
        previousPlayerLength = players.Length;
    }

    private void PlayerChange()
    {
        if (players.Length == 2)
        {
            preview.SetElement(PreviewObject.Element.MIRROR);
        }
        else if (players.Length == 3)
        {
            preview.SetElement(PreviewObject.Element.PRISM);
        }
        else if (players.Length == 4)
        {
            preview.SetElement(PreviewObject.Element.SPLITTER);
        }
    }

    public override void RemovePlayer(Player player)
    {
        if (player != null)
        {
            player.isAvailable = true;
            player.ResetFriendCounter();
        }
        var tempPlayers = new List<Player>(players);
        tempPlayers.Remove(player);
        players = tempPlayers.ToArray();

        foreach (Player p in players)
        {
            p.DecreaseFriendCounter();
        }
    }

    public override void SetProperties(Player[] newPlayers, AbstractOpticalElement opticalElement)
    {
        //TODO?
    }

    public override PreviewObject GetPreview()
    {
        return preview;
    }
}
