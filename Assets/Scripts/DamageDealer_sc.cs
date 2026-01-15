using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] private int damageAmount = 5;
    [SerializeField] private string targetTag; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. KORUMA: Kendi kendime (veya kendi gövdeme) vurmayayım.
        if (collision.transform.root == transform.root) return;

        // --- 2. KORUMA
        // Eğer çarptığım şeyin üzerinde de "DamageDealer" scripti varsa,
        // bu demektir ki bir silahın ucuna veya AttackArea'ya vurdum.
        // Silaha vurunca adam ölmez, o yüzden işlemi iptal et.
        if (collision.GetComponent<DamageDealer>() != null) return;
        // ----------------------------------------------

        if (targetTag == "Player")
        {
            Player player = collision.GetComponentInParent<Player>();
            if (player != null)
            {
                player.TakeDamage(damageAmount);
                
                // Kılıcın sahibi olan Enemy'yi bul ve ödülünü ver
                Enemy_sc owner = GetComponentInParent<Enemy_sc>();
                if (owner != null)
                {
                    owner.PlayerHasarAldi();
                }
                
            }
        }
        else if (targetTag == "Enemy")
        {
            Enemy_sc enemy = collision.GetComponentInParent<Enemy_sc>();
            if (enemy != null) enemy.TakeDamage(damageAmount);
        }
    }
}