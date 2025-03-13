using System;
using UnityEditor.Callbacks;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    float horizontalInput;
    float moveSpeed = 5f;
    bool isFacingRight = false;
    Rigidbody2D rb;
    Animator animator;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        FlipSprite();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocityY);
        animator.SetFloat("xVelocity", Math.Abs(rb.linearVelocityX));
    }

    void FlipSprite(){
        if(isFacingRight && horizontalInput < 0f || !isFacingRight && horizontalInput > 0f){
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }
}
