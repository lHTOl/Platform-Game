using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterScript : MonoBehaviour
{
    [SerializeField] private float MoveSpeed = 10;
    [SerializeField] private float JumpSpeed = 20;
    [SerializeField] private Transform HealtUI;
    [SerializeField] private HealtSystem CharacterHealt;

    public event EventHandler<InputBool> InputStartOrStop;
    public class InputBool : EventArgs
    {
        public bool InputEnabled;
    }

    private PlayerInput Inputs;
    private Rigidbody2D PlayerRigidbody;
    private CapsuleCollider2D PlayerBodyCollider;
    private BoxCollider2D PlayerFeetCollider;
    private Animator PlayerAnim;
    private Vector2 MoveInput;
    private bool Jump;
    private GameManager Gamemanager;
    private CheckpointSystem Checkpointsystem;

    void Start()
    {
        Inputs = GetComponent<PlayerInput>();
        PlayerRigidbody = GetComponent<Rigidbody2D>();
        PlayerBodyCollider = GetComponent<CapsuleCollider2D>();
        PlayerFeetCollider = GetComponent<BoxCollider2D>();
        PlayerAnim = transform.GetChild(0).GetComponent<Animator>();
        Gamemanager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        Checkpointsystem = GameObject.FindGameObjectWithTag("Checkpoints").GetComponent<CheckpointSystem>();
        CharacterHealt.Healt = Mathf.Clamp(CharacterHealt.Healt, 0, 3);
        Jump = true;
    }

    void Update()
    {
        Run();
        Flip();
    }

    public void SetMoveInput(float value)
    {
        MoveInput = new Vector2(value, PlayerRigidbody.velocity.y);
    }

    void OnMove(InputValue value)
    {
        MoveInput = value.Get<Vector2>();
    }
    void OnPause()
    {
        Gamemanager.Pause();
    }

    private void Run()
    {
        Vector2 PlayerVelocity = new Vector2(MoveInput.x * MoveSpeed, PlayerRigidbody.velocity.y);
        PlayerRigidbody.velocity = PlayerVelocity;
        PlayerAnim.SetFloat("Speed", PlayerRigidbody.velocity.magnitude);
    }

    private void Flip()
    {
        if (MoveInput.x > 0)
            transform.localScale = new Vector2(-1, 1);
        if (MoveInput.x < 0)
            transform.localScale = new Vector2(1, 1);
    }

    void OnJump(InputValue value)
    {
        if (!PlayerFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) && Jump)
            return;
        if (value.isPressed)
        {
            PlayerRigidbody.velocity += new Vector2(0f, JumpSpeed);
            PlayerAnim.SetBool("Jump", true);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<EnemyMovement>() || collision.gameObject.CompareTag("MapOff"))
        {
            StartCoroutine(Damage());
        }
        if(collision.collider.CompareTag("Trap"))
        {
            StartCoroutine(Damage());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (PlayerFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            PlayerAnim.SetBool("Jump", false);
        }
        if (collision.CompareTag("Checkpoint"))
        {
            collision.transform.GetChild(0).GetComponent<Animator>().SetTrigger("CheckTrigger");
            collision.transform.parent.GetComponent<CheckpointSystem>().SetLastCheckpoint(collision.transform);
            Destroy(collision.transform.GetComponent<BoxCollider2D>());
            Destroy(collision.transform.GetChild(0).GetComponent<Animator>(), 2);
        }

        if (collision.CompareTag("FinishMap"))
        {
            Gamemanager.Finish();
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!PlayerFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            PlayerAnim.SetBool("Jump", true);
        }
    }

    IEnumerator Damage()
    {
        PlayerBodyCollider.isTrigger = true;
        PlayerRigidbody.constraints = RigidbodyConstraints2D.FreezePositionY;
        InputStartStop();
        CharacterHealt.Damage(1);
        HealtUI.GetChild(CharacterHealt.Healt).GetComponent<Animator>().SetTrigger("Damage");
        if (CharacterHealt.Healt > 0)
        {
            //Animation Dead
            PlayerAnim.SetTrigger("Dead");
            yield return new WaitForSeconds(1);
            PlayerBodyCollider.isTrigger = false;
            PlayerRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            InputStartStop();
            //Last checkpoint
            transform.position = Checkpointsystem.GetLastCheckpoint().position;
        }
        else
        {
            //Animation Dead
            PlayerAnim.SetTrigger("Dead");
            yield return new WaitForSeconds(1);
            //character Dead
            Gamemanager.Died();
        }
    }

    private void InputStartStop()
    {
        if (Inputs.isActiveAndEnabled)
        {
            Inputs.enabled = false;
            InputStartOrStop?.Invoke(this, new InputBool { InputEnabled = false });
        }
        else if (!Inputs.isActiveAndEnabled)
        {
            Inputs.enabled = true;
            InputStartOrStop?.Invoke(this, new InputBool { InputEnabled = true });
        }
    }
}