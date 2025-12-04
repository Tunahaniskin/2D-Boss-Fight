using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    
    [SerializeField]float Player_speed = 4.0f;
    float dashSpeed = 50.0f;
    float Player_jumpForce = 9.0f;
    private Rigidbody2D rb;
    private bool isGrounded;
    private Animator animator;
    private float CanDashTime = 0f;
    private float DashWaitTime = 2f;

    [SerializeField] private int PlayerHealth = 20;
    [SerializeField] private GameObject AttackArea;
    private float AttackStartDuration = 1.1f;
    private float AttackCanDamageDuration = 0.3f;
    private bool isAttacking = false;
    [SerializeField] private AudioSource jumpSoundSource;


    void Start()
    {
       rb = GetComponent<Rigidbody2D>();
       animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        HandleMovementInput();
        handleJumpInput();
        HandleJumpFallAnimation();
        HandleAttackInput();
        
    }
    
    void FixedUpdate()
    {
        
    }

    private void HandleMovementInput()
    {   
        
        float moveDirection = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveDirection * Player_speed, rb.linearVelocity.y);

        // Koşma animasyonunu kontrol et
        if(moveDirection == 0)
        {
            animator.SetBool("Run", false);
        }
        else
        {
            animator.SetBool("Run", true);
        }
        
            if(moveDirection > 0)
        {
            
            transform.localScale = new Vector3(5, 5, 1); // sağa bak
            
            if(Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= CanDashTime)
            {
                rb.AddForce(new Vector2(dashSpeed, 0), ForceMode2D.Impulse);
                CanDashTime = Time.time + DashWaitTime;
                animator.SetTrigger("Dash");
                 
            }
             
        }
        else if(moveDirection < 0)
        {
        
            transform.localScale = new Vector3(-5, 5, 1); // sola bak
            
            
            if(Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= CanDashTime)
            {
                rb.AddForce(new Vector2(-dashSpeed, 0), ForceMode2D.Impulse);
                CanDashTime = Time.time + DashWaitTime;
                animator.SetTrigger("Dash");
            }      
        }   
        
    }

    private void handleJumpInput()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && isGrounded)
        {
            rb.AddForce(new Vector2(0, Player_jumpForce), ForceMode2D.Impulse);
            jumpSoundSource.Play();
            isGrounded = false;
              
        }
        else
        {
            animator.SetInteger("Jump", 0);
        }
    }

    private void HandleJumpFallAnimation()
    {

        if (isGrounded)
        {
            return;
        }

        float verticalVelocity = rb.linearVelocity.y;
        
        if (verticalVelocity > 0)
        {
            animator.SetInteger("Jump", 1); // Zıplama animasyonu
        }
        else if (verticalVelocity < 0)
        {
            animator.SetInteger("Jump", 2); // Düşme animasyonu
        }
    }

    private void HandleAttackInput()
    {
        //TODO: trigerenter ise enemy take damage çağır
        if (Input.GetMouseButtonDown(0) && !isAttacking) // 0 = sol fare tuşu
        {
            StartCoroutine(PerformAttack());
            
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(AttackCanDamageDuration); //Animasyonel ayarlama
        AttackArea.SetActive(true);    
        yield return new WaitForSeconds(AttackStartDuration - AttackCanDamageDuration);
        AttackArea.SetActive(false);
        isAttacking = false;
    }   

    private void TakeDamage(int damage)
    {
        PlayerHealth -= damage;

        animator.SetTrigger("Hurt");
        
        if (PlayerHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        //TODO: canvastan oyunu bitir
        animator.SetBool("Death", true);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
           
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
     
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
           
        }
    }



}
