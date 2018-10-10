using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour {
    private List<MoveInteraction> moveInteractionList = new List<MoveInteraction>();
    private List<CreateInteraction> createInteractionList = new List<CreateInteraction>();
    public CreateInteraction CIPrefab;
    public MoveInteraction MIPrefab;

    private List<RotationInteraction> rotationInteractionList = new List<RotationInteraction>();
    public RotationInteraction RIPrefab;

    #region GroupInteraction

    //searches if the Gameobject player is contained in any of the interacting groups. If so, it returns the group.
    public CreateInteraction FindPlayerInGroups(Player player)
    {
        foreach (CreateInteraction createInteraction in createInteractionList)
        {
            if (createInteraction.ContainsPlayer(player))
            {
                return createInteraction;
            }
        }
        //if player is not contained in any group
        return null;
    }

    /**Updates the current interacting groups as soon as any player sends a triggerEnter event with another player*/
    public void GroupInteractionOnPlayerCollisionEnter (Player player1, Player player2)
    {
        CreateInteraction groupInteraction1 = FindPlayerInGroups(player1);
        if (groupInteraction1 != null)
        {
            CreateInteraction groupInteraction2 = FindPlayerInGroups(player2);
            if (groupInteraction2 != null)
            {
                if (groupInteraction1 == groupInteraction2)
                {
                    //both players are in the same group
                    return;
                }

                
                if (groupInteraction1.players.Length == 2 && groupInteraction1.players.Length == 2)
                {
                    //merge 4 players from two groups of two
                    foreach (Player player in groupInteraction2.players)
                    {
                        groupInteraction1.AddPlayer(player);
                        RemoveCreateInteraction(groupInteraction2);
                    }                   
                }
                else
                {
                    //more than four players
                    //TODO: handle this or do nothing
                }
                
            }
            else
            {
                //player1 is part of a group. player2 has to be added.
                if (groupInteraction1.players.Length < 4)
                {
                    groupInteraction1.AddPlayer(player2);
                }
            }
        }
        else
        {
            CreateInteraction groupInteraction2 = FindPlayerInGroups(player2);
            if (groupInteraction2 != null)
            {
                //player2 is part of a group. player1 has to be added.
                if (groupInteraction2.players.Length < 4)
                {
                    groupInteraction2.AddPlayer(player1);
                }
            }
            else
            {
                //none is part of a group. new Group has to be added.
                CreateInteraction newCreateInteraction = Instantiate(CIPrefab);
                newCreateInteraction.AddPlayer(player1);
                newCreateInteraction.AddPlayer(player2);
                createInteractionList.Add(newCreateInteraction);
            }
        }
    }

    /**Updates the current interacting groups as soon as any player sends a triggerExit event with another player*/
    /*
    * Handles player trigger exiting in creating groups
    * Player trigger exit in moveinteraction takes no effect, since this should be handled via GripPoints Trigger exit
    **/
    //TODO check if above comment is coded correctly
    public void GroupInteractionOnPlayerCollisionExit(Player player1, Player player2)
    {
        CreateInteraction groupInteraction1 = FindPlayerInGroups(player1);
        if (groupInteraction1 != null)
        {
            CreateInteraction groupInteraction2 = FindPlayerInGroups(player2);
            if (groupInteraction2 != null)
            {
                //both players are in the same group
                if (groupInteraction1 == groupInteraction2)
                {
                    //if player1 is still connected to the group
                    if (player1.GetFriendCounter() > 1)
                    {
                        //both players are still connected to the group
                        if (player2.GetFriendCounter() > 1)
                        {
                            //do nothing because the group is the same as before
                        } 
                        //only player1 is connected, player2 is leaving
                        else
                        {
                            groupInteraction1.RemovePlayer(player2);
                        }
                    }
                    //only player2 is connected, player1 is leaving
                    else if (player2.GetFriendCounter() > 1)
                    {
                        groupInteraction2.RemovePlayer(player1);
                    }
                    //player1 and player2 are the two players left in the creating group
                    else
                    {
                        groupInteraction1.ResetAllPlayersFriendCounters();
                        RemoveCreateInteraction(groupInteraction1);
                    }
                }
                //players are in different groups (e.g. 2 and 3 players walk over each others triggers)
                else
                {
                    //TODO: handle this or do nothing
                }
            }
            //player1 is part of a "full" group. player2 is not in a group and leaves group from p1
            //else {//do nothing}
        }
        //player1 is not in a group and leaves from WHERE?? is this even possible?
        //else {//do nothing}
    }

    public MoveInteraction AddMoveInteraction(AbstractOpticalElement opticalElement, Player[] players)
    {
        Debug.Log("AddMoveInteraction");
        MoveInteraction newGroupInteraction = Instantiate(MIPrefab);
        newGroupInteraction.SetProperties(players, opticalElement);
        moveInteractionList.Add(newGroupInteraction);
        return newGroupInteraction;
    }

    public void RemoveMoveInteraction(MoveInteraction group)
    {
        // reset place animation
        group.opticalElement.waitCircle.fillAmount = 0;

        moveInteractionList.Remove(group);

        bool placed = group.PlaceObject();
        if (placed)
        {
            group.StartCoroutine(group.DelayedDestroy());
        }
        else
        {
            Destroy(group);
        }
    }

    public void RemoveCreateInteraction(CreateInteraction group)
    {
        group.SetAllPlayersCreating(false);
        group.ResetAllPlayersFriendCounters();
        Destroy(group.GetPreview().gameObject);
        createInteractionList.Remove(group);
        Destroy(group.gameObject);
    }

    #endregion

    public RotationInteraction AddRotationInteraction(AbstractOpticalElement opticalElement, Player player)
    {
        RotationInteraction newRotationInteraction = Instantiate(RIPrefab);
        newRotationInteraction.SetProperties(player, opticalElement);
        rotationInteractionList.Add(newRotationInteraction);
        return newRotationInteraction;
    }

    public void RemoveRotationInteraction(RotationInteraction rotInt)
    {
        // reset place animation
        rotInt.opticalElement.waitCircle.fillAmount = 0;

        rotationInteractionList.Remove(rotInt);

        bool placed = rotInt.PlaceObject();
        if (placed)
        {
            rotInt.StartCoroutine(rotInt.DelayedDestroy());
        }
        else
        {
            Destroy(rotInt);
        }
    }

}
