using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerAttackSystem))]
public class PlayerController : MonoBehaviour
{
    [Header("Player Stats")]
    public string playerName = "Player";

    [Header("Movement")]
    public float moveSpeed = 5f;
    public int moveDistance = 3;

    public int actionsRemaining = 2;
    private bool hasMovedThisTurn = false;
    private bool hasAttackedThisTurn = false;

    private PlayerAttackSystem attackSystem;

    void Awake()
    {
        attackSystem = GetComponent<PlayerAttackSystem>();
    }

    public void ResetActions()
    {
        actionsRemaining = 2;
        hasMovedThisTurn = false;
        hasAttackedThisTurn = false;
    }

    public void RegisterAttackPerformed()
    {
        hasAttackedThisTurn = true;
        actionsRemaining--;
    }

    public bool CanMove()
    {
        return actionsRemaining > 0 && !hasMovedThisTurn;
    }

    public bool CanAttack()
    {
        return actionsRemaining > 0 && !hasAttackedThisTurn;
    }

    public void Move(Vector3 direction)
    {
        if (!CanMove()) return;

        Vector3 targetPosition = transform.position + direction * moveDistance;

        StartCoroutine(MoveToPosition(targetPosition));
        hasMovedThisTurn = true;
        actionsRemaining--;
    }

    private IEnumerator MoveToPosition(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = target;
    }
}
