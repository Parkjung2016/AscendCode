using Michsky.LSS;
using PJH.Core;
using PJH.Scene;
using UnityEngine;

namespace PJH.Manager
{
    public class SceneManagerEx
    {
        private BaseScene _currentScene;

        public BaseScene CurrentScene
        {
            get
            {
                if (_currentScene == null)
                    _currentScene = Object.FindAnyObjectByType<BaseScene>();
                return _currentScene;
            }
        }

        private LSS_Manager _lssManager;

        public LSS_Manager LssManager
        {
            get
            {
                if (_lssManager == null)
                {
                    _lssManager = Managers.Addressable.Instantiate<LSS_Manager>("LSS Manager");
                }

                return _lssManager;
            }
        }

        public void LoadScene(Define.EScene type)
        {
            LssManager.LoadScene(type.ToString());
        }
    }
}