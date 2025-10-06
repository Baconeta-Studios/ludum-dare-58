using Managers;
using UnityEngine;

namespace UI
{
    public class EndGameUI : MonoBehaviour
    {
        [SerializeField] private GameObject endScreen;

        private void OnEnable()
        {
            GameManager.OnGameStateChanged += HandleStateChanged;
        }

        private void OnDisable()
        {
            GameManager.OnGameStateChanged -= HandleStateChanged;
        }

        private void HandleStateChanged(GameState state, string s)
        {
            if (state == GameState.Ended)
            {
                endScreen.SetActive(true);
            }
            else
            {
                endScreen.SetActive(false);
            }
        }
    }

}