using UnityEngine;

namespace Pigger.GUI.Screens.EndGame
{
    public class EndGameScreen : BaseScreen
    {
        [SerializeField] private GameObject loseTitle;
        [SerializeField] private GameObject winTitle;

        private void Awake()
        {
            ResetScreen();
        }

        public void SetLoseScreen()
        {
            loseTitle.SetActive(true);
        }

        public void SetWinScreen()
        {
            winTitle.SetActive(true);
        }

        private void ResetScreen()
        {
            loseTitle.SetActive(false);
            winTitle.SetActive(false);
        }
    }
}

