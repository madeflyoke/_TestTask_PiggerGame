using Pigger.GamePlay.Units.MainCharacter;
using UnityEngine;
using Zenject;
using DG.Tweening;
using UnityEngine.UI;

namespace Pigger.GUI.Screens.GamePlay
{
    public class BombButton : BaseButton
    {
        [Inject] private PlayerController player;
        private Image bombImage;
        private Sequence sequence;

        protected override void Awake()
        {
            base.Awake();
            bombImage = GetComponent<Image>();
            sequence = DOTween.Sequence();
        }

        protected override void Listeners()
        {
            base.Listeners();
            BombLogic();
        }

        private void BombLogic()
        {
            sequence.Kill();
            sequence = DOTween.Sequence();
            button.enabled = false;
            player.SetBomb();
            bombImage.fillAmount = 0;          
            sequence.Append(bombImage.DOFillAmount(1, player.BombCooldown).SetEase(Ease.Linear))
                .Append(bombImage.transform.DOPunchScale(Vector3.one * 0.1f, 0.3f, elasticity:0.3f)
                .OnComplete(() => button.enabled = true));
        }
    }
}

