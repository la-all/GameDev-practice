using UnityEngine;
using UnityEngine.UI;

public class BattleUIController : MonoBehaviour
{
    public static BattleUIController Instance;

    [Header("Action Buttons")]
    public Button superAttackBtn;
    public Button moveUpBtn;
    public Button moveDownBtn;
    public Button moveLeftBtn;
    public Button moveRightBtn;
    public Button attackBtn_1; // Melee
    public Button attackBtn_2; // Ranged
    public Button attackBtn_3; // Spell
    public Button endTurnBtn;

    private PlayerAttackSystem currentPlayerAttack;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Назначаем обработчики кнопок
        moveUpBtn.onClick.AddListener(() => MovePlayer(Vector3.up));
        moveDownBtn.onClick.AddListener(() => MovePlayer(Vector3.down));
        moveLeftBtn.onClick.AddListener(() => MovePlayer(Vector3.left));
        moveRightBtn.onClick.AddListener(() => MovePlayer(Vector3.right));

        attackBtn_1.onClick.AddListener(MeleeAttack);
        attackBtn_2.onClick.AddListener(RangedAttack);
        attackBtn_3.onClick.AddListener(SpellAttack);
        superAttackBtn.onClick.AddListener(SuperAttack);
        endTurnBtn.onClick.AddListener(EndTurn);
    }

    void Update()
    {
        UpdateButtonsInteractivity();
    }

    private void UpdateButtonsInteractivity()
    {
        PlayerController player = BattleSystem.Instance?.GetCurrentPlayer();
        if (player == null) return;

        currentPlayerAttack = player.GetComponent<PlayerAttackSystem>();
        bool canAttack = player.CanAttack();

        attackBtn_1.interactable = canAttack;
        attackBtn_2.interactable = canAttack;
        attackBtn_3.interactable = canAttack;
        superAttackBtn.interactable = currentPlayerAttack?.CanSuperAttack() ?? false;

        bool canMove = player.CanMove();
        moveUpBtn.interactable = canMove;
        moveDownBtn.interactable = canMove;
        moveLeftBtn.interactable = canMove;
        moveRightBtn.interactable = canMove;
    }

    public void MovePlayer(Vector3 direction)
    {
        PlayerController player = BattleSystem.Instance?.GetCurrentPlayer();
        player?.Move(direction);
    }

    public void MeleeAttack()
    {
        if (currentPlayerAttack != null)
            currentPlayerAttack.MeleeAttack();
    }

    public void RangedAttack()
    {
        if (currentPlayerAttack != null)
            currentPlayerAttack.RangedAttack();
    }

    public void SpellAttack()
    {
        if (currentPlayerAttack != null)
            currentPlayerAttack.SpellAttack();
    }

    public void SuperAttack()
    {
        if (currentPlayerAttack != null)
            currentPlayerAttack.SuperAttack();
    }

    public void EndTurn()
    {
        BattleSystem.Instance?.EndTurn();
    }

    // Метод для обновления UI при смене игрока
    public void OnPlayerChanged()
    {
        UpdateButtonsInteractivity();
    }
}