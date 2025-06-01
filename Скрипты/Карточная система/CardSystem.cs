using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CardSystem : MonoBehaviour
{
    [Header("Card References")]
    public GameObject cardSelectionPanel;
    public GameObject cardPrefab;
    public List<Card> allCards;

    [Header("Card Settings")]
    public float cardFanSpread = 100f;
    public float cardYPosition = -400f;
    public float cardMaxAngle = 15f;
    public float cardAppearDelay = 0.15f;
    public float cardAppearDuration = 0.5f;

    private List<Card> currentTurnCards = new List<Card>();
    private bool waitingForCardSelection = false;

    public delegate void CardSelectedHandler(Card selectedCard);
    public event CardSelectedHandler OnCardSelectedEvent;

    public void InitializeTurnCards()
    {
        currentTurnCards.Clear();
        
        // Генерация 2 уникальных карт
        for (int i = 0; i < 2; i++)
        {
            Card randomCard = allCards[Random.Range(0, allCards.Count)];
            while (currentTurnCards.Contains(randomCard))
            {
                randomCard = allCards[Random.Range(0, allCards.Count)];
            }
            currentTurnCards.Add(randomCard);
        }
    }

    public void ShowCardSelection()
    {
        if (currentTurnCards.Count < 2)
        {
            Debug.LogError("Not enough cards generated!");
            return;
        }

        ClearCardPanel();
        StartCoroutine(AnimateCardsAppearance());
        waitingForCardSelection = true;
        cardSelectionPanel.SetActive(true);
    }

    private void ClearCardPanel()
    {
        foreach (Transform child in cardSelectionPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private IEnumerator AnimateCardsAppearance()
    {
        for (int i = 0; i < currentTurnCards.Count; i++)
        {
            yield return new WaitForSeconds(cardAppearDelay);
            CreateAndAnimateCard(i);
        }
    }

    private void CreateAndAnimateCard(int cardIndex)
    {
        GameObject cardObj = Instantiate(cardPrefab, cardSelectionPanel.transform);
        CardUI cardUI = cardObj.GetComponent<CardUI>();
        
        if (cardUI == null)
        {
            Debug.LogError("Card prefab missing CardUI component");
            return;
        }

        cardUI.Initialize(currentTurnCards[cardIndex], () => OnCardSelected(currentTurnCards[cardIndex]));

        // Настройка позиции и поворота
        float ratio = cardIndex / (float)(currentTurnCards.Count - 1);
        float xPos = Mathf.Lerp(-cardFanSpread, cardFanSpread, ratio);
        float angle = Mathf.Lerp(cardMaxAngle, -cardMaxAngle, ratio);
        
        Vector3 startPos = new Vector3(xPos, cardYPosition - 100f, 0);
        Vector3 endPos = new Vector3(xPos, cardYPosition, 0);
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        StartCoroutine(AnimateCard(cardObj.transform, startPos, endPos, rotation));
    }

    private IEnumerator AnimateCard(Transform cardTransform, Vector3 startPos, Vector3 endPos, Quaternion rotation)
    {
        cardTransform.localPosition = startPos;
        cardTransform.localRotation = rotation;
        cardTransform.localScale = Vector3.zero;

        float elapsed = 0f;
        while (elapsed < cardAppearDuration)
        {
            float progress = elapsed / cardAppearDuration;
            
            // Плавные анимации
            float scaleProgress = Mathf.Sin(progress * Mathf.PI * 0.5f);
            float moveProgress = 1f - Mathf.Pow(1f - progress, 3);
            
            cardTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, scaleProgress);
            cardTransform.localPosition = Vector3.Lerp(startPos, endPos, moveProgress);
            
            elapsed += Time.deltaTime;
            yield return null;
        }

        cardTransform.localScale = Vector3.one;
        cardTransform.localPosition = endPos;
    }

    private void OnCardSelected(Card selectedCard)
    {
        cardSelectionPanel.SetActive(false);
        waitingForCardSelection = false;
        OnCardSelectedEvent?.Invoke(selectedCard);
    }

    public bool IsWaitingForCardSelection()
    {
        return waitingForCardSelection;
    }
}