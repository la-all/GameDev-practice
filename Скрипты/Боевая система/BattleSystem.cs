using UnityEngine;
using UnityEngine.UI;

public class BattleSystem : MonoBehaviour
{
    public static BattleSystem Instance;

    [Header("Player References")]
    public PlayerController player1;
    public PlayerController player2;

    [Header("UI Elements")]
    public GameObject actionPanel;
    public Text turnText;
    public Text gameOverText;
    public Text turnCounterText;

    private PlayerController currentPlayer;
    private bool gameOver = false;
    private int turnCounter = 1;
    private const int maxTurns = 10;
    private CardSystem cardSystem;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        cardSystem = GetComponent<CardSystem>();
        cardSystem.OnCardSelectedEvent += ApplyCardEffects;
    }

    void Start()
    {
        currentPlayer = player1;
        UpdateTurnCounterUI();
        StartTurn();
    }

    void StartTurn()
    {
        if (gameOver) return;

        cardSystem.InitializeTurnCards();
        cardSystem.ShowCardSelection();
        
        actionPanel.SetActive(true);
        turnText.text = $"Ходит {currentPlayer.playerName}";
        currentPlayer.ResetActions();
        BattleUIController.Instance.OnPlayerChanged();
    }

    void ApplyCardEffects(Card selectedCard)
    {
        PlayerHealth playerHealth = currentPlayer.GetComponent<PlayerHealth>();
        PlayerAttackSystem playerAttack = currentPlayer.GetComponent<PlayerAttackSystem>();

        foreach (CardEffect effect in selectedCard.effects)
        {
            switch (effect.type)
            {
                case CardEffect.EffectType.HealPercent:
                    playerHealth.HealPercent(effect.value);
                    break;
                case CardEffect.EffectType.BuffAttack:
                    playerAttack.AddAttackModifier(effect.value);
                    break;
                case CardEffect.EffectType.Sacrifice:
                    playerHealth.TakeDamagePercent(effect.value);
                    playerAttack.AddAttackModifier(3f);
                    break;
                case CardEffect.EffectType.SuperAttack:
                    playerAttack.GainSuperMeter(100);
                    break;
            }
        }
        
        actionPanel.SetActive(true);
        BattleUIController.Instance.OnPlayerChanged();
    }

    public void EndTurn()
    {
        if (cardSystem.IsWaitingForCardSelection()) return;

        actionPanel.SetActive(false);
        currentPlayer.GetComponent<PlayerAttackSystem>().ResetModifiers();

        turnCounter++;
        UpdateTurnCounterUI();

        if (CheckGameOver() || CheckTurnLimit()) return;

        currentPlayer = (currentPlayer == player1) ? player2 : player1;
        StartTurn();
    }

    void UpdateTurnCounterUI()
    {
        if (turnCounterText != null)
            turnCounterText.text = $"Ход: {turnCounter}/{maxTurns}";
    }

    bool CheckGameOver()
    {
        PlayerHealth p1Health = player1.GetComponent<PlayerHealth>();
        PlayerHealth p2Health = player2.GetComponent<PlayerHealth>();

        if (p1Health.currentHealth <= 0)
        {
            gameOverText.text = $"Победил {player2.playerName}!";
            GameOver();
            return true;
        }

        if (p2Health.currentHealth <= 0)
        {
            gameOverText.text = $"Победил {player1.playerName}!";
            GameOver();
            return true;
        }

        return false;
    }

    bool CheckTurnLimit()
    {
        PlayerHealth p1Health = player1.GetComponent<PlayerHealth>();
        PlayerHealth p2Health = player2.GetComponent<PlayerHealth>();

        if (turnCounter > maxTurns)
        {
            if (p1Health.currentHealth > p2Health.currentHealth)
            {
                gameOverText.text = $"Победил {player1.playerName}!";
            }
            else if (p2Health.currentHealth > p1Health.currentHealth)
            {
                gameOverText.text = $"Победил {player2.playerName}!";
            }
            else
            {
                gameOverText.text = "Ничья";
            }

            GameOver();
            return true;
        }
        return false;
    }

    void GameOver()
    {
        gameOver = true;
        gameOverText.gameObject.SetActive(true);
        actionPanel.SetActive(false);
    }

    public PlayerController GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public PlayerController GetOpponentPlayer()
    {
        return (currentPlayer == player1) ? player2 : player1;
    }

    void Update()
    {
        if (currentPlayer != null)
        {
            bool canSuper = currentPlayer.GetComponent<PlayerAttackSystem>().CanSuperAttack();
            BattleUIController.Instance.superAttackBtn.interactable = canSuper;
            
            BattleUIController.Instance.superAttackBtn.image.color = 
                canSuper ? Color.yellow : Color.white;
        }
    }
}