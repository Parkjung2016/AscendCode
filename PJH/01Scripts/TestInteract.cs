using Game.Events;
using PJH.Core;
using UnityEngine;

public class TestInteract : MonoBehaviour, MInterface.IInteractable
{
    public GameEventChannelSO _interactEventChannelSO;
    [field: SerializeField] public MInterface.InteractInfo InteractInfo { get; set; }

    public void Interact()
    {
        Debug.Log("??");
    }
}