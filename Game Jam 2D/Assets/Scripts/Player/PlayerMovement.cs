using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D playerRb;
    private Vector2 movementInput;
    private Animator playerAnimator;

    public bool _canMove = true;
    [SerializeField] private float speed;

    // [Smm] dashing vars
    private bool _canDash = true;
    [SerializeField] private float _dashPower = 2.0f;
    [SerializeField] private float _dashTime;
    [SerializeField] private float _dashCooldown;
    [SerializeField] private TrailRenderer _dashTrail;

    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();

        _dashTrail = GetComponent<TrailRenderer>();
    }

    private void Update()
    {
        if(_canMove)
        {
            return;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        movementInput = new Vector2(moveX, moveY).normalized;

        playerAnimator.SetFloat("Horizontal", moveX);
        playerAnimator.SetFloat("Vertical", moveY);
        playerAnimator.SetFloat("Speed", movementInput.sqrMagnitude);

        if (Input.GetKeyDown(KeyCode.Space) && _canDash)
        {
            StartCoroutine(Dash());
        }
    }
    private void FixedUpdate()
    {
        if (_canMove)
        {
            return;
        }

        playerRb.MovePosition(playerRb.position + movementInput * speed * Time.fixedDeltaTime);
    }

    // [Smm] Dashing: If dash, multiply the speed by the dash power. When finish, divide to return to the original speed.
    private IEnumerator Dash()
    {
        _canDash = false;
        _canMove = true;

        _dashTrail.emitting = true;

        speed *= _dashPower;
        
        // [Smm] Pauses the function in this exact line and the next frame or when the time to wait is over, it continues from here. (creo)
        // SDL Delay pero bien, que no peta todo el juego
        yield return new WaitForSeconds(_dashTime); 

        _dashTrail.emitting = false;
        _canMove = false;
        speed /= _dashPower;

        yield return new WaitForSeconds(_dashCooldown);
        _canDash = true;
    }
}
