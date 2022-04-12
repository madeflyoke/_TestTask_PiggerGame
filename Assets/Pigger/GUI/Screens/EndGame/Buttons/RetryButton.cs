using Pigger.Managers;
using UnityEngine;
using Zenject;

namespace Pigger.GUI.Screens.EndGame.Buttons
{
    public class RetryButton : BaseButton
    {
        [Inject] private GameManager gameManager;

        protected override void Listeners()
        {
            base.Listeners();
            gameManager.ButtonsLogicHandler(this);
        }
    }

}
