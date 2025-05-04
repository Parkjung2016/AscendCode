using Game.Events;
using PJH.Core;
using PJH.Manager;
using PJH.Projectile;
using UnityEngine;

namespace Game.Core
{
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField] private GameEventChannelSO _spawnEventChannel;
        private PoolManagerSO _poolManager;

        [SerializeField] private PoolTypeSO _arrow;

        [SerializeField] private PoolTypeSO _lunarSlashEffect;

        private void Awake()
        {
            _poolManager = Managers.Addressable.Load<PoolManagerSO>("PoolManager");
        }

        private void Start()
        {
            _spawnEventChannel.AddListener<PlayerArrowSpawn>(HandleArrowSpawn);
            _spawnEventChannel.AddListener<EffectSpawn>(HandleEffectSpawn);
            _spawnEventChannel.AddListener<LunarSlashEffectSpawn>(HandleLunarSlashEffectSpawn);
        }

        private void OnDestroy()
        {
            _spawnEventChannel.RemoveListener<PlayerArrowSpawn>(HandleArrowSpawn);
            _spawnEventChannel.RemoveListener<EffectSpawn>(HandleEffectSpawn);
            _spawnEventChannel.RemoveListener<LunarSlashEffectSpawn>(HandleLunarSlashEffectSpawn);
        }

        private void HandleLunarSlashEffectSpawn(LunarSlashEffectSpawn evt)
        {
            var effect = _poolManager.Pop(_lunarSlashEffect) as LunarSlashEffectPlayer;
            effect.ChangeSize(evt.size);
            effect.power = evt.power;
            effect.attribute = evt.attribute;
            effect.PlayEffects(evt.position, evt.rotation);
        }

        private void HandleArrowSpawn(PlayerArrowSpawn evt)
        {
            var arrow = _poolManager.Pop(_arrow) as Arrow;
            arrow.transform.position = evt.position;
            arrow.owner = evt.owner;
            arrow.Fire(evt.direction, evt.speed, evt.damage);
        }

        private void HandleEffectSpawn(EffectSpawn evt)
        {
            var effect = _poolManager.Pop(evt.effectType) as PoolEffectPlayer;
            effect.PlayEffects(evt.position, evt.rotation);
        }
    }
}