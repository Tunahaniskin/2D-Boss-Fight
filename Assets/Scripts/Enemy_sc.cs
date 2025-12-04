using System.Collections;
using UnityEngine;

public class Enemy_sc : MonoBehaviour
{

    private Animator animator;
    private Rigidbody2D rb;
    [SerializeField]float Enemy_speed = 4.0f;
    float Enemy_jumpForce = 9.0f;
    private bool isGrounded;
    float dashSpeed = 50.0f;
    private float CanDashTime = 0f;
    private float DashWaitTime = 2f;
    private bool isAttacking = false;
    [SerializeField] private GameObject AttackArea;
    private float AttackStartDuration = 1.1f;
    private float AttackCanDamageDuration = 0.3f;

    [SerializeField] private int EnemyHealth = 50;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

   
    void Update()
    {
        HandleMovementInput();
        HandleAttackInput();
        handleJumpInput();
    }
    void FixedUpdate()
    {
        
    }

    private void TakeDamage(int damage)
    {
        EnemyHealth -= damage;
        if(EnemyHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        animator.SetBool("Die",true);
    }
private void HandleMovementInput()
    {   
        
        float moveDirection = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveDirection * Enemy_speed, rb.linearVelocity.y);

        // Koşma animasyonunu kontrol et
        if(moveDirection == 0)
        {
            animator.SetBool("Walk", false);
        }
        else
        {
            animator.SetBool("Walk", true);
        }
        
            if(moveDirection < 0)
        {
            
            transform.localScale = new Vector3(5, 5, 1); // sola bak
            //dash aksiyonu
            if(Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= CanDashTime)
            {
                rb.AddForce(new Vector2(-dashSpeed, 0), ForceMode2D.Impulse);
                CanDashTime = Time.time + DashWaitTime;
                
                 
            }
             
        }
        else if(moveDirection > 0)
        {
        
            transform.localScale = new Vector3(-5, 5, 1); // sağa bak
            //dash aksiyonu
            if(Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= CanDashTime)
            {
                rb.AddForce(new Vector2(dashSpeed, 0), ForceMode2D.Impulse);
                CanDashTime = Time.time + DashWaitTime;
                
                 
            }
             
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
    private void handleJumpInput()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && isGrounded)
        {
            rb.AddForce(new Vector2(0, Enemy_jumpForce), ForceMode2D.Impulse);
            isGrounded = false;
           
        }
        
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
