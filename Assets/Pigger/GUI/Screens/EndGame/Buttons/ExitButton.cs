using UnityEngine;

namespace Pigger.GUI.Screens.EndGame.Buttons
{
    public class ExitButton : BaseButton
    {
        protected override void Listeners()
        {
            base.Listeners();
            Application.Quit();
        }
    }
}

