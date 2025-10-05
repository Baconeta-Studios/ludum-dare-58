using System.Collections.Generic;
using Movement;
using UnityEngine;

[System.Serializable] [CreateAssetMenu(fileName = "CardData", menuName = "Cards/New Card")]
public class CardData : ScriptableObject {
    public PlayerClickInteraction.InteractionType interactionType;
    public Sprite cardSprite;
}

public class CardUIManager : MonoBehaviour {
    public CardSlot[] leftSlots;
    public CardSlot handSlot;
    private PlayerClickInteraction _player;

    public void Init(PlayerClickInteraction p, List<CardData> allCards) {
        _player = p;
        handSlot.SetCard(allCards[0], OnHandChanged);
        for (int i = 1; i < allCards.Count; i++) {
            leftSlots[i-1].SetCard(allCards[i], OnLeftCardClicked);
        }
    }

    private void OnLeftCardClicked(CardSlot clickedSlot) {
        var temp = handSlot.Card;
        handSlot.SetCard(clickedSlot.Card, OnHandChanged);
        clickedSlot.SetCard(temp, OnLeftCardClicked);
        _player.currentInteractionType = handSlot.Card.interactionType;
    }

    private void OnHandChanged(CardSlot slot) {
        _player.currentInteractionType = slot.Card.interactionType;
    }
}