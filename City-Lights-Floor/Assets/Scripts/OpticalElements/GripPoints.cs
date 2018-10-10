using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GripPoints : MonoBehaviour
{
    private Player inputPlayer;
    private bool hasSamePlayer = false;
    private bool isActive;
    private GripPoints[] gripPoints;
    private Transform objectPinned;

    public void Awake()
    {
        gripPoints = transform.parent.GetComponentsInChildren<GripPoints>();
        objectPinned = transform.parent.GetComponent<AbstractOpticalElement>().transform;
        isActive = false;
    }

    protected void OnTriggerEnter(Collider other)
    {
        Collider playerCollider = other.gameObject.GetComponent<Player>().GetInnerCollider();

        //Check if GripPoint is already active
        if (!isActive)
        {
            //Player tries to enter GripPoint of OpticalElement
            if (playerCollider)
            {
                Player player = playerCollider.gameObject.GetComponent<Player>();
                //Check if player isn't already connected to a grippoint
                if (player.isAvailable)
                {
                    //Check if adding player has been successfull
                    if (transform.GetComponentInParent<AbstractOpticalElement>().AddPlayer(player))
                    {
                        isActive = true;
                        inputPlayer = player;
                        player.isAvailable = false;

                        //Set connection to connected object
                        Vector3 middlePoint = new Vector3(objectPinned.transform.position.x, objectPinned.transform.position.y, objectPinned.transform.position.z);
                        Debug.Log("middlepoint: " + middlePoint);
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

    protected void OnTriggerExit(Collider other)
    {
        Collider playerCollider = other.gameObject.GetComponent<Player>().GetInnerCollider();
        //Player tries to exit GripPoint of OpticalElement
        if (playerCollider)
        {
            Player player = playerCollider.gameObject.GetComponent<Player>();
            //Check if right player wants to exit
            if (player == inputPlayer)
            {
                //Check if removing player has been successfull
                if (transform.GetComponentInParent<AbstractOpticalElement>().RemovePlayer(inputPlayer.GetID()))
                {
                    inputPlayer = null;
                    isActive = false;
                    player.isAvailable = true;
                }
                else
                {
                    throw new System.Exception("Player could not be removed.");
                }
            }
        }
    }
}