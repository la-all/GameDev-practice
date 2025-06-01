using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    
    [Header("Health Bar UI")]
    public Image healthBarFill;
    public Gradient healthGradient;
    public float healthSmoothTime = 0.3f;
    
    private float smoothHealthVelocity;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI(true);
    }

    void Update()
    {
        UpdateHealthUI();
    }

    public void UpdateHealthUI(bool immediate = false)
    {
        if (healthBarFill == null) return;
        
        float targetFill = (float)currentHealth / maxHealth;
        
        if (immediate)
        {
            healthBarFill.fillAmount = targetFill;
        }
        else
        {
            healthBarFill.fillAmount = Mathf.SmoothDamp(
                healthBarFill.fillAmount, 
                targetFill, 
                ref smoothHealthVelocity, 
                healthSmoothTime);
        }
        
        healthBarFill.color = healthGradient.Evaluate(targetFill);
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        Debug.Log($"{name} takes {damage} damage!");
        
        StartCoroutine(DamageEffect());
        UpdateHealthUI();
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        UpdateHealthUI();
    }

    public void TakeDamagePercent(float percent)
    {
        int damage = Mathf.RoundToInt(maxHealth * percent / 100f);
        TakeDamage(damage);
    }

    public void HealPercent(float percent)
    {
        int healAmount = Mathf.RoundToInt(maxHealth * percent / 100f);
        Heal(healAmount);
    }

    private void Die()
    {
        Debug.Log($"{name} has been defeated!");
        // Логика смерти
    }

    private IEnumerator DamageEffect()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.2f);
        GetComponent<SpriteRenderer>().color = Color.white;
    }
}