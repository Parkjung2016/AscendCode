using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using PJH.Core;
using RayFire;
using TransitionsPlus;
using UnityEngine;
using UnityEngine.Playables;

namespace PJH.Scene
{
    using Managers = PJH.Manager.Managers;

    public class TitleScene : BaseScene
    {
        [SerializeField] private GameObject _gameTitleGroup;
        [SerializeField] private EventReference _musicEventReference;
        [SerializeField] private EventReference _titleExplosionEventReference;
        [SerializeField] private TransitionAnimator _transitionAnimator;

        private EventInstance _musicInstance;

        protected override void Awake()
        {
            PlayerPrefs.SetInt(Baek.Define.Define.EBattleUIKey.ShowDamageTextToggle.ToString(), 1);
            _transitionAnimator.SetProgress(1);
            _musicInstance = RuntimeManager.CreateInstance(_musicEventReference);
            _musicInstance.setParameterByNameWithLabel("Type", "Start");
            _musicInstance.start();
            SceneType = Define.EScene.Title;
            Managers.FMODSound.SetGameSoundVolume(1);
            _gameTitleGroup.transform.GetChild(0).transform.GetComponent<MeshRenderer>().enabled = true;
            _gameTitleGroup.transform.GetChild(1).gameObject.SetActive(false);
            StartLoadAssets();
        }

        private async void StartLoadAssets()
        {
            Managers.Addressable.LoadALlAsync<Object>("PreLoad", (key, count, totalCount) =>
            {
                if (count == totalCount)
                {
                    DOVirtual.Float(1, 0, .5f, x => _transitionAnimator.SetProgress(x));

                    Debug.Log("Addressable All Load Complete");
                    Managers.PopupText.Init();
                    Managers.Cursor.Init();
                    Managers.FMODSound.Init();
                }
            });
        }


        public bool CanClickUI()
        {
            return _transitionAnimator.progress <= .1f;
        }

        public void PlayStartTimeline()
        {
            _musicInstance.setParameterByNameWithLabel("Type", "End");
            RuntimeManager.PlayOneShot(_titleExplosionEventReference);
            _gameTitleGroup.transform.GetChild(0).transform.GetComponent<MeshRenderer>().enabled = false;
            _gameTitleGroup.transform.GetChild(1).gameObject.SetActive(true);
            FindAnyObjectByType<RayfireBomb>().Explode(0);
            FindAnyObjectByType<PlayableDirector>().Play();
        }

        public void GotoGameScene()
        {
            _musicInstance.release();
            Managers.Scene.LoadScene(Define.EScene.InGame);
        }
    }
}