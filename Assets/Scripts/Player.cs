using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Player : MonoBehaviour
{


    public bl_Joystick joystick;
    private Animator animator;
    private Rigidbody2D rigidbody2D;

    private float Speed = 4.5f;
    private float FallingSpeed = 9.8f;

    private bool isTriggerLadder = false;
    private bool isJumping = false;
    private bool isDied = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isDied) return;
        if (isJumping)
        {
            animator.Play("Jump");
        }
        else
        {
            float v = joystick.Vertical; //get the vertical value of joystick
            float h = joystick.Horizontal;//get the horizontal value of joystick
            if (isTriggerLadder && math.abs(v) > 3)
            {
                Vector3 translate = (new Vector3(0, v / math.abs(v), 0) * Time.deltaTime) * Speed;
                transform.Translate(translate);
                animator.Play("Climb");
                rigidbody2D.gravityScale = 0;
            }
            else
            {
                if (math.abs(h) <= 0.01f)
                {
                    animator.Play("Idle");
                }
                else
                {
                    if (h > 0)
                    {
                        transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                    }
                    else
                    {
                        transform.rotation = Quaternion.Euler(0.0f, 180f, 0.0f);
                    }
                    Vector3 translate = (new Vector3(1, 0, 0) * Time.deltaTime) * Speed;
                    transform.Translate(translate);
                    animator.Play("Run");
                }
            }
        }
       

    }

    public void Die()
    {
        animator.Play("Hurt");
        GameManager.instance.PlayerDied();
        isDied = true;
    }

    public void HurtAnimationEnd()
    {
        GameManager.instance.Rebirth();
        isDied = false;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ladder")
        {
            isTriggerLadder = true;
        }
        
        if (collision.tag == "JumpBlock")
        {
            isJumping = true;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ladder")
        {
            isTriggerLadder = false;
            rigidbody2D.gravityScale = 1;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "Ground")
        {
            isJumping = false;
        }
    }
}
