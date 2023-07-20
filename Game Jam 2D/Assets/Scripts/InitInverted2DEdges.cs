using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitInverted2DEdges : MonoBehaviour
{
    [SerializeField] private float Radius;

    [SerializeField] private int NumEdges;

    private void Start()
    {
        Generate();
    }
    private void OnValidate()
    {
        Generate();
    }
    private void Generate()
    {
        Vector2[] points = new Vector2[NumEdges + 1];

        for (int i = 0; i < NumEdges; i++)
        {
            float angle = 2 * Mathf.PI * i / NumEdges;
            float x = Radius * Mathf.Cos(angle);
            float y = Radius * Mathf.Sin(angle);

            points[i] = new Vector2(x, y);
        }

        points[NumEdges] = points[0];
        GetComponentInParent<EdgeCollider2D>().points = points;
    }
}