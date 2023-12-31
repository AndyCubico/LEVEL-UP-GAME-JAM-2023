using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D playerRb;
    private Vector2 movementInput;
    private Animator playerAnimator;

    public bool _stopMove = false;
    public bool rechargeStop = false;// [Andy] not moving while recharging
    public float speed;

    // [Smm] dashing vars
    private bool _canDash = true;
    [SerializeField] private float _dashPower = 2.0f;
    [SerializeField] private float _dashTime;
    [SerializeField] private float _dashCooldown;
    private TrailRenderer _dashTrail;

    //Audio
    private AudioSource audioSource;
    [SerializeField] private AudioSource walkSource;
    [SerializeField] private AudioClip walkClip;
    [SerializeField] private AudioClip dashClip;
    public AudioManager audioMan;

    void Start()
    {
        GameObject.DontDestroyOnLoad(this.gameObject);

        playerRb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        _dashTrail = GetComponent<TrailRenderer>();
        _stopMove = false;
    }

    private void Update()
    {
        if(_stopMove || LevelUpMenu.isPaused || PauseMenu.isPausedMenu)
        {
            return;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        if (moveX != 0 || moveY != 0)
        {
            if (!walkSource.isPlaying)
            {
                audioMan.PlayAudio(walkSource, walkClip);
            }
        }
        movementInput = new Vector2(moveX, moveY).normalized;

        playerAnimator.SetFloat("Horizontal", moveX);
        playerAnimator.SetFloat("Vertical", moveY);
        playerAnimator.SetFloat("Speed", movementInput.sqrMagnitude);

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton2)) && _canDash)
        {
            Debug.Log("dash");
            StartCoroutine(Dash());
        }
    }
    private void FixedUpdate()
    {
        if (rechargeStop)
        {
            return;
        }
        playerRb.MovePosition(playerRb.position + movementInput * speed * Time.fixedDeltaTime);

    }

    // [Smm] Dashing: If dash, multiply the speed by the dash power. When finish, divide to return to the original speed.
    private IEnumerator Dash()
    {
        _canDash = false;
        _stopMove = true;

        _dashTrail.emitting = true;

        speed *= _dashPower;
        audioMan.PlayAudio(audioSource, dashClip);

        // [Smm] Pauses the function in this exact line and the next frame or when the time to wait is over, it continues from here. (creo)
        // SDL Delay pero bien, que no peta todo el juego
        yield return new WaitForSeconds(_dashTime); 

        _dashTrail.emitting = false;
        _stopMove = false;
        speed /= _dashPower;

        yield return new WaitForSeconds(_dashCooldown);
        _canDash = true;
    }
}
