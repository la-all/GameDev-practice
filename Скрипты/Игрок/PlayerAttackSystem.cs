using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerController))]
public class PlayerAttackSystem : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange_MeleeAttack = 2f;
    public float attackRange_RangedAttack = 5f;
    public float attackRange_SpellAttack = 4f;
    public int attackDamage = 10;


    [Header("Super Attack")]
    public int superAttackDamage = 25;
    public int superMeterMax = 100;
    public int superMeter = 0;
    public int superGainPerHit = 25;
    public Image superMeterUI;
    
    private float attackModifier = 0f;
    private PlayerController playerController;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    public void AddAttackModifier(float value)
    {
        attackModifier += value;
    }

    public void ResetModifiers()
    {
        attackModifier = 0f;
    }

    public void MeleeAttack()
    {
        if (!playerController.CanAttack()) return;

        PlayerController opponent = BattleSystem.Instance.GetOpponentPlayer();
        float distance = Vector3.Distance(transform.position, opponent.transform.position);
        
        if (distance <= attackRange_MeleeAttack)
        {
            
                int actualDamage = Mathf.RoundToInt(attackDamage * (1 + attackModifier));
                opponent.GetComponent<PlayerHealth>().TakeDamage(actualDamage);
                playerController.RegisterAttackPerformed();
                GainSuperMeter(superGainPerHit);
        }
        else
        {
            playerController.RegisterAttackPerformed();
            Debug.Log("Too far!");
        }
    }

    public void RangedAttack()
    {
        if (!playerController.CanAttack()) return;

        PlayerController opponent = BattleSystem.Instance.GetOpponentPlayer();
        float distance = Vector3.Distance(transform.position, opponent.transform.position);
        
        if (distance <= attackRange_RangedAttack)
        {
            
                int actualDamage = Mathf.RoundToInt(attackDamage * (1 + attackModifier));
                opponent.GetComponent<PlayerHealth>().TakeDamage(actualDamage);
                playerController.RegisterAttackPerformed();
                GainSuperMeter(superGainPerHit);
        }
        else
        {
            playerController.RegisterAttackPerformed();
            Debug.Log("Too far!");
        }
    }

    public void SpellAttack()
    {
        if (!playerController.CanAttack()) return;

        PlayerController opponent = BattleSystem.Instance.GetOpponentPlayer();
        float distance = Vector3.Distance(transform.position, opponent.transform.position);
        
        if (distance <= attackRange_SpellAttack)
        {
            int actualDamage = Mathf.RoundToInt(attackDamage * (1 + attackModifier));
            opponent.GetComponent<PlayerHealth>().TakeDamage(actualDamage);
            playerController.RegisterAttackPerformed();
            GainSuperMeter(superGainPerHit);
        }
        else
        {
            playerController.RegisterAttackPerformed();
            Debug.Log("Target is too far!");
        }
    }

    public void SuperAttack()
    {
        if (!CanSuperAttack()) return;

        //СуперАтака всегда проходит вне зависимости от расстояния
            PlayerController opponent = BattleSystem.Instance.GetOpponentPlayer();
            opponent.GetComponent<PlayerHealth>().TakeDamage(superAttackDamage);
            superMeter = 0;
            UpdateSuperMeterUI();
            playerController.RegisterAttackPerformed();

    }

    public void GainSuperMeter(int amount)
    {
        superMeter = superMeter + amount;
        UpdateSuperMeterUI();
    }

    void UpdateSuperMeterUI()
    {
        if (superMeterUI != null)
        {
            float targetFill = (float)superMeter / superMeterMax;
            superMeterUI.fillAmount = Mathf.Lerp(superMeterUI.fillAmount, targetFill, Time.deltaTime * 1000f);
            
            superMeterUI.color = superMeter >= superMeterMax ? Color.yellow : Color.blue;
        }
    }

    public bool CanSuperAttack()
    {
        return playerController.CanAttack() && superMeter >= superMeterMax;
    }

    void OnDrawGizmosSelected()
    {
        // Рисуем радиусы атак
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange_MeleeAttack);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange_RangedAttack);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange_SpellAttack);
    }
}