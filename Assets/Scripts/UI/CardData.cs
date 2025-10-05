using Movement;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "CardData", menuName = "Cards/New Card")]
public class CardData : ScriptableObject
{
    public PlayerClickInteraction.InteractionType interactionType;
    public Sprite cardSprite;
}