namespace UI
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using DG.Tweening;

    public class SlidingParchment : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        [Header("Animation")]
        public RectTransform parchment;
        public float slideDuration = 0.5f;
        public float visibleY = 0f;   // anchoredPosition.y when visible
        public float hiddenY = -200f; // anchoredPosition.y when hidden (peek offset)
        public float autoHideDelay = 3f;

        private Tween currentTween;
        private bool isVisible;

        private void Start()
        {
            parchment.anchoredPosition = new Vector2(parchment.anchoredPosition.x, hiddenY);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Show();
        }

        public void OnDrag(PointerEventData eventData)
        {
            parchment.anchoredPosition += new Vector2(0, eventData.delta.y);

            float y = Mathf.Clamp(parchment.anchoredPosition.y, hiddenY, visibleY);
            parchment.anchoredPosition = new Vector2(parchment.anchoredPosition.x, y);

            isVisible = (y > (visibleY + hiddenY) / 2);
        }

        private void Show()
        {
            isVisible = true;
            AnimateTo(visibleY);

            CancelInvoke(nameof(Hide));
            Invoke(nameof(Hide), autoHideDelay);
        }

        private void Hide()
        {
            if (!isVisible) return;
            isVisible = false;
            AnimateTo(hiddenY);
        }

        private void AnimateTo(float targetY)
        {
            currentTween?.Kill();
            currentTween = parchment.DOAnchorPosY(targetY, slideDuration).SetEase(Ease.OutCubic);
        }
    }

}