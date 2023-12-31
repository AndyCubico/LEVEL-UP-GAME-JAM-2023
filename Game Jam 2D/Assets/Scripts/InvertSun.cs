using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvertSun : MonoBehaviour
{
    [SerializeField] public float Radius;
    [SerializeField] public float RadiusDiff;
    [SerializeField] private EdgeCollider2D Out;
    [SerializeField] private EdgeCollider2D In;
    [SerializeField] private LineRenderer sun;
    private GameObject player;

    [SerializeField] private BoolSO isPlayerInLight;

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

        if (Input.GetKeyUp(KeyCode.R) || Input.GetKeyUp(KeyCode.JoystickButton3)) { DeleteSun(); }
        else if (GameObject.Find("Boss").GetComponent<BossBehaviour>().boss_Mode != BOSS_MODE.BOSS_MODE_SUN) { DeleteSun(); }
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
            float x = (RadiusDiff + 0.1f) * Mathf.Cos(angle) + transform.position.x;
            float y = (RadiusDiff + 0.1f) * Mathf.Sin(angle) + transform.position.y;

            points[i] = new Vector2(x, y);
        }

        points[NumEdges] = points[0];
        In.points = points;

        if (IsPlayerIn())
        {
            isPlayerInLight.Value = true;
        }
        else 
        {
            isPlayerInLight.Value = false;
        }
    }

    public bool IsPlayerIn()
    {
        bool ret = true;

        //if ((player.transform.position.x <= (transform.position.x * 2) + Radius && player.transform.position.x >= (transform.position.x * 2) - Radius &&
        //    player.transform.position.y <= (transform.position.y * 2) + Radius && player.transform.position.y >= (transform.position.y * 2) - Radius))
        //{
        //    if ((player.transform.position.x >= (transform.position.x * 2) + RadiusDiff || player.transform.position.y >= (transform.position.y * 2) + RadiusDiff ||
        //        player.transform.position.x <= (transform.position.x * 2) - RadiusDiff || player.transform.position.y <= (transform.position.y * 2) - RadiusDiff))
        //    {
        //        ret = true;
        //    }
        //}

        return ret;
    }

    public void DeleteSun()
    {
        isPlayerInLight.Value = false;
        GetComponent<LineRenderer>().enabled = false;
        Out.enabled = false;
        In.enabled = false;
        Destroy(GetComponent<LineRenderer>().gameObject);
        Destroy(Out.gameObject);
        Destroy(In.gameObject);
    }
}