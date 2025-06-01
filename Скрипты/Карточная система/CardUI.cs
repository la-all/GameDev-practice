using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    public Image cardImage;
    public Text cardNameText;
    public Text cardDescriptionText;

    public void Initialize(Card card, System.Action onClickCallback)
    {
        cardImage.sprite = card.icon;
        cardNameText.text = card.cardName;

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (CardEffect effect in card.effects)
        {
            sb.AppendLine(effect.description);
        }
        cardDescriptionText.text = sb.ToString();

        GetComponent<Button>().onClick.AddListener(() => onClickCallback());
    }
}