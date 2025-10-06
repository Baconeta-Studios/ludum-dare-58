using DG.Tweening;
using FMODUnity;
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
        // Play card interaction sound with FMOD.
        RuntimeManager.PlayOneShot("event:/SFX/Cards/CardSelection");
        
        var handVisual = handSlot.CardVisual;
        var leftVisual = clickedSlot.CardVisual;

        Vector3 handTarget = clickedSlot.transform.position;
        Vector3 leftTarget = handSlot.transform.position;

        var seq = DOTween.Sequence();

        // Animate visuals to each other’s positions
        seq.Join(handVisual.DOMove(handTarget, 0.25f).SetEase(Ease.InOutQuad));
        seq.Join(leftVisual.DOMove(leftTarget, 0.25f).SetEase(Ease.InOutQuad));

        seq.OnComplete(() => {
            // Swap card data AFTER animation finishes
            var temp = handSlot.Card;
            handSlot.SetCard(clickedSlot.Card, OnHandChanged);
            clickedSlot.SetCard(temp, OnLeftCardClicked);

            // Snap visuals back to their slots
            handSlot.RefreshVisual();
            clickedSlot.RefreshVisual();

            // Update player
            _player.SetInteraction(handSlot.Card.interactionType);
        });
    }
    
    private void OnHandChanged(CardSlot slot) {
        _player.currentInteractionType = slot.Card.interactionType;
    }
}