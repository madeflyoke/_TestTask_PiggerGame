using Pigger.GUI.Screens.EndGame;
using Pigger.GUI.Screens.GamePlay;
using Pigger.Managers;
using UnityEngine;
using Zenject;

namespace Pigger.GUI
{
    public class ScreensController : MonoBehaviour
    {
        [Inject] private GameManager gameManager;

        [SerializeField] private GamePlayScreen gamePlayScreen;
        [SerializeField] private EndGameScreen endGameScreen;

        private void Awake()
        {
            gamePlayScreen.Hide();
            endGameScreen.Hide();
        }

        private void Start()
        {
            gamePlayScreen.Show();
        }

        private void OnEnable()
        {
            gameManager.loseGameEvent += GameLoseLogic;
            gameManager.winGameEvent += GameWinLogic;
        }
        private void OnDisable()
        {
            gameManager.loseGameEvent += GameLoseLogic;
            gameManager.winGameEvent -= GameWinLogic;
        }

        private void GameLoseLogic()
        {
            gamePlayScreen.Hide();
            endGameScreen.Show();
            endGameScreen.SetLoseScreen();
        }

        private void GameWinLogic()
        {
            gamePlayScreen.Hide();
            endGameScreen.Show();
            endGameScreen.SetWinScreen();
        }
    }
}

