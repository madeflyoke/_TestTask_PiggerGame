using Cysharp.Threading.Tasks;
using Pigger.GamePlay.Units.MainCharacter;
using Pigger.GUI;
using Pigger.GUI.Screens.EndGame.Buttons;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pigger.Managers
{
    public class GameManager : MonoBehaviour
    {
        public event Action endGameEvent;

        [SerializeField] private int targetFps;
        private PlayerController player;

        private void Awake()
        {
            Application.targetFrameRate = targetFps;
            player = FindObjectOfType<PlayerController>();
        }

        private void OnEnable()
        {
            player.playerDiedEvent += EndGame;
        }
        private void OnDisable()
        {
            player.playerDiedEvent -= EndGame;
        }

        private void EndGame()
        {
            endGameEvent?.Invoke();
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

