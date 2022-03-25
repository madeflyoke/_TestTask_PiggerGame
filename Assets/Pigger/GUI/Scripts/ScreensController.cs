using Pigger.GUI.Screens.EndGame;
using Pigger.Managers;
using UnityEngine;
using Zenject;

namespace Pigger.GUI
{
    public class ScreensController : MonoBehaviour
    {
        [Inject] private GameManager gameManager;

        [SerializeField] private EndGameScreen endGameScreen;
        private void Awake()
        {
            endGameScreen.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            gameManager.loseGameEvent += GameLoseLogic;
        }
        private void OnDisable()
        {
            gameManager.loseGameEvent += GameLoseLogic;
        }

        private void GameLoseLogic()
        {
            endGameScreen.gameObject.SetActive(true);
        }

    }
}

