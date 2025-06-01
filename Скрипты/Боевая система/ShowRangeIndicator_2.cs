using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Button))]
public class AttackRangeVisualizer_2 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Main Settings")]
    public float rangeDisplayDuration = 0.2f;
    public Color validRangeColor = new Color(0, 1, 0, 0.3f);
    public Color invalidRangeColor = new Color(1, 0, 0, 0.3f);

    [Header("References")]
    public BattleSystem battleSystem;
    public GameObject rangeIndicatorPrefab;
    public AttackType attackType; // Перечисление для типа атаки

    private GameObject currentIndicator;
    private SpriteRenderer rangeRenderer;
    private bool isShowing = false;
    private PlayerAttackSystem playerAttackSystem;

    public enum AttackType
    {
        Melee,
        Ranged,
        Spell
    }

    void Start()
    {
        currentIndicator = Instantiate(rangeIndicatorPrefab);
        rangeRenderer = currentIndicator.GetComponent<SpriteRenderer>();
        currentIndicator.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayerController currentPlayer = battleSystem.GetCurrentPlayer();
        if (currentPlayer != null)
        {
            playerAttackSystem = currentPlayer.GetComponent<PlayerAttackSystem>();
            ShowRangeIndicator();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideRangeIndicator();
    }

    void Update()
    {
        if (isShowing && battleSystem.GetCurrentPlayer() != null)
        {
            UpdateIndicatorPosition();
            CheckEnemiesInRange();
        }
    }

    private void ShowRangeIndicator()
    {
        if (playerAttackSystem == null || !playerAttackSystem.GetComponent<PlayerController>().CanAttack()) 
            return;

        isShowing = true;
        currentIndicator.SetActive(true);
        UpdateIndicatorPosition();
        
        rangeRenderer.color = new Color(
            rangeRenderer.color.r,
            rangeRenderer.color.g,
            rangeRenderer.color.b,
            0);
        
        StartCoroutine(FadeIndicator(0, 1, rangeDisplayDuration));
    }

    private void HideRangeIndicator()
    {
        if (!isShowing) return;
        
        StartCoroutine(FadeAndHide(1, 0, rangeDisplayDuration));
    }

    private void UpdateIndicatorPosition()
    {
        PlayerController player = battleSystem.GetCurrentPlayer();
        if (player != null)
        {
            currentIndicator.transform.position = player.transform.position;

            // Устанавливаем размер индикатора в зависимости от радиуса атаки
            float attackRange = GetCurrentAttackRange();
            currentIndicator.transform.localScale = Vector3.one;
        }
    }

    private void CheckEnemiesInRange()
    {
        PlayerController player = battleSystem.GetCurrentPlayer();
        PlayerController enemy = battleSystem.GetOpponentPlayer();

        if (player == null || enemy == null || playerAttackSystem == null) return;

        float distance = Vector3.Distance(
            player.transform.position,
            enemy.transform.position);

        float currentRange = GetCurrentAttackRange();
        rangeRenderer.color = distance <= currentRange
            ? validRangeColor
            : invalidRangeColor;
    }

    private float GetCurrentAttackRange()
    {
        if (playerAttackSystem == null) return 0f;

        switch (attackType)
        {
            case AttackType.Melee:
                return playerAttackSystem.attackRange_MeleeAttack;
            case AttackType.Ranged:
                return playerAttackSystem.attackRange_RangedAttack;
            case AttackType.Spell:
                return playerAttackSystem.attackRange_SpellAttack;
            default:
                return 0f;
        }
    }

    private IEnumerator FadeIndicator(float from, float to, float duration)
    {
        float elapsed = 0f;
        Color color = rangeRenderer.color;
        
        while (elapsed < duration)
        {
            color.a = Mathf.Lerp(from, to, elapsed / duration);
            rangeRenderer.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        color.a = to;
        rangeRenderer.color = color;
    }

    private IEnumerator FadeAndHide(float from, float to, float duration)
    {
        yield return StartCoroutine(FadeIndicator(from, to, duration));
        currentIndicator.SetActive(false);
        isShowing = false;
    }

    void OnDestroy()
    {
        if (currentIndicator != null)
            Destroy(currentIndicator);
    }
}