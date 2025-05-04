using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PJH.Manager
{
    [Serializable]
    public struct EnemySpawnData
    {
        public List<PoolTypeSO> enemyPoolTypes;
        [HideIf("isBossStage")] public int maxEnemyCount;
        public bool isBossStage;
        [ShowIf("isBossStage")] public string bossName;
        [ShowIf("isBossStage")] public Sprite bossImage;
    }

    [CreateAssetMenu(menuName = "SO/Spawn/EnemySpawnDataList")]
    public class EnemySpawnDataListSO : ScriptableObject
    {
        public List<EnemySpawnData> spawnDataList;
    }
}