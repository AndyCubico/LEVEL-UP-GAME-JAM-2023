using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvertSun : MonoBehaviour
{
    [SerializeField] private float Radius;
    [SerializeField] private float RadiusDiff;
    [SerializeField] private EdgeCollider2D Out;
    [SerializeField] private EdgeCollider2D In;
    [SerializeField] private LineRenderer sun;
    private GameObject player;

    [SerializeField] private int NumEdges;

    private void Start()
    {
        player = GameObject.Find("Player");
    }
    private void OnValidate()
    {
        Generate();
    }
    private void Update()
    {
        Generate();
    }
    private void Generate()
    {
        sun.SetWidth((Radius - RadiusDiff), (Radius - RadiusDiff));

        Vector2[] points = new Vector2[NumEdges + 1];
        sun.positionCount = NumEdges + 1;

        for (int i = 0; i < NumEdges; i++)
        {
            float angle = 2 * Mathf.PI * i / NumEdges;
            float x = Radius * Mathf.Cos(angle) + transform.position.x;
            float y = Radius * Mathf.Sin(angle) + transform.position.y;

            points[i] = new Vector2(x, y);
        }

        points[NumEdges] = points[0];
        Out.points = points;

        for (int i = 0; i < NumEdges; i++)
        {
            float angle = 2 * Mathf.PI * i / NumEdges;
            float x = ((Radius + RadiusDiff) / 2) * Mathf.Cos(angle) + (transform.position.x * 2);
            float y = ((Radius + RadiusDiff) / 2) * Mathf.Sin(angle) + (transform.position.y * 2);

            sun.SetPosition(i, new Vector3(x, y, 0));

            points[i] = new Vector2(x, y);
        }

        sun.SetPosition(NumEdges, new Vector3(points[0].x, points[0].y, 0));

        for (int i = 0; i < NumEdges; i++)
        {
            float angle = 2 * Mathf.PI * i / NumEdges;
            float x = (RadiusDiff) * Mathf.Cos(angle) + transform.position.x;
            float y = (RadiusDiff) * Mathf.Sin(angle) + transform.position.y;

            points[i] = new Vector2(x, y);
        }

        points[NumEdges] = points[0];
        In.points = points;

        if (IsPlayerIn())
        {
            Debug.Log("Dentro");
        }
    }

    private bool IsPlayerIn()
    {
        bool ret = false;

        if (Radius != RadiusDiff)
        {
            if ((player.transform.position.x <= transform.position.x + Radius && player.transform.position.x >= transform.position.x - Radius &&
               player.transform.position.y <= transform.position.y + Radius && player.transform.position.y >= transform.position.y - Radius))
            {
                if ((player.transform.position.x >= transform.position.x + RadiusDiff || player.transform.position.y >= transform.position.y + RadiusDiff ||
                    player.transform.position.x <= transform.position.x - RadiusDiff || player.transform.position.y <= transform.position.y - RadiusDiff))
                {
                    ret = true;
                }
            }
        }
        else 
        {
            if ((player.transform.position.x <= transform.position.x + Radius && player.transform.position.x >= transform.position.x - Radius &&
               player.transform.position.y <= transform.position.y + Radius && player.transform.position.y >= transform.position.y - Radius))
            {
                ret = true;
            }
        }

        return ret;
    }
}