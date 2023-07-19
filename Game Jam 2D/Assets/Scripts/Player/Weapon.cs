using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private Camera mainCam;
    private Vector3 mousePos;
    private Vector2 rightStickInput;

    private void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    private void Update()
    {
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        Vector3 rotation = mousePos - transform.position;

        float rotz = Mathf.Atan2(rotation.y, rotation.x)*Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, rotz);

        // [Andy] Gamepad control weapon
        rightStickInput = new Vector2(Input.GetAxis("R_Horizontal"), Input.GetAxis("R_Vertical"));
        if (rightStickInput.magnitude>0f)
        {
            float rotG = Mathf.Atan2(rightStickInput.y, rightStickInput.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, rotG);
        }

        if (transform.rotation.eulerAngles.z < 270.0f && transform.rotation.eulerAngles.z > 90.0f)
        {
            transform.localScale = new Vector3(1, -1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
