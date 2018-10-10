using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    private Player destroyingPlayer;
    AbstractOpticalElement aoe;
    private float timeEntered;

    public static float DestroyTime; //Settings.txt

    private void Update()
    {
        if (aoe.GetState() == AbstractOpticalElement.ElementState.DESTROY)
        {
            aoe.waitCircle.fillAmount = (Time.time - timeEntered) / DestroyTime;
        }
    }

    void Awake()
    {
        aoe = transform.GetComponentInParent<AbstractOpticalElement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.gameObject.GetComponent<Player>();

        if (player && aoe.PlayerIsConnected(player))
        {
            if (other == player.GetInnerCollider())
            {
                if (aoe.GetState() == AbstractOpticalElement.ElementState.ACTIVE || aoe.GetState() == AbstractOpticalElement.ElementState.WAIT)
                {
                    if (aoe.GetPlayersActive() == 1)
                    {
                        destroyingPlayer = player;
                        timeEntered = Time.time;
                        DestroyCoroutine = DestroyElement();
                        StartCoroutine(DestroyCoroutine);
                        aoe.ChangeState(AbstractOpticalElement.ElementState.DESTROY);
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (destroyingPlayer != null)
        {
            if (destroyingPlayer == other.gameObject.GetComponent<Player>())
            {
                Debug.Log("OnTriggerExit " + destroyingPlayer);
                Debug.Log("OnTriggerExit " + destroyingPlayer.GetInnerCollider());
                if (other == destroyingPlayer.GetInnerCollider())
                {
                    StopCoroutine(DestroyCoroutine);
                    aoe.CheckPlayerNr();
                    aoe.waitCircle.fillAmount = 0;
                    destroyingPlayer = null;
                }
            }
        }
    }

    private IEnumerator DestroyCoroutine; //for restarting the coroutine instead of resuming it with StopCoroutine
    private IEnumerator DestroyElement()
    {
        yield return new WaitForSeconds(DestroyTime);
        if (aoe.GetState() == AbstractOpticalElement.ElementState.DESTROY)
        {
            aoe.ClearInteractions();
            Destroy(aoe.gameObject);
        }
    }
}
