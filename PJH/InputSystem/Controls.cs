//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.11.2
//     from Assets/00.Work/PJH/InputSystem/Controls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @Controls: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @Controls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Controls"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""b303ccc3-b867-4504-80fb-6b800a4a6821"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""13b02319-1bc6-41d9-85d9-0352e1463f24"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Aim"",
                    ""type"": ""Value"",
                    ""id"": ""c237809f-854c-46b0-a29f-8bc71ed136f5"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Evasion"",
                    ""type"": ""Button"",
                    ""id"": ""481b5243-03b7-42d6-85cf-8ede1877eb03"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""16cfe936-fb0b-4861-b623-b31ab5d84a05"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ChangeWepoan_1"",
                    ""type"": ""Button"",
                    ""id"": ""6ff65bdc-1ce0-4128-a3cd-9c1a7867c860"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ChangeWepoan_2"",
                    ""type"": ""Button"",
                    ""id"": ""c565dd35-a522-4953-941b-3e103726bd20"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ChangeWepoan_3"",
                    ""type"": ""Button"",
                    ""id"": ""596d270c-ea1f-45c8-880c-5f439dd4cb8a"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""492be16a-a1bd-435e-9935-89a74305886a"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Skill1"",
                    ""type"": ""Button"",
                    ""id"": ""52e301ff-59a0-4db2-8414-b8d82f3fae51"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Skill2"",
                    ""type"": ""Button"",
                    ""id"": ""3af0d1c2-cc1b-4a14-9077-81f7a1ddb325"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Skill3"",
                    ""type"": ""Button"",
                    ""id"": ""6fde9d39-a43b-4f60-b707-c63f6e22059a"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WSAD"",
                    ""id"": ""31ccad80-de1e-45ef-92e2-dc113d04ce0c"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""04d1f37e-582b-49d9-9c0f-9570999e6b4d"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyMouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""df8c9a3f-9cc4-43a1-b2f4-5709628b5108"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyMouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""83567df9-ab74-445f-9aec-d5a07a8337b2"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyMouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""1b4bae39-d987-43d8-8b37-3882e8f6cb2b"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyMouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""1a436288-125e-44eb-9794-a17f502a8b8b"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyMouse"",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6f292c4a-4c3a-4af4-ac72-c58024a1b51b"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Evasion"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""af95570a-5cdf-40a6-b398-5d52f803b852"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": ""Hold(duration=0.15,pressPoint=0.3),Press(pressPoint=0.3)"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0ed64b01-04f9-48e8-b179-77df3c6508cf"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ChangeWepoan_1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""de871d11-7ec4-4e5c-843f-0dd0eeb2fe34"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ChangeWepoan_2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d4d43d17-c76f-4c57-aa8d-ee4c89bb1274"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ChangeWepoan_3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""41e9068b-2de5-4419-a1a4-80c53748fc77"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e6f2d9a6-13db-450c-bb2b-2ea1bb1c43e0"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": ""Hold,Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Skill1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ebe2a4de-9130-4d6d-ac42-259b3c274a4b"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": ""Hold,Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Skill2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1aa9d6bd-c5e6-4f07-b656-0895d8ee9715"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": ""Hold,Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Skill3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""UI"",
            ""id"": ""ac9f305d-2765-47c6-86ae-9d44b946d754"",
            ""actions"": [
                {
                    ""name"": ""ESC"",
                    ""type"": ""Button"",
                    ""id"": ""7fb4bf12-37ec-4589-bd30-24a64eafa6e9"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""310aa2cb-3a9a-42ea-a2bd-a46ebc1f8354"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ESC"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""KeyMouse"",
            ""bindingGroup"": ""KeyMouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Movement = m_Player.FindAction("Movement", throwIfNotFound: true);
        m_Player_Aim = m_Player.FindAction("Aim", throwIfNotFound: true);
        m_Player_Evasion = m_Player.FindAction("Evasion", throwIfNotFound: true);
        m_Player_Attack = m_Player.FindAction("Attack", throwIfNotFound: true);
        m_Player_ChangeWepoan_1 = m_Player.FindAction("ChangeWepoan_1", throwIfNotFound: true);
        m_Player_ChangeWepoan_2 = m_Player.FindAction("ChangeWepoan_2", throwIfNotFound: true);
        m_Player_ChangeWepoan_3 = m_Player.FindAction("ChangeWepoan_3", throwIfNotFound: true);
        m_Player_Interact = m_Player.FindAction("Interact", throwIfNotFound: true);
        m_Player_Skill1 = m_Player.FindAction("Skill1", throwIfNotFound: true);
        m_Player_Skill2 = m_Player.FindAction("Skill2", throwIfNotFound: true);
        m_Player_Skill3 = m_Player.FindAction("Skill3", throwIfNotFound: true);
        // UI
        m_UI = asset.FindActionMap("UI", throwIfNotFound: true);
        m_UI_ESC = m_UI.FindAction("ESC", throwIfNotFound: true);
    }

    ~@Controls()
    {
        UnityEngine.Debug.Assert(!m_Player.enabled, "This will cause a leak and performance issues, Controls.Player.Disable() has not been called.");
        UnityEngine.Debug.Assert(!m_UI.enabled, "This will cause a leak and performance issues, Controls.UI.Disable() has not been called.");
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Player
    private readonly InputActionMap m_Player;
    private List<IPlayerActions> m_PlayerActionsCallbackInterfaces = new List<IPlayerActions>();
    private readonly InputAction m_Player_Movement;
    private readonly InputAction m_Player_Aim;
    private readonly InputAction m_Player_Evasion;
    private readonly InputAction m_Player_Attack;
    private readonly InputAction m_Player_ChangeWepoan_1;
    private readonly InputAction m_Player_ChangeWepoan_2;
    private readonly InputAction m_Player_ChangeWepoan_3;
    private readonly InputAction m_Player_Interact;
    private readonly InputAction m_Player_Skill1;
    private readonly InputAction m_Player_Skill2;
    private readonly InputAction m_Player_Skill3;
    public struct PlayerActions
    {
        private @Controls m_Wrapper;
        public PlayerActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Player_Movement;
        public InputAction @Aim => m_Wrapper.m_Player_Aim;
        public InputAction @Evasion => m_Wrapper.m_Player_Evasion;
        public InputAction @Attack => m_Wrapper.m_Player_Attack;
        public InputAction @ChangeWepoan_1 => m_Wrapper.m_Player_ChangeWepoan_1;
        public InputAction @ChangeWepoan_2 => m_Wrapper.m_Player_ChangeWepoan_2;
        public InputAction @ChangeWepoan_3 => m_Wrapper.m_Player_ChangeWepoan_3;
        public InputAction @Interact => m_Wrapper.m_Player_Interact;
        public InputAction @Skill1 => m_Wrapper.m_Player_Skill1;
        public InputAction @Skill2 => m_Wrapper.m_Player_Skill2;
        public InputAction @Skill3 => m_Wrapper.m_Player_Skill3;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void AddCallbacks(IPlayerActions instance)
        {
            if (instance == null || m_Wrapper.m_PlayerActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Add(instance);
            @Movement.started += instance.OnMovement;
            @Movement.performed += instance.OnMovement;
            @Movement.canceled += instance.OnMovement;
            @Aim.started += instance.OnAim;
            @Aim.performed += instance.OnAim;
            @Aim.canceled += instance.OnAim;
            @Evasion.started += instance.OnEvasion;
            @Evasion.performed += instance.OnEvasion;
            @Evasion.canceled += instance.OnEvasion;
            @Attack.started += instance.OnAttack;
            @Attack.performed += instance.OnAttack;
            @Attack.canceled += instance.OnAttack;
            @ChangeWepoan_1.started += instance.OnChangeWepoan_1;
            @ChangeWepoan_1.performed += instance.OnChangeWepoan_1;
            @ChangeWepoan_1.canceled += instance.OnChangeWepoan_1;
            @ChangeWepoan_2.started += instance.OnChangeWepoan_2;
            @ChangeWepoan_2.performed += instance.OnChangeWepoan_2;
            @ChangeWepoan_2.canceled += instance.OnChangeWepoan_2;
            @ChangeWepoan_3.started += instance.OnChangeWepoan_3;
            @ChangeWepoan_3.performed += instance.OnChangeWepoan_3;
            @ChangeWepoan_3.canceled += instance.OnChangeWepoan_3;
            @Interact.started += instance.OnInteract;
            @Interact.performed += instance.OnInteract;
            @Interact.canceled += instance.OnInteract;
            @Skill1.started += instance.OnSkill1;
            @Skill1.performed += instance.OnSkill1;
            @Skill1.canceled += instance.OnSkill1;
            @Skill2.started += instance.OnSkill2;
            @Skill2.performed += instance.OnSkill2;
            @Skill2.canceled += instance.OnSkill2;
            @Skill3.started += instance.OnSkill3;
            @Skill3.performed += instance.OnSkill3;
            @Skill3.canceled += instance.OnSkill3;
        }

        private void UnregisterCallbacks(IPlayerActions instance)
        {
            @Movement.started -= instance.OnMovement;
            @Movement.performed -= instance.OnMovement;
            @Movement.canceled -= instance.OnMovement;
            @Aim.started -= instance.OnAim;
            @Aim.performed -= instance.OnAim;
            @Aim.canceled -= instance.OnAim;
            @Evasion.started -= instance.OnEvasion;
            @Evasion.performed -= instance.OnEvasion;
            @Evasion.canceled -= instance.OnEvasion;
            @Attack.started -= instance.OnAttack;
            @Attack.performed -= instance.OnAttack;
            @Attack.canceled -= instance.OnAttack;
            @ChangeWepoan_1.started -= instance.OnChangeWepoan_1;
            @ChangeWepoan_1.performed -= instance.OnChangeWepoan_1;
            @ChangeWepoan_1.canceled -= instance.OnChangeWepoan_1;
            @ChangeWepoan_2.started -= instance.OnChangeWepoan_2;
            @ChangeWepoan_2.performed -= instance.OnChangeWepoan_2;
            @ChangeWepoan_2.canceled -= instance.OnChangeWepoan_2;
            @ChangeWepoan_3.started -= instance.OnChangeWepoan_3;
            @ChangeWepoan_3.performed -= instance.OnChangeWepoan_3;
            @ChangeWepoan_3.canceled -= instance.OnChangeWepoan_3;
            @Interact.started -= instance.OnInteract;
            @Interact.performed -= instance.OnInteract;
            @Interact.canceled -= instance.OnInteract;
            @Skill1.started -= instance.OnSkill1;
            @Skill1.performed -= instance.OnSkill1;
            @Skill1.canceled -= instance.OnSkill1;
            @Skill2.started -= instance.OnSkill2;
            @Skill2.performed -= instance.OnSkill2;
            @Skill2.canceled -= instance.OnSkill2;
            @Skill3.started -= instance.OnSkill3;
            @Skill3.performed -= instance.OnSkill3;
            @Skill3.canceled -= instance.OnSkill3;
        }

        public void RemoveCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPlayerActions instance)
        {
            foreach (var item in m_Wrapper.m_PlayerActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PlayerActions @Player => new PlayerActions(this);

    // UI
    private readonly InputActionMap m_UI;
    private List<IUIActions> m_UIActionsCallbackInterfaces = new List<IUIActions>();
    private readonly InputAction m_UI_ESC;
    public struct UIActions
    {
        private @Controls m_Wrapper;
        public UIActions(@Controls wrapper) { m_Wrapper = wrapper; }
        public InputAction @ESC => m_Wrapper.m_UI_ESC;
        public InputActionMap Get() { return m_Wrapper.m_UI; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UIActions set) { return set.Get(); }
        public void AddCallbacks(IUIActions instance)
        {
            if (instance == null || m_Wrapper.m_UIActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_UIActionsCallbackInterfaces.Add(instance);
            @ESC.started += instance.OnESC;
            @ESC.performed += instance.OnESC;
            @ESC.canceled += instance.OnESC;
        }

        private void UnregisterCallbacks(IUIActions instance)
        {
            @ESC.started -= instance.OnESC;
            @ESC.performed -= instance.OnESC;
            @ESC.canceled -= instance.OnESC;
        }

        public void RemoveCallbacks(IUIActions instance)
        {
            if (m_Wrapper.m_UIActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IUIActions instance)
        {
            foreach (var item in m_Wrapper.m_UIActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_UIActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public UIActions @UI => new UIActions(this);
    private int m_KeyMouseSchemeIndex = -1;
    public InputControlScheme KeyMouseScheme
    {
        get
        {
            if (m_KeyMouseSchemeIndex == -1) m_KeyMouseSchemeIndex = asset.FindControlSchemeIndex("KeyMouse");
            return asset.controlSchemes[m_KeyMouseSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnAim(InputAction.CallbackContext context);
        void OnEvasion(InputAction.CallbackContext context);
        void OnAttack(InputAction.CallbackContext context);
        void OnChangeWepoan_1(InputAction.CallbackContext context);
        void OnChangeWepoan_2(InputAction.CallbackContext context);
        void OnChangeWepoan_3(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnSkill1(InputAction.CallbackContext context);
        void OnSkill2(InputAction.CallbackContext context);
        void OnSkill3(InputAction.CallbackContext context);
    }
    public interface IUIActions
    {
        void OnESC(InputAction.CallbackContext context);
    }
}
