using Cysharp.Threading.Tasks;
using Pigger.GamePlay.Points;
using Pigger.GamePlay.Units.MainCharacter;
using Pigger.GUI;
using Pigger.GUI.Screens.EndGame.Buttons;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Pigger.Managers
{
    public class GameManager : MonoBehaviour
    {
        [Inject] private PlayerController player;
        [Inject] private PointsController pointsController;

        public event Action loseGameEvent;
        public event Action winGameEvent;

        [SerializeField] private int targetFps;

        private void Awake()
        {
            Application.targetFrameRate = targetFps;
        }

        private void OnEnable()
        {
            player.playerDiedEvent += LoseGame;
            pointsController.allPointsEvent += WinGame;
        }
        private void OnDisable()
        {
            player.playerDiedEvent -= LoseGame;
            pointsController.allPointsEvent -= WinGame;
        }

        private void WinGame()
        {
            winGameEvent?.Invoke();
        }

        private void LoseGame()
        {
            loseGameEvent?.Invoke();
        }

        private async void RetryGame()
        {
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;
            AsyncOperation load = SceneManager.LoadSceneAsync(sceneIndex);
            while (load.isDone == false)
            {
                await UniTask.Yield();
            }                                          
        }

        public void ButtonsLogicHandler(BaseButton button)
        {
            if (button.GetType()==typeof(RetryButton))
            {
                RetryGame();
            }
        }
    }
}

