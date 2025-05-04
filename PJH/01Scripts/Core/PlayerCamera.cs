using DG.Tweening;
using Game.Events;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private GameEventChannelSO _gameEventChannel;

    private float _originCamDistance;
    private float _originCamFov;
    private CinemachineCamera _cinemachineCamera;
    private CinemachinePositionComposer _positionComposer;
    private CinemachineBasicMultiChannelPerlin _perlin;
    private Sequence _positionComposerSeq, _perlinSeq;

    private void Awake()
    {
        _cinemachineCamera = GetComponent<CinemachineCamera>();
        _perlin = GetComponent<CinemachineBasicMultiChannelPerlin>();
        _positionComposer = GetComponent<CinemachinePositionComposer>();
        _originCamDistance = _positionComposer.CameraDistance;
        _originCamFov = _cinemachineCamera.Lens.FieldOfView;
    }

    private void Start()
    {
        _gameEventChannel.AddListener<PlayerDead>(HandlePlayerDead);
        _gameEventChannel.AddListener<ChargeAttack>(HandleChargeAttack);
        _gameEventChannel.AddListener<ChargeAttackReachMaxTime>(HandleChargeAttackReachMaxTime);
    }


    private void OnDestroy()
    {
        _gameEventChannel.RemoveListener<PlayerDead>(HandlePlayerDead);
        _gameEventChannel.RemoveListener<ChargeAttack>(HandleChargeAttack);
        _gameEventChannel.RemoveListener<ChargeAttackReachMaxTime>(HandleChargeAttackReachMaxTime);
    }

    private void HandleChargeAttackReachMaxTime(ChargeAttackReachMaxTime evt)
    {
        if (_perlinSeq != null && _perlinSeq.IsActive()) _perlinSeq.Kill();
        _perlinSeq = DOTween.Sequence();
        _perlinSeq.Append(DOTween.To(() => _perlin.AmplitudeGain, x => _perlin.AmplitudeGain = x, .5f, .3f));
    }

    private void HandleChargeAttack(ChargeAttack evt)
    {
        var isAttackHolding = evt.isAttackCharging;
        if (_positionComposerSeq != null && _positionComposerSeq.IsActive()) _positionComposerSeq.Kill();

        _positionComposerSeq = DOTween.Sequence();

        if (isAttackHolding)
        {
            _positionComposerSeq.Append(DOTween.To(() => _positionComposer.CameraDistance,
                x => _positionComposer.CameraDistance = x, 22.5f,
                evt.maxChargeAttackTime));
        }
        else
        {
            _positionComposerSeq.Append(DOTween.To(() => _positionComposer.CameraDistance,
                x => _positionComposer.CameraDistance = x,
                _originCamDistance, .3f));
            if (_perlinSeq != null && _perlinSeq.IsActive()) _perlinSeq.Kill();
            _perlin.AmplitudeGain = 0;
        }
    }

    private void HandlePlayerDead(PlayerDead evt)
    {
        _perlin.AmplitudeGain = 1;
        DOTween.To(() => _positionComposer.CameraDistance, x => _positionComposer.CameraDistance = x, 50, 30)
            .SetUpdate(false);
    }
}