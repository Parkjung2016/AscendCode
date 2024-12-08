using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using FIMSpace;
using FIMSpace.FLook;
using FIMSpace.FProceduralAnimation;
using PJH.Core;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace PJH.Agent.Player
{
    public class PlayerIK : MonoBehaviour, MInterface.IAgentComponent
    {
        public LegsAnimator LegsAnimatorCompo { get; private set; }
        public FLookAnimator LookAnimatorCompo { get; private set; }
        public LeaningAnimator LeaningAnimator { get; private set; }
        private TwoBoneIKConstraint _handRIK;
        private Transform _handRIKTargetTrm;
        private Player _player;

        public void Initialize(Agent agent)
        {
            _player = agent as Player;

            LegsAnimatorCompo = GetComponent<LegsAnimator>();
            LookAnimatorCompo = GetComponent<FLookAnimator>();
            LeaningAnimator = GetComponent<LeaningAnimator>();
            _handRIK = transform.Find("HandIK/HandRIK").GetComponent<TwoBoneIKConstraint>();
            EnableHandRIK(false, 0);
        }

        public void AfterInit()
        {
            var animatorCompo = _player.GetCompo<PlayerAnimator>();
            animatorCompo.EndEvasionEvent += HandleEndEvasionEvent;
            animatorCompo.GetHitEvent += HandleGetHitEvent;
            var movementCompo = _player.GetCompo<PlayerMovement>();
            movementCompo.EvasionEvent += HandleEvasionEvent;


            var attackCompo = _player.GetCompo<PlayerAttack>();
            attackCompo.AttackEvent += HandleAttackEvent;

            _player.PlayerInput.MovementEvent += HandleMovementEvent;
            _player.PlayerInput.ChargeAttackEvent += HandleChargeAttackEvent;
        }

        private void HandleGetHitEvent(bool isHitting)
        {
            if (isHitting)
            {
                LegsAnimatorCompo.User_FadeToDisabled(.1f);
                LookAnimatorCompo.SwitchLooking(false);
                LeaningAnimator.enabled = false;
            }
            else
            {
                LegsAnimatorCompo.User_FadeEnabled(.1f);
                LookAnimatorCompo.SwitchLooking(true);
                LeaningAnimator.enabled = true;
            }
        }


        private void HandleMovementEvent(Vector2 input)
        {
            float inputSqr = input.sqrMagnitude;
            bool enabled = inputSqr <= 0;
            if (enabled)
            {
                LegsAnimatorCompo.User_FadeEnabled(.1f);
            }
            else
                LegsAnimatorCompo.User_FadeToDisabled(.1f);
        }

        private void HandleChargeAttackEvent(bool isAttackHolding)
        {
            LookAnimatorCompo.SwitchLooking(false, .1f);
        }

        private void HandleAttackEvent(int comboCount)
        {
            LookAnimatorCompo.SwitchLooking(false, .1f);
        }

        private void HandleEvasionEvent()
        {
            LegsAnimatorCompo.User_FadeToDisabled(.1f);
            LookAnimatorCompo.SwitchLooking(false, .1f);
        }

        private void HandleEndEvasionEvent()
        {
            LegsAnimatorCompo.User_FadeEnabled(.1f);
            LookAnimatorCompo.SwitchLooking(true, .1f);
        }

        public void Dispose()
        {
            var animatorCompo = _player.GetCompo<PlayerAnimator>();
            animatorCompo.EndEvasionEvent -= HandleEndEvasionEvent;
            animatorCompo.GetHitEvent -= HandleGetHitEvent;
            var movementCompo = _player.GetCompo<PlayerMovement>();
            movementCompo.EvasionEvent -= HandleEvasionEvent;


            var attackCompo = _player.GetCompo<PlayerAttack>();
            attackCompo.AttackEvent -= HandleAttackEvent;

            _player.PlayerInput.MovementEvent -= HandleMovementEvent;
            _player.PlayerInput.ChargeAttackEvent -= HandleChargeAttackEvent;
        }

        private void LateUpdate()
        {
            if (!_handRIKTargetTrm) return;
            _handRIK.data.target.SetPositionAndRotation(_handRIKTargetTrm.position, _handRIKTargetTrm.rotation);
        }

        public void SetHandRIKTarget(Transform target)
        {
            _handRIKTargetTrm = target;
        }

        public void EnableHandRIK(bool enabled, float duration = .5f)
        {
            DOTween.To(() => _handRIK.weight, x => _handRIK.weight = x, Convert.ToByte(enabled), duration);
        }
    }
}