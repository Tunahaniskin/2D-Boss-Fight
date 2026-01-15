using System.Collections;
using System.IO;
using UnityEngine;

public class Enemy_sc : MonoBehaviour
{
    
    private Animator animator;
    private Rigidbody2D rb;
    private Transform playerTransform;

   
    [SerializeField] float Enemy_speed = 4.0f;
    [SerializeField] private BoxCollider2D AttackArea;
    [SerializeField] private int EnemyHealth = 50;
    private float pendingReward = 0f; 
    
    
    [Header("Movement Settings")]
    [SerializeField] float Enemy_jumpForce = 9.0f;
    [SerializeField] float dashSpeed = 10.0f; 
    private float CanDashTime = 0f;
    private float DashWaitTime = 5f;
    private bool isGrounded = true;

    private bool isAttacking = false;
    private float AttackStartDuration = 1.1f;
    private float AttackCanDamageDuration = 0.3f;

   
    private QLearningBrain brain;
    
    // 0: Bekle
    // 1: Sola Yürü
    // 2: Sağa Yürü
    // 3: Saldır
    // 4: Zıpla
    // 5: Dash At
    private int actionCount = 6; 
    
    public bool isTraining = true; 
    [Range(0, 1)] public float epsilon = 0.1f; 

    private float decisionTimer = 0f;
    private float decisionInterval = 0.2f;

    private string currentState;
    private int currentAction;

    public static string loadedBrainData = ""; 
    public static bool usePreTrainedAI = false; 

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (GameUIManager.Instance != null) 
            GameUIManager.Instance.UpdateEnemyHealth(EnemyHealth, 20); // 20 maks can
        
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) playerTransform = p.transform;

        if (AttackArea != null) AttackArea.enabled = false;

        InitializeBrain();

        currentState = GetState();
    }

    void InitializeBrain()
    {
        brain = new QLearningBrain(actionCount);
        if (usePreTrainedAI && !string.IsNullOrEmpty(loadedBrainData))
        {
            brain.LoadBrain(loadedBrainData);
            isTraining = false; 
            epsilon = 0f;       
            Debug.Log("Düşman: Eğitilmiş modda.");
        }
    }

    void FixedUpdate()
    {
        if (playerTransform == null) return;

        if (Time.time > decisionTimer && !isAttacking)
        {
            decisionTimer = Time.time + decisionInterval;
            ThinkAndAct();
        }

        HandleJumpFallAnimation();

        // Eğitim Kayıt Tuşu (K)
        if (Input.GetKeyDown(KeyCode.K) && isTraining)
        {
            string path = Path.Combine(Application.streamingAssetsPath, "enemy_weights.json");
            if (!Directory.Exists(Application.streamingAssetsPath)) 
                Directory.CreateDirectory(Application.streamingAssetsPath);
            brain.SaveBrain(path);
        }
    }

    void ThinkAndAct()
    {
        if (brain == null) InitializeBrain();

        string state = GetState();
        
        int action = brain.GetAction(state, isTraining ? epsilon : 0f);

        PerformAction(action);

        if (isTraining)
        {
            float reward = CalculateReward(action);
            string nextState = GetState();
            brain.Learn(state, action, reward, nextState);
        }

        currentState = state;
        currentAction = action;
    }

    string GetState()
    {
        if (playerTransform == null) return "Dead";

        float distance = Vector2.Distance(transform.position, playerTransform.position);
        float xDiff = playerTransform.position.x - transform.position.x;

        string distState = "Far";
        if (distance < 3.5f) distState = "AttackRange";
        else if (distance < 6.0f) distState = "Close";

        string dirState = (xDiff > 0) ? "PlayerRight" : "PlayerLeft";
        
        string groundState = isGrounded ? "Ground" : "Air";

        return distState + "_" + dirState + "_" + groundState; 
    }

    void PerformAction(int action)
    {

        float currentY = rb.linearVelocity.y;
        
        // 0: Bekle
        if (action == 0)
        {
            rb.linearVelocity = new Vector2(0, currentY);
            animator.SetBool("Walk", false);
        }
        // 1: Sola Git
        else if (action == 1)
        {
            rb.linearVelocity = new Vector2(-Enemy_speed, currentY);
            transform.localScale = new Vector3(5, 5, 1); // Sola bak
            animator.SetBool("Walk", true);
        }
        // 2: Sağa Git
        else if (action == 2)
        {
            rb.linearVelocity = new Vector2(Enemy_speed, currentY);
            transform.localScale = new Vector3(-5, 5, 1); // Sağa bak
            animator.SetBool("Walk", true);
        }
        // 3: Saldır
        else if (action == 3 && isGrounded)
        {
            // Hareket etmeyi durdur ve saldır
            rb.linearVelocity = new Vector2(0, currentY);
            StartCoroutine(PerformAttack());
        }
        // 4: zıpla
        else if (action == 4)
        {
            if (isGrounded)
            {
                rb.AddForce(new Vector2(0, Enemy_jumpForce), ForceMode2D.Impulse);
                isGrounded = false;
            }
        }
        // 5: dash at
        else if (action == 5)
        {
            if (Time.time >= CanDashTime)
            {
                // Baktığı yöne doğru dash atar
                float dashDirection = transform.localScale.x > 0 ? -1 : 1; 
                
                rb.AddForce(new Vector2(dashDirection * dashSpeed, 0), ForceMode2D.Impulse);
                CanDashTime = Time.time + DashWaitTime;
            }
        }
    }

    private void HandleJumpFallAnimation()
    {
        if (isGrounded)
        {  
            return;
        }
        
    }

  float CalculateReward(int action)
    {
        float reward = -0.01f; // Var olma cezası
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        // --- 1. KISIM: GERÇEK HASAR ÖDÜLÜ ---
        if (pendingReward > 0)
        {
            reward += pendingReward;
            pendingReward = 0f; 
        }

        if (action == 3) // Saldırı
        {
            // A) YÖN KONTROLÜ
            // Player sağımda mı solumda mı?
            float xDiff = playerTransform.position.x - transform.position.x;
            bool isPlayerRight = xDiff > 0;
            
            // Ben sağa mı bakıyorum sola mı? (Senin koduna göre -5 Sağa, +5 Sola bakıyor)
            bool amILookingRight = transform.localScale.x < 0; 
            
            // Yüzüm Player'a dönük mü?
            // (Player Sağda VE Ben Sağa Bakıyorum) VEYA (Player Solda VE Ben Sola Bakıyorum)
            bool isFacingCorrectly = (isPlayerRight && amILookingRight) || (!isPlayerRight && !amILookingRight);

            if (!isFacingCorrectly)
            {
                // Yüzün dönük değilse direkt büyük ceza ver!
                reward -= 0.5f; // "Arkana bakarak kime vuruyorsun?"
            }
            else
            {
                // B) MESAFE KONTROLÜ (Sadece yüzü dönükse buraya girer)
                
                // Durum A: İdeal Menzil -> KÜÇÜK İPUCU ÖDÜLÜ
                if (distance >= 2.0f && distance <= 3.5f) 
                {
                    reward += 0.2f; 
                }
                // Durum B: Çok Yakın -> KÜÇÜK CEZA
                else if (distance < 1.0f) 
                {
                    reward -= 0.2f; 
                }
                // Durum C: Iska (Menzil dışı) -> BÜYÜK CEZA
                else 
                {
                    reward -= 0.5f; 
                }
            }
        }
        // 2. Dash Ödülleri
        else if (action == 5)
        {
            // A) YÖN KONTROLÜ
            // Player sağımda mı solumda mı?
            float xDiff = playerTransform.position.x - transform.position.x;
            bool isPlayerRight = xDiff > 0;
            
            // Ben sağa mı bakıyorum sola mı? (Senin koduna göre -5 Sağa, +5 Sola bakıyor)
            bool amILookingRight = transform.localScale.x < 0; 
            
            // Yüzüm Player'a dönük mü?
            // (Player Sağda VE Ben Sağa Bakıyorum) VEYA (Player Solda VE Ben Sola Bakıyorum)
            bool isFacingCorrectly = (isPlayerRight && amILookingRight) || (!isPlayerRight && !amILookingRight);

            if (!isFacingCorrectly)
            {
                // Yüzün dönük değilse direkt büyük ceza ver!
                reward -= 0.5f;
            }
            else
            {
                // B) MESAFE KONTROLÜ (Sadece yüzü dönükse buraya girer)
                
                // Dash sonrası mesafe kontrolü
            if (distance > 4.0f) reward = 0.5f; 
            else if (distance < 2.0f) reward = -0.2f;
            }
        }
        // 3. Zıplama Ödülleri
        else if (action == 4)
        {
            if (playerTransform.position.y > transform.position.y + 1.0f) reward = 0.5f;
            else reward = -0.4f; 
        }
        // --- YÜRÜME ÖDÜLLERİ (Action 1: Sola, Action 2: Sağa) ---
        else if (action == 1 || action == 2)
        {
            float xDiff = playerTransform.position.x - transform.position.x;
            // Player'a doğru mu gidiyor?
            bool movingTowardsPlayer = (action == 1 && xDiff < 0) || (action == 2 && xDiff > 0);

            // DURUM A: Menzil Dışındayım (Uzağım) -> YAKLAŞMALI
            if (distance > 3.5f)
            {
                if (movingTowardsPlayer) reward += 0.1f; // Aferin, hedefe gidiyorsun.
                else reward -= 0.1f; // Yanlış yöne gidiyorsun, dön!
            }
            // DURUM B: Menzildeyim (Vurma Mesafesi) -> DURMALI
            else if (distance >= 2.0f && distance <= 3.5f)
            {
                reward -= 0.2f; // Menzildesin, neden hala yürüyorsun? Dur ve vur!
            }
            // DURUM C: Çok Dibindeyim -> GERİ ÇEKİLMELİ
            else if (distance < 1.0f)
            {
                if (!movingTowardsPlayer) reward += 0.1f; // Geri çekiliyor, aferin (Spacing).
                else reward -= 0.2f; // Daha da dibine girme!
            }
        }
        
        return reward;
    }
    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(AttackCanDamageDuration);
        if(AttackArea != null) AttackArea.enabled = true;
        yield return new WaitForSeconds(AttackStartDuration - AttackCanDamageDuration);
        if(AttackArea != null) AttackArea.enabled = false;
        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        EnemyHealth -= damage;

        if (GameUIManager.Instance != null) 
            GameUIManager.Instance.UpdateEnemyHealth(EnemyHealth, 20);

        if(isTraining && currentState != null)
        {
             brain.Learn(currentState, currentAction, -1.0f, GetState());
        }

        if (EnemyHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        animator.SetBool("Die", true);
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        rb.linearVelocity = Vector2.zero;

        if (GameUIManager.Instance != null)
            GameUIManager.Instance.GameFinished("KAZANDIN!");

        Destroy(gameObject, 2.5f); 
    }

    public void PlayerHasarAldi()
{
    pendingReward += 2.0f; // GERÇEK VURUŞ ÖDÜLÜ (Büyük Puan)

}
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) isGrounded = true;
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) isGrounded = false;
    }
}