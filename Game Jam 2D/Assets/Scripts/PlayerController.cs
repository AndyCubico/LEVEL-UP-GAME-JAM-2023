using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float horizontal;
    private float vertical;
    [SerializeField] private float speed = 6.0f;
    private bool Up; //Para hacer más tarde el flip del sprite
    private bool Down; //Para hacer más tarde el flip del sprite

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        GetComponentInParent<Rigidbody2D>().velocity = new Vector2(horizontal * speed, vertical * speed);
    }
}
