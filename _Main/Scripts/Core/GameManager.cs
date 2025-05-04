using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Events;
using PJH.Agent.Player;
using PJH.Core;
using PJH.Manager;
using PJH.Scene;
using TransitionsPlus;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Game.Core
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO _gameEventChannel;
        [SerializeField] private TransitionAnimator _transitionAnimator;
        private Beautify.Universal.Beautify _beautify;

        private Sequence _rightBeforeThePlayerDeathSequence;

        private void Awake()
        {
            FindAnyObjectByType<Volume>().profile.TryGet(out _beautify);
            _gameEventChannel.AddListener<PlayerDead>(HandlePlayerDead);
            _gameEventChannel.AddListener<FadeOut>(HandleFadeOut);
            _gameEventChannel.AddListener<FadeIn>(HandleFadeIn);
            _gameEventChannel.AddListener<RightBeforeThePlayerDeath>(HandleRightBeforeThePlayerDeath);
        }

        private void Start()
        {
            _transitionAnimator.SetProgress(1);
        }

        private void HandleFadeIn(FadeIn evt)
        {
            _transitionAnimator.onTransitionEnd = new UnityEvent();
            _transitionAnimator.profile.duration = evt.duration;
            _transitionAnimator.Play();
            _transitionAnimator.onTransitionEnd.AddListener(() => evt.EndEvent?.Invoke());
        }

        private void HandleFadeOut(FadeOut evt)
        {
            _transitionAnimator.onTransitionEnd = null;

            DOVirtual.Float(1, 0, evt.duration, x => _transitionAnimator.SetProgress(x)).OnComplete(() =>
            {
                evt.EndEvent?.Invoke();
            });
        }

        private void HandleRightBeforeThePlayerDeath(RightBeforeThePlayerDeath evt)
        {
            if (_rightBeforeThePlayerDeathSequence != null && _rightBeforeThePlayerDeathSequence.IsActive())
                _rightBeforeThePlayerDeathSequence.Kill();
            Managers.FMODSound.SetBeforePlayerDead(evt.isRightBeforeThePlayerDeath);
            if (evt.isRightBeforeThePlayerDeath)
            {
                _rightBeforeThePlayerDeathSequence = DOTween.Sequence();
                Tween tween = DOTween.ToAlpha(() => _beautify.frameColor.value, x => _beautify.frameColor.value = x,
                    .5f,
                    1);
                _rightBeforeThePlayerDeathSequence.Append(tween);
                _rightBeforeThePlayerDeathSequence.AppendInterval(.1f);
                tween = DOTween.ToAlpha(() => _beautify.frameColor.value, x => _beautify.frameColor.value = x, 0,
                    1);
                _rightBeforeThePlayerDeathSequence.Append(tween);
                _rightBeforeThePlayerDeathSequence.SetLoops(-1, LoopType.Yoyo);
            }
            else
            {
                _rightBeforeThePlayerDeathSequence = DOTween.Sequence();
                Tween tween = DOTween.ToAlpha(() => _beautify.frameColor.value, x => _beautify.frameColor.value = x, 0,
                    .5f);
                _rightBeforeThePlayerDeathSequence.Append(tween);
            }
        }


        private void HandlePlayerDead(PlayerDead evt)
        {
            Time.timeScale = .2f;
            DOTween.To(() => _beautify.sepia.value, x => _beautify.sepia.value = x, 1, 2.5f);
            DOVirtual.DelayedCall(4f, () =>
            {
                var fadeInEvt = GameEvents.FadeIn;
                fadeInEvt.duration = 1.5f;
                fadeInEvt.EndEvent = () =>
                {
                    evt.FadeEndEvent?.Invoke();
                    (Managers.Scene.CurrentScene as GameScene).Player.PlayerInput.ResetControl();

                    DOVirtual.DelayedCall(3.5f, () =>
                    {
                        Time.timeScale = 1f;
                        SceneManager.LoadScene(Define.EScene.Title.ToString());
                    });
                };
                _gameEventChannel.RaiseEvent(fadeInEvt);
            });
        }

        private void OnDestroy()
        {
            _gameEventChannel.RemoveListener<PlayerDead>(HandlePlayerDead);
            _gameEventChannel.RemoveListener<FadeIn>(HandleFadeIn);
            _gameEventChannel.RemoveListener<FadeOut>(HandleFadeOut);
            _gameEventChannel.RemoveListener<RightBeforeThePlayerDeath>(HandleRightBeforeThePlayerDeath);

            GameEvents.FadeIn.EndEvent = null;
            GameEvents.FadeOut.EndEvent = null;
            GameEvents.PlayerDead.FadeEndEvent = null;
        }
    }
}