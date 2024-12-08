using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PJH.Agent.Player
{
    using Core;
    using Equipment.Weapon;

    public class PlayerEquipmentController : SerializedMonoBehaviour, MInterface.IAgentComponent
    {
        public event Action<PlayerWeapon> WeaponChangeEvent;
        public event Action<Equipment.Equipment> AddPossessionEquipmentEvent;
        private Player _player;

        [SerializeField] private Dictionary<Define.ESocketType, Transform> _sockets = new();

        [ReadOnly, SerializeField] private Dictionary<Define.EPlayerEquipmentType, Equipment.Equipment> _allEquipments;
        public Dictionary<Define.EPlayerEquipmentType, Equipment.Equipment> AllEquipments => _allEquipments;
        private Dictionary<Define.ESocketType, Equipment.Equipment> _currentEquipments = new();

        [ReadOnly, SerializeField] private List<Equipment.Equipment> _possessionEquipments = new();


        public void Initialize(Agent agent)
        {

            _player = agent as Player;
            _allEquipments = new();
            foreach (var pair in _sockets)
            {
                foreach (Define.EPlayerEquipmentType weaponType in Enum.GetValues(typeof(Define.EPlayerEquipmentType)))
                {
                    var equipment = pair.Value.Find(weaponType.ToString())?.GetComponent<Equipment.Equipment>();
                    if (equipment)
                    {
                        equipment.Init(_player);
                        _allEquipments.Add(weaponType, equipment);
                    }
                }
            }
        }

        private async void Start()
        {
            await UniTask.WaitForFixedUpdate();
            foreach (var equipment in _allEquipments.Values)
            {
                equipment.gameObject.SetActive(false);
            }

            foreach (Define.EPlayerEquipmentType equipmentType in Enum.GetValues(typeof(Define.EPlayerEquipmentType)))
            {
                AddPossessionEquipment(equipmentType);
            }

            ChangeItemInSocket(_possessionEquipments[0]);
        }

        public void AfterInit()
        {
            _player.PlayerInput.ChangeWeaponEvent += HandleChangeWeaponEvent;
        }


        public void Dispose()
        {
            _player.PlayerInput.ChangeWeaponEvent -= HandleChangeWeaponEvent;
        }

        private void HandleChangeWeaponEvent(int index)
        {
            var attackCompo = _player.GetCompo<PlayerAttack>();
            var movementCompo = _player.GetCompo<PlayerMovement>();
            if (_player.IsUsingSkill || attackCompo.IsAttacking || movementCompo.IsEvasion || _player.IsHitting) return;
            if (_possessionEquipments.Count - 1 >= index)
                ChangeItemInSocket(_possessionEquipments[index]);
        }



        public T GetEquipment<T>(Define.ESocketType socketType) where T : Equipment.Equipment
        {
            if (_currentEquipments.TryGetValue(socketType, out var equipment))
            {
                return equipment as T;
            }

            return null;
        }

        public PlayerWeapon GetWeapon()
        {
            var weapon = GetEquipment<PlayerWeapon>(Define.ESocketType.HANDL);
            if (weapon == null)
                weapon = GetEquipment<PlayerWeapon>(Define.ESocketType.HANDR);
            if (weapon != null)
                return weapon;
            return null;
        }

        public void ChangeItemInSocket(Equipment.Equipment equipment)
        {
            var socketType = equipment.socketType;
            if (_currentEquipments.ContainsKey(socketType) && _currentEquipments[socketType] == equipment) return;

            bool existsCurrentEquipmentInSocket = false;
            existsCurrentEquipmentInSocket = _currentEquipments.ContainsKey(Define.ESocketType.HANDL) ||
                                             _currentEquipments.ContainsKey(Define.ESocketType.HANDR);

            if (existsCurrentEquipmentInSocket)
            {
                if (!_currentEquipments.Remove(Define.ESocketType.HANDL,
                        out var weapon))
                    _currentEquipments.Remove(Define.ESocketType.HANDR,
                        out weapon);
                var playerWeapon = (weapon as PlayerWeapon);

                playerWeapon?.UnEquip(() => { playerWeapon.gameObject.SetActive(false); });
            }


            _currentEquipments[socketType] = equipment;
            equipment.gameObject.SetActive(true);
            equipment.Equip();
            WeaponChangeEvent?.Invoke(equipment as PlayerWeapon);
        }


        public void AddPossessionEquipment(Define.EPlayerEquipmentType type)
        {
            Equipment.Equipment equipment = _allEquipments[type];
            if (_possessionEquipments.Contains(equipment))
                return;

            _possessionEquipments.Add(equipment);
            AddPossessionEquipmentEvent?.Invoke(equipment);
        }
    }
}