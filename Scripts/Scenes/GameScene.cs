using Baek.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using Game.Events;
using PJH.Core;
using PJH.Manager;
using PJH.UI;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.Timeline;
using static PJH.Core.Define;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace PJH.Scene
{
    using Managers = PJH.Manager.Managers;

    public class GameScene : BaseScene
    {
        [SerializeField] private GameEventChannelSO _gameEventChannel;
        [SerializeField] private EventReference _musicEventReference;
        [SerializeField] private PopupTypeSO _bossHealthPopupType;

        [FormerlySerializedAs("_gameEndingTimelineAsset")] [SerializeField]
        private TimelineAsset _gameEndingPlayableAsset;

        private PlayableDirector _playableDirector;
        private PoolManagerSO _poolManager;
        public Agent.Player.Player Player { get; private set; }

        private EventInstance _musicEventInstance;

        protected override void Awake()
        {
            _playableDirector = FindAnyObjectByType<PlayableDirector>();
            SceneType = EScene.InGame_PJH;
            Player = FindAnyObjectByType<Agent.Player.Player>();
            Player.PlayerInput.EnablePlayerInput(false);
            Player.PlayerInput.EnableUIInput(false);
            _poolManager = Managers.Addressable.Load<PoolManagerSO>("PoolManager");
            _musicEventInstance = RuntimeManager.CreateInstance(_musicEventReference);
            EnemySpawnManager.Instance.EnemyAllDeadEvent += ClearStage;
            _poolManager.CompletedInitEvent += HandleCompletedInitEvent;
        }

        private void Start()
        {
            Player.HealthCompo.DeathEvent += HandleDeathEvent;
            WaveManager.Instance.StartNextStageEvent += HandleNextStageEvent;
            WaveManager.Instance.BossStageEvent += SetMusicEventParameter;
        }

        private async void HandleDeathEvent()
        {
            _musicEventInstance.setParameterByName("IsDeath", 1);
            _musicEventInstance.getParameterByName("IsDeath", out float value);
            await UniTask.WaitForSeconds(1f);
            Managers.FMODSound.SetGameSoundVolume(0, 1);
            await UniTask.WaitForSeconds(1f);
            _musicEventInstance.stop(STOP_MODE.IMMEDIATE);
            _musicEventInstance.release();
        }

        private void HandleNextStageEvent(int stage)
        {
            Baek.Manager.Managers.UI.ClosePopup(_bossHealthPopupType);
            SetMusicEventParameter(stage);
        }

        private void SetMusicEventParameter(int stage)
        {
            Debug.Log(stage);
            _musicEventInstance.setParameterByName("IsInStore", 0);
            _musicEventInstance.setParameterByName("Stage", stage);
        }

        private void HandleCompletedInitEvent()
        {
            var evt = GameEvents.FadeOut;
            evt.duration = .5f;

            evt.EndEvent = WaveManager.Instance.InitStage;
            evt.EndEvent += () =>
            {
                Debug.Log(3);
                DOVirtual.DelayedCall(1f, () =>
                {
                    Player.PlayerInput.EnableUIInput(true);
                    Player.PlayerInput.EnablePlayerInput(true);
                });
                _musicEventInstance.start();
            };
            _gameEventChannel.RaiseEvent(evt);
        }

        public void PauseGame()
        {
            Managers.FMODSound.PauseMainSound();
            Player.PlayerInput.EnablePlayerInput(false);
            Time.timeScale = 0;
        }

        public void ResumeGame()
        {
            Managers.FMODSound.ResumeMainSound();
            Player.PlayerInput.EnablePlayerInput(true);
            Time.timeScale = 1;
        }

        private void OnDestroy()
        {
            _poolManager.CompletedInitEvent -= HandleCompletedInitEvent;
            Player.HealthCompo.DeathEvent -= HandleDeathEvent;
            WaveManager waveManager = WaveManager.Instance;
            if (waveManager)
            {
                waveManager.StartNextStageEvent -= HandleNextStageEvent;
                waveManager.BossStageEvent -= SetMusicEventParameter;
            }

            EnemySpawnManager spawnManager = EnemySpawnManager.Instance;
            if (!spawnManager) return;
            spawnManager.EnemyAllDeadEvent -= ClearStage;
        }

        private async void ClearStage()
        {
            int currentStage = WaveManager.Instance.GetCurrentStage();
            bool isGameClear = currentStage >= EnemySpawnManager.Instance.EnemySpawnDataList.spawnDataList.Count - 1;
            if (!isGameClear)
            {
                var evt = GameEvents.ClearStage;
                _musicEventInstance.setParameterByName("IsInStore", 1);
                _gameEventChannel.RaiseEvent(evt);
            }
            else
            {
                Player.PlayerInput.EnablePlayerInput(false);
                Player.PlayerInput.EnableUIInput(false);
                Managers.FMODSound.SetGameSoundVolume(0, 1f);
                await UniTask.WaitForSeconds(2);
                FadeUI.FadeIn(2, async () =>
                {
                    await UniTask.WaitForSeconds(1.5f);
                    Player.gameObject.SetActive(false);
                    Player.PlayerInput.ResetControl();

                    _playableDirector.playableAsset = _gameEndingPlayableAsset;
                    _playableDirector.Play();
                });
                await UniTask.WaitForSeconds(.4f);
                _musicEventInstance.stop(STOP_MODE.IMMEDIATE);
                _musicEventInstance.release();
            }
        }

        public async void GameEnding()
        {
            Managers.FMODSound.SetTimelineSoundVolume(0, 1);
            await UniTask.WaitForSeconds(4f);
            FadeUI.Instance.ShowThankYou();
            await UniTask.WaitForSeconds(4f);
            FadeUI.Instance.HideThankYou();
            await UniTask.WaitForSeconds(2.5f);
            SceneManager.LoadScene(Define.EScene.Title.ToString());
        }
    }
}