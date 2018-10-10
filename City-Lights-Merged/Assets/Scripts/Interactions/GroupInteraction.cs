using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public abstract class GroupInteraction : MonoBehaviour {
    public Player[] players;
    public float floorHeight = 0f; //is used for Vector 3 after determining the objects
    protected float timeLastMoved;
    public static float secondsToPlaceObject;
    public InteractionManager interactionManager;
    protected Vector2 midpoint; //ElementMidpoint
    protected bool groupIsMoving;
    protected Vector2[] playersPoints;

    protected Vector2 concaveMidpointError = new Vector2(-1000, 1000);

    public abstract void SetProperties(Player[] newPlayers, AbstractOpticalElement opticalElement);

    virtual public void Update()
    {
        /* Check for group is active */
        groupIsMoving = false;
        foreach (Player p in players)
        {
            if (p.IsMoving()) {
                groupIsMoving = true;
            }
        }

        /* Calculate midpoint for element */
        playersPoints = new Vector2[players.Length];
        for (int i = 0; i < playersPoints.Length; i++)
        {
            playersPoints[i] = new Vector2(players[i].transform.position.x, players[i].transform.position.z);
        }
        midpoint = CalculateElementMidpoint(playersPoints);

        //connection to midpoint here, vertices at object group
        foreach (Player p in players)
        {
            p.SetConnectionPoint(new Vector3(midpoint.x, floorHeight, midpoint.y));
        }
    }

    public bool GroupIntact()
    {
        bool intact = true;
        foreach (Player p in players)
        {
            if (p == null)
            {
                intact = false;
            }
        }
        return intact;
    }

    public void SetAllPlayersAvailable(bool isAvailable)
    {
        foreach (Player p in players)
        {
            if (p != null)
            {
                p.isAvailable = isAvailable;
            }
            else
            {
                Debug.LogWarning("Some Player in group is NULL");
                //TODO handle null-player interactions
            }
        }
    }

    public void ResetAllPlayersFriendCounters()
    {
        foreach (Player p in players)
        {
            if (p != null)
            {
                p.ResetFriendCounter();
            }
            else
            {
                Debug.LogWarning("Some Player in group is NULL");
                //TODO handle null-player interactions
            }
        }
    }

    virtual public PreviewObject GetPreview()
    {
        return null;
    }

    virtual public void Awake()
    {
        players = new Player[0];
        interactionManager = FindObjectOfType<InteractionManager>();
        timeLastMoved = Time.time;
    }

    public Quaternion CalculateElementRotation(Vector2[] points, Vector2 mid)
    {
        if (players.Length == 2 || players.Length == 3)
        {
            Vector3 target = new Vector3(points[0].x, floorHeight, points[0].y);
            Vector3 local = new Vector3(mid.x, floorHeight, mid.y);

            Vector3 lookTo = target - local;

            Quaternion result = Quaternion.LookRotation(lookTo) * Quaternion.AngleAxis(90f, Vector3.up);
            return result;
        }
        else if (players.Length == 4)
        {
            //calculate two midpoints
            Vector2 sideMidpointOne = CalculateLineMidpoint(points[0], points[1]);
            Vector2 sideMidpointTwo = CalculateLineMidpoint(points[1], points[2]);

            //angle in degrees
            float angleOne = Vector2.Angle(mid, sideMidpointOne);
            float angleTwo = Vector2.Angle(mid, sideMidpointTwo);
            float midAngle = Math.Abs(angleTwo - angleOne) * 0.5f + Math.Min(angleTwo, angleOne);

            //calculate back from the quads diagonal to the original rotation by 45°
            return Quaternion.AngleAxis(45f - midAngle, Vector3.up);
        }

        throw new ArgumentOutOfRangeException("Calculating rotation of defect GroupInteraction");
    }

    public Vector2 CalculateElementMidpoint(Vector2[] points)
    {
        if (players.Length == 2)
        {
            return CalculateLineMidpoint(points[0], points[1]);
        }
        else if (players.Length == 3)
        {
            Vector2 a1 = CalculateLineMidpoint(points[0], points[1]);
            Vector2 a2 = points[2];
            Vector2 b1 = CalculateLineMidpoint(points[1], points[2]);
            Vector2 b2 = points[0];

            float dy1 = a2.y - a1.y;
            float dx1 = a2.x - a1.x;
            float dy2 = b2.y - b1.y;
            float dx2 = b2.x - b1.x;

            float x = ((b1.y - a1.y) * dx1 * dx2 + dy1 * dx2 * a1.x - dy2 * dx1 * b1.x) / (dy1 * dx2 - dy2 * dx1);
            float y = a1.y + (dy1 / dx1) * (x - a1.x);

            return new Vector2(x, y);
        }
        else if (players.Length == 4)
        {
            if (this.GetComponent<MoveInteraction>())
            {
                //calculate midpoints
                Vector2 sideMidpointOne = CalculateLineMidpoint(points[0], points[1]);
                Vector2 sideMidpointTwo = CalculateLineMidpoint(points[2], points[3]);
                return CalculateLineMidpoint(sideMidpointOne, sideMidpointTwo);
            } 
            //long calculation of the correct vertex order is only happening in the time before Instantiating the Element
            else
            {
                if (QuadrilateralIntersects(points))
                {
                    //calculate midpoints
                    Vector2 sideMidpointOne = CalculateLineMidpoint(points[0], points[1]);
                    Vector2 sideMidpointTwo = CalculateLineMidpoint(points[2], points[3]);
                    return CalculateLineMidpoint(sideMidpointOne, sideMidpointTwo);
                }
                else
                {
                    Vector2 temp = points[1];
                    points[1] = points[2];
                    points[2] = temp;

                    if (QuadrilateralIntersects(points))
                    {
                        //calculate midpoints
                        Vector2 sideMidpointOne = CalculateLineMidpoint(points[0], points[1]);
                        Vector2 sideMidpointTwo = CalculateLineMidpoint(points[2], points[3]);
                        return CalculateLineMidpoint(sideMidpointOne, sideMidpointTwo);
                    }
                    else
                    {
                        temp = points[1];
                        points[1] = points[2];
                        points[2] = temp;

                        temp = points[2];
                        points[2] = points[3];
                        points[3] = temp;

                        if (QuadrilateralIntersects(points))
                        {
                            //calculate midpoints
                            Vector2 sideMidpointOne = CalculateLineMidpoint(points[0], points[1]);
                            Vector2 sideMidpointTwo = CalculateLineMidpoint(points[2], points[3]);
                            return CalculateLineMidpoint(sideMidpointOne, sideMidpointTwo);
                        }
                        else
                        {
                            //concave!!
                            return concaveMidpointError;
                        }
                    }
                }
            }
        }
        throw new ArgumentOutOfRangeException("Calculating midpoint of defect GroupInteraction. Players size: " + players.Length);
    }

    //TODO implement this function
    public Vector3[] CalculateElementVertices(Quaternion rotation, Vector2 midpoint)
    {
        Vector3[] vertices;
        if (players.Length == 2)
        {
            vertices = new Vector3[2];
            //TODO calculate with rotation quaternion
            Vector3 midpoint3D = new Vector3(midpoint.x, floorHeight, midpoint.y);
            return vertices;

        }
        else if (players.Length == 3)
        {
            vertices = new Vector3[3];
            Vector3 midpoint3D = new Vector3(midpoint.x, floorHeight, midpoint.y);

            Quaternion[] rotations = new Quaternion[] { Quaternion.AngleAxis(60, Vector3.up), Quaternion.AngleAxis(180, Vector3.up), Quaternion.AngleAxis(300, Vector3.up) };

            for(int i = 0; i < vertices.Length; i++)
            {
                //TODO
                //change rotation of vertices[i]
                //add distance to vertices[i]
            }
            return vertices;
        }
        else if (players.Length == 4)
        {
            vertices = new Vector3[4];
            //TODO calculate with rotation quaternion
            Vector3 midpoint3D = new Vector3(midpoint.x, floorHeight, midpoint.y);
            return vertices;
        }

        throw new ArgumentOutOfRangeException("Calculating vertices of defect GroupInteraction");
    }

    protected bool QuadrilateralIntersects(Vector2[] pointList)
    {
        Vector2 a1 = pointList[0];
        Vector2 a2 = pointList[2];
        Vector2 b1 = pointList[1];
        Vector2 b2 = pointList[3];

        float denominator = ((a2.x - a1.x) * (b2.y - b1.y)) - ((a2.y - a1.y) * (b2.x - b1.x));
        float numerator1 = ((a1.y - b1.y) * (b2.x - b1.x)) - ((a1.x - b1.x) * (b2.y - b1.y));
        float numerator2 = ((a1.y - b1.y) * (a2.x - a1.x)) - ((a1.x - b1.x) * (a2.y - a1.y));

        // Detect coincident lines
        if (denominator == 0) return numerator1 == 0 && numerator2 == 0;

        float r = numerator1 / denominator;
        float s = numerator2 / denominator;

        return (r >= 0 && r <= 1) && (s >= 0 && s <= 1);
    }

    protected Vector2 CalculateLineMidpoint(Vector2 p1, Vector2 p2)
    {
        //float x = (Math.Max(p1.x, p2.x) - Math.Min(p1.x, p2.x)) * 0.5f + Math.Min(p1.x, p2.x);
        float x = Math.Abs(p2.x - p1.x) * 0.5f + Math.Min(p1.x, p2.x);
        float y = Math.Abs(p2.y - p1.y) * 0.5f + Math.Min(p1.y, p2.y);
        return new Vector2(x, y);
    }

    #region playersListHandling

    public bool ContainsPlayer(Player player)
    {
        return players.Contains(player);
    }

    public abstract void RemovePlayer(Player player);

    #endregion
}

