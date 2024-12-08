using System;
using System.Collections;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Events;
using PJH.Core;
using UnityEngine;


namespace PJH.Agent.Player
{
    public class PlayerInteract : MonoBehaviour, MInterface.IAgentComponent
    {
        [SerializeField] private float _detectRange;
        [SerializeField] private float _detectRate;
        [SerializeField] private GameEventChannelSO _interactObjectInfoEventChannel;
        private Collider[] _detectInteractables;

        private Player _player;

        private MInterface.IInteractable _interactableTarget;

        public void Initialize(Agent agent)
        {
            _player = agent as Player;
            _detectInteractables = new Collider[5];
        }

        public void AfterInit()
        {
            _player.PlayerInput.InteractEvent += HandleInteractEvent;
        }

        private void HandleInteractEvent()
        {
            if (_interactableTarget != null)
            {
                _interactableTarget.Interact();
            }
        }

        public void Dispose()
        {
            _player.PlayerInput.InteractEvent -= HandleInteractEvent;
        }

        private IEnumerator Start()
        {
            while (true)
            {
                DetectInteractObject();
                yield return YieldCache.WaitForSeconds(_detectRate);
            }
        }


        private void DetectInteractObject()
        {
            var arrayCopy = _detectInteractables.ToArray();
            int cnt = Physics.OverlapSphereNonAlloc(transform.position, _detectRange, arrayCopy,
                Define.MLayerMask.WhatIsInteractable);
            if (cnt > 0)
            {
                Array.Resize(ref arrayCopy, cnt);
                var nearInteractableObject = arrayCopy
                    .OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).First();
                if (nearInteractableObject.TryGetComponent(out MInterface.IInteractable interactable))
                {
                    if (_interactableTarget != interactable)
                    {
                        _interactableTarget = interactable;
                        var evt = GameEvents.InteractObjectInfo;
                        evt.targetInteractInfo = _interactableTarget.InteractInfo;
                        evt.targetTrm = nearInteractableObject.transform;
                        _interactObjectInfoEventChannel.RaiseEvent(evt);
                    }
                }
            }
            else
            {
                if (_interactableTarget != null)
                {
                    _interactableTarget = null;
                    var evt = GameEvents.InteractObjectInfo;
                    evt.targetTrm = null;
                    evt.targetInteractInfo = default;
                    _interactObjectInfoEventChannel.RaiseEvent(evt);
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _detectRange);
            Gizmos.color = Color.white;
        }
#endif
    }
}