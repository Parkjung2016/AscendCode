using System;
using PJH.Manager;
using UnityEngine;

public class PoolManagerMono : MonoBehaviour
{
    private PoolManagerSO _poolManager;

    private void Awake()
    {
        _poolManager = Managers.Addressable.Load<PoolManagerSO>("PoolManager");
        StartCoroutine(_poolManager.InitializePool(transform));
    }

    private void OnDestroy()
    {
        _poolManager.ReleasePoolAsset();
    }
}