using UnityEngine;

[System.Serializable]
public class CardEffect
{
    public enum EffectType
    {
        HealPercent,
        BuffAttack,
        Sacrifice,
        SuperAttack
    }

    public EffectType type;
    public float value;
    public string description;

}

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/New Card")]
public class Card : ScriptableObject
{
    public string cardName;
    public Sprite icon;
    public CardEffect[] effects;
}
