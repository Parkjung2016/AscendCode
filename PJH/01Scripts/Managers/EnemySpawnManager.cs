using System;
using System.Collections.Generic;
using System.Linq;
using PJH.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PJH.Manager
{
    public class EnemySpawnManager : LJS.MonoSingleton<EnemySpawnManager>
    {
        [SerializeField] private Bounds _outerBounds;
        [SerializeField] private Bounds _innerBounds;
        [SerializeField] private float _spawnY;
        private EnemySpawnDataListSO _enemySpawnDataList;
        private PoolManagerSO _poolManager;
        public event Action EnemyAllDeadEvent;
        public event Action<int> ChangedEnemyCountEvent;
        public EnemySpawnDataListSO EnemySpawnDataList => _enemySpawnDataList;
        public List<MInterface.IEnemy> EnemyList { get; private set; }

        public int LeftEnemyCount => EnemyList.Count;

        private Bounds[] _outerBoundsArray;

        private void Awake()
        {
            _outerBoundsArray = SubtractBounds(_outerBounds, _innerBounds);

            _enemySpawnDataList = Managers.Addressable.Load<EnemySpawnDataListSO>("EnemySpawnDataList");
            _poolManager = Managers.Addressable.Load<PoolManagerSO>("PoolManager");

            EnemyList = new();
            WaveManager.Instance.StartNextStageEvent += HandleStartNextStageEvent;
            WaveManager.Instance.BossStageEvent += HandleBossStageEvent;

            _outerBoundsArray = SubtractBounds(_outerBounds, _innerBounds);
        }

        private void HandleBossStageEvent(int stage)
        {
            var spawnData = _enemySpawnDataList.spawnDataList[stage];
            var bossPoolType = spawnData.enemyPoolTypes[0];
            var boss = _poolManager.Pop(bossPoolType) as MInterface.IEnemy;
            AddObject(boss);
        }

        Bounds[] SubtractBounds(Bounds outer, Bounds inner)
        {
            if (!outer.Intersects(inner))
            {
                return new Bounds[] { outer };
            }

            Bounds top = new Bounds(
                new Vector3(outer.center.x, (outer.max.y + inner.max.y) / 2, outer.center.z),
                new Vector3(outer.size.x, outer.max.y - inner.max.y, outer.size.z)
            );

            Bounds bottom = new Bounds(
                new Vector3(outer.center.x, (outer.min.y + inner.min.y) / 2, outer.center.z),
                new Vector3(outer.size.x, inner.min.y - outer.min.y, outer.size.z)
            );

            Bounds left = new Bounds(
                new Vector3((outer.min.x + inner.min.x) / 2, inner.center.y, outer.center.z),
                new Vector3(inner.min.x - outer.min.x, inner.size.y, outer.size.z)
            );

            Bounds right = new Bounds(
                new Vector3((outer.max.x + inner.max.x) / 2, inner.center.y, outer.center.z),
                new Vector3(outer.max.x - inner.max.x, inner.size.y, outer.size.z)
            );

            return new Bounds[] { top, bottom, left, right }
                .Where(b => b.size.x >= 0 && b.size is { y: >= 0, z: >= 0 })
                .ToArray();
        }

        private void HandleStartNextStageEvent(int stage)
        {
            SpawnEnemyByStageData(stage);
        }

        public void DeleteObject(MInterface.IEnemy delObj)
        {
            EnemyList.Remove(delObj);
            ChangedEnemyCountEvent?.Invoke(LeftEnemyCount);
            if (EnemyList.Count <= 0)
            {
                EnemyAllDeadEvent?.Invoke();
                return;
            }
        }

        private void AddObject(MInterface.IEnemy addObj)
        {
            EnemyList.Add(addObj);
            ChangedEnemyCountEvent?.Invoke(LeftEnemyCount);
        }

        private void SpawnEnemyByStageData(int stage)
        {
            var spawnData = _enemySpawnDataList.spawnDataList[stage];
            for (int i = 0; i < spawnData.maxEnemyCount; i++)
            {
                var enemyPoolType = spawnData.enemyPoolTypes[Random.Range(0, spawnData.enemyPoolTypes.Count)];
                SpawnEnemy(enemyPoolType);
            }
        }

        public Enemy SpawnEnemy(PoolTypeSO enemyPoolType)
        {
            var bounds = _outerBoundsArray[Random.Range(0, _outerBoundsArray.Length)];
            Vector3 min = transform.position + bounds.min;
            Vector3 max = transform.position + bounds.max;

            Vector3 spawnPos = new Vector3(
                Random.Range(min.x, max.x),
                _spawnY,
                Random.Range(min.z, max.z));
            var enemy = _poolManager.Pop(enemyPoolType) as Enemy;
            enemy.transform.position = spawnPos;
            enemy.NavAgent.enabled = true;
            enemy.BehaviourTree.EnableBehavior();
            AddObject(enemy as MInterface.IEnemy);
            return enemy;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position + _outerBounds.center, _outerBounds.size);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + _innerBounds.center, _innerBounds.size);
            Gizmos.color = Color.blue;
            _outerBoundsArray = SubtractBounds(_outerBounds, _innerBounds);
            foreach (var bounds in _outerBoundsArray)
            {
                Gizmos.DrawWireCube(transform.position + bounds.center, bounds.size);
            }
        }

#endif
    }
}