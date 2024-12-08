using PJH.Core;
using UnityEngine;


namespace PJH.Scene
{
    public abstract class BaseScene : MonoBehaviour
    {
        public Define.EScene SceneType { get; protected set; } = Define.EScene.Unknown;


        protected virtual void Awake()
        {
        }
    }
}