using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBeam : MonoBehaviour
{
    public string colorName;

    private int ID;
    private static int idCounter = 0;

    private LayerMask layerMask; // layer with objects that should be ignored by the raycast, e.g. players
    private LineRenderer lineRenderer;
    private RaycastHit hit;
    private RaycastHit lastHit;
    private bool active;

    private RaycastHit[] hitPreviewObjects;
    private RaycastHit[] lastHitPreviewObjects;

    private RaycastHit[] hitPlayers;
    private RaycastHit[] lastHitPlayers;
    private float distance;

    private Vector3 height = new Vector3(0, 0.2f, 0);

    // properties that have to be set on instantiation:
    private Color color = Color.black;
    private Vector3 direction;
    private float raydius = 0.5f;

    public void Start()
    {
        ID = idCounter;
        idCounter++;

        active = true;

        layerMask = 1 << 2; // 2 is the integer value of ignore raycast layer
        layerMask |= 1 << 9; // 9 is the integer value of player layer
        layerMask |= 1 << 10; // 10 is the integer value of preview objects layer
        layerMask = ~layerMask;

        direction = transform.forward;

        if (color == Color.black)
        {
            switch (colorName)
            {
                case "red":
                    color = AbstractOpticalElement.red;
                    break;
                case "yellow":
                    color = AbstractOpticalElement.yellow;
                    break;
                case "green":
                    color = AbstractOpticalElement.green;
                    break;
                case "cyan":
                    color = AbstractOpticalElement.cyan;
                    break;
                case "blue":
                    color = AbstractOpticalElement.blue;
                    break;
                case "magenta":
                    color = AbstractOpticalElement.magenta;
                    break;
            }
        }
        else
        {
            if (color == AbstractOpticalElement.red)
            {
                colorName = "red";
            }
            else if (color == AbstractOpticalElement.yellow)
            {
                colorName = "yellow";
            }
            else if (color == AbstractOpticalElement.green)
            {
                colorName = "green";
            }
            else if (color == AbstractOpticalElement.cyan)
            {
                colorName = "cyan";
            }
            else if (color == AbstractOpticalElement.blue)
            {
                colorName = "blue";
            }
            else if (color == AbstractOpticalElement.magenta)
            {
                colorName = "magenta";
            }
        }

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

    public void Update()
    {
        if (active)
        {
            // if active, set the line renderer to the light beam's current color
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;

            // if active, cast a ray in given direction
            Ray charles = new Ray(transform.position + height + direction * raydius, direction);
            Debug.DrawRay(transform.position + height + direction * raydius, direction, Color.red);

            ////// 1) LIGHTBEAM RAY //////

            if (Physics.Raycast(charles, out hit, Mathf.Infinity, layerMask))
            {

                // Debug.Log(ID + " (" + colorName + ") did hit: " + hit.collider.tag + " (" + hit.collider.transform.parent + ")");

                // if the element that the ray hits changes, remove this lightBeam from the previously hit element's input list
                if (lastHit.collider != null && lastHit.collider != hit.collider)
                {
                    if (lastHit.collider.tag == "lightInput")
                    {
                        lastHit.collider.transform.parent.GetComponent<AbstractOpticalElement>().RemoveInputLight(this);
                    }
                    else if (lastHit.collider.tag == "lampPort")
                    {
                        lastHit.collider.GetComponent<Network_LampPort>().RemoveInputLight(this);
                    }
                        
                }

                // if the ray hits an optical element, add the beam to the element'a input list
                if (hit.collider.tag == "lightInput")
                {
                    hit.collider.transform.parent.GetComponent<AbstractOpticalElement>().AddInputLight(this);
                    lastHit = hit;
                }
                // if the ray hits a lamp port, add the beam as the element's input
                else if (hit.collider.tag == "lampPort")
                {
                    hit.collider.GetComponent<Network_LampPort>().AddInputLight(this);
                    lastHit = hit;
                }

                // render light beam
                lineRenderer.SetPosition(0, transform.position + height);
                lineRenderer.SetPosition(1, hit.point);
                if (hit.collider.tag == "lightInput" && hit.collider.transform.parent.GetComponent<AbstractOpticalElement>().GetType().ToString() == "Mirror")
                {
                    lineRenderer.SetPosition(2, hit.point);
                }
                else if (hit.collider.tag == "lightInput")
                {
                    lineRenderer.SetPosition(2, hit.collider.transform.parent.transform.position + height);
                }
                else
                {
                    lineRenderer.SetPosition(2, hit.collider.transform.position + height);
                }

                distance = hit.distance;
            }
            else
            {
                // if nothing's hit, render the light beam with length 20 ( longer then screen's diagonal => seems infinite)
                lineRenderer.SetPosition(0, transform.position + height);
                lineRenderer.SetPosition(1, transform.position + direction * 20 + height);
                lineRenderer.SetPosition(2, transform.position + direction * 20 + height);

                // Debug.Log(ID + " (" + colorName + ") did not hit.");

                // remove beam from the previously hit element's input list
                if (lastHit.collider != null)
                {
                    // previously hit element is a lamp port
                    if (lastHit.collider.transform.tag == "lampPort")
                    {
                        lastHit.collider.transform.GetComponent<Network_LampPort>().RemoveInputLight(this);
                    }

                    // previously hit element is an abstract optical element
                    else
                    {
                        lastHit.collider.transform.parent.GetComponent<AbstractOpticalElement>().RemoveInputLight(this);
                    }

                    lastHit = hit;
                }

                distance = 20;
            }



            ////// 2) RAY FOR PREVIEW ELEMENT DETECTION //////

            hitPreviewObjects = Physics.RaycastAll(charles, distance, 1 << 10);

            if (lastHitPreviewObjects != null)
            {
                for (int i = 0; i < lastHitPreviewObjects.Length; i++)
                {
                    if (lastHitPreviewObjects[i].collider != null)
                    {
                        lastHitPreviewObjects[i].collider.GetComponent<PreviewObject>().hitByRay = false;
                    }       
                }
            }
            

            for (int i = 0; i < hitPreviewObjects.Length; i++)
            {
                hitPreviewObjects[i].collider.GetComponent<PreviewObject>().hitByRay = true;
            }

            lastHitPreviewObjects = hitPreviewObjects;




            ////// 3) RAY FOR PLAYER DETECTION / PARTICLE EFFECT //////

            hitPlayers = Physics.RaycastAll(charles, distance, 1 << 9);

            if (lastHitPlayers != null)
            {
                for (int i = 0; i < lastHitPlayers.Length; i++)
                {
                    if (lastHitPlayers[i].collider != null)
                    {
                        lastHitPlayers[i].collider.transform.parent.GetComponent<Player>().DisableParticles();
                    }
                }
            }


            for (int i = 0; i < hitPlayers.Length; i++)
            {
                hitPlayers[i].collider.transform.parent.GetComponent<Player>().EnableParticles(color);
            }

            lastHitPlayers = hitPlayers;




        }
        // if not active, ...
        else
        {
            // ... draw line with length 0 to make beam invisible
            // Debug.Log(ID + " (" + colorName + ") is deactivated");
            lineRenderer.SetPosition(0, transform.position + height);
            lineRenderer.SetPosition(1, transform.position + height);
            lineRenderer.SetPosition(2, transform.position + height);

            // ... remove beam from the previously hit element's input list
            if (lastHit.collider != null)
            {
                // previously hit element is a lamp port
                if (lastHit.collider.transform.tag == "lampPort")
                {
                    lastHit.collider.transform.GetComponent<Network_LampPort>().RemoveInputLight(this);
                }

                // previously hit element is an abstract optical element
                else
                {
                    lastHit.collider.transform.parent.GetComponent<AbstractOpticalElement>().RemoveInputLight(this);
                }

                lastHit = hit;
            }
        }

    }

    private void OnDestroy()
    {
        // remove from optical elements
        if (lastHit.collider != null)
        {
            // previously hit element is a lamp port
            if (lastHit.collider.transform.tag == "lampPort")
            {
                lastHit.collider.transform.GetComponent<Network_LampPort>().RemoveInputLight(this);
            }

            // previously hit element is an abstract optical element
            else
            {
                lastHit.collider.transform.parent.GetComponent<AbstractOpticalElement>().RemoveInputLight(this);
            }
        }

        // remove from preview objects
        if (lastHitPreviewObjects != null)
        {
            for (int i = 0; i < lastHitPreviewObjects.Length; i++)
            {
                if (lastHitPreviewObjects[i].collider != null)
                {
                    lastHitPreviewObjects[i].collider.GetComponent<PreviewObject>().hitByRay = false;
                }
            }
        }

        // remove from player
        if (lastHitPlayers != null)
        {
            for (int i = 0; i < lastHitPlayers.Length; i++)
            {
                if (lastHitPlayers[i].collider != null)
                {
                    lastHitPlayers[i].collider.transform.parent.GetComponent<Player>().DisableParticles();
                }
            }
        }
    }

    public int GetID()
    {
        return ID;
    }

    public RaycastHit GetRaycastHit()
    {
        return hit;
    }

    public Color GetColor()
    {
        return color;
    }

    public void SetColor(Color color)
    {
        this.color = color;
    }

    public Vector3 GetDirection()
    {
        return direction;
    }

    public void SetDirection(Vector3 direction)
    {
        this.direction = direction;
    }

    public void Enable()
    {
        active = true;
    }

    public void Disable()
    {
        active = false;
    }
}
