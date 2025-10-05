using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CardSlot : MonoBehaviour {
    [Header("Visuals")]
    [SerializeField] private RectTransform cardVisual;
    [SerializeField] private Image cardImage;
    [SerializeField] private TMP_Text cardLabel;
    [SerializeField] private Button button;

    public CardData Card { get; private set; }
    public RectTransform CardVisual => cardVisual;       // expose for tweens

    private Action<CardSlot> _onClick;

    public void SetCard(CardData data, Action<CardSlot> clickCallback) {
        Card = data;
        _onClick = clickCallback;

        if (cardImage != null) cardImage.sprite = data.cardSprite;
        if (cardLabel != null) cardLabel.text = data.interactionType.ToString();

        if (button != null) {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => _onClick?.Invoke(this));
        }
    }

    public void RefreshVisual() {
        if (cardVisual != null) {
            // snap the visual back into place
            cardVisual.SetParent(transform, false);
            cardVisual.localPosition = Vector3.zero;
            cardVisual.localRotation = Quaternion.identity;
            cardVisual.localScale = Vector3.one;
        }
    }
}