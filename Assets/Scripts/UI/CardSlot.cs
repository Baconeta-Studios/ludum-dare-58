using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CardSlot : MonoBehaviour {
    [SerializeField] private Image cardImage;
    [SerializeField] private Button button;

    public CardData Card { get; private set; }

    private Action<CardSlot> _onClick;

    public void SetCard(CardData data, Action<CardSlot> clickCallback) {
        Card = data;
        _onClick = clickCallback;

        if (cardImage != null) cardImage.sprite = data.cardSprite;

        if (button == null)
        {
            return;
        }
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => _onClick?.Invoke(this));
    }
}