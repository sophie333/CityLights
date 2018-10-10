using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GripPoints : MonoBehaviour
{
    private AbstractOpticalElement aoe;

    /* //if we had dedicated grippoint-to-player connections:
    //private GripPoints[] gripPoints;
    private bool isActive;
    private Player inputPlayer;**/

    public void Awake()
    {
        //gripPoints = transform.parent.GetComponentsInChildren<GripPoints>();
        aoe = transform.GetComponentInParent<AbstractOpticalElement>();
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (aoe.GetState() == AbstractOpticalElement.ElementState.ACTIVE || aoe.GetState() == AbstractOpticalElement.ElementState.WAIT || aoe.GetState() == AbstractOpticalElement.ElementState.DESTROY)
        {
            Player player = other.gameObject.GetComponent<Player>();
            //Player tries to enter GripPoint of OpticalElement
            if (player)
            {
                if (other == player.GetInnerCollider())
                {
                    //Check if player isn't already connected to a grippoint
                    if (player.isAvailable && !player.isCreating)
                    {
                        //Check if adding player has been successfull
                        if(aoe.AddPlayer(player))
                        {
                            player.isAvailable = false;

                            //Set connection to connected object
                            Vector3 middlePoint = new Vector3(aoe.transform.position.x, aoe.transform.position.y, aoe.transform.position.z);
                            //Debug.Log("middlepoint: " + middlePoint);
                            player.SetConnectionPoint(middlePoint);
                        }
                        else
                        {
                            throw new System.Exception("Player could not be added.");
                        }
                    }
                }
            }
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        //Player tries to exit GripPoint of OpticalElement
        if (player)
        {
            if (other == player.GetOuterCollider() && !player.isAvailable)
            {
                //Check if removing player has been successfull
                if (transform.GetComponentInParent<AbstractOpticalElement>().RemovePlayer(player.GetID()))
                {
                    GetComponentInParent<AbstractOpticalElement>().ClearInteractions();
                    aoe.CheckPlayerNr(); //should become either WAIT or ACTIVE
                }
                else
                {
                    throw new System.Exception("Player " + player + " could not be removed from GripPoint.");
                }
            }
        }
    }
}