using DG.Tweening;
using Movement;
using UnityEngine;

public class CardUIManager : MonoBehaviour {
    public CardSlot[] leftSlots;
    public CardSlot handSlot;
    private PlayerClickInteraction _player;
    
    [Header("Available Cards")]
    public CardData[] allCards;

    public void Init(PlayerClickInteraction p) {
        _player = p;

        handSlot.SetCard(allCards[0], OnHandChanged);

        for (var i = 1; i < allCards.Length; i++) {
            leftSlots[i - 1].SetCard(allCards[i], OnLeftCardClicked);
        }

        _player.SetInteraction(handSlot.Card.interactionType);
    }

    private void OnLeftCardClicked(CardSlot clickedSlot) {
        var handPos = handSlot.transform.position;
        var clickedPos = clickedSlot.transform.position;

        // Animate them to each other’s positions
        handSlot.transform.DOMove(clickedPos, 0.2f).OnComplete(() => {
            var temp = handSlot.Card;
            handSlot.SetCard(clickedSlot.Card, OnHandChanged);
            clickedSlot.SetCard(temp, OnLeftCardClicked);
        });
        clickedSlot.transform.DOMove(handPos, 0.2f);
    }
    
    public void AnimateMoveTo(Vector3 targetPosition, float duration = 0.2f) {
        transform.DOMove(targetPosition, duration).SetEase(Ease.InOutQuad);
    }
    
    private void OnHandChanged(CardSlot slot) {
        _player.currentInteractionType = slot.Card.interactionType;
    }
}