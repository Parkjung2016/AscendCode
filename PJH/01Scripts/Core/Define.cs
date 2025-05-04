using System.Collections.Generic;
using UnityEngine;

namespace PJH.Core
{
    public static class Define
    {
        public enum  EScene
        {
            Unknown,
            Title,
            InGame,
            InGame_PJH,
            InGame_KHJ,
            PlayTestScene_LJS
        }

        public enum ENoticeLevel
        {
            WARNING,
            ERROR,
            DEFAULT,
        }

        public enum EDamageCategory
        {
            Normal,
            Debuff_Red,
            Debuff_Yellow,
        }

        public enum EResourceType
        {
            POINT,
        }

        public enum ESocketType
        {
            HANDR,
            HANDL
        }

        public enum EPlayerEquipmentType
        {
            Sword,
            Bow,
            Hammer
        }

        public static readonly Dictionary<ENoticeLevel, Color> NoticeColor =
            new()
            {
                { ENoticeLevel.ERROR, Color.red },
                { ENoticeLevel.WARNING, Color.yellow },
                { ENoticeLevel.DEFAULT, Color.white }
            };


        public static class MLayerMask
        {
            public static readonly LayerMask WhatIsGround = LayerMask.GetMask("Ground");
            public static readonly LayerMask WhatIsWall = LayerMask.GetMask("Wall");
            public static readonly LayerMask WhatIsEnemy = LayerMask.GetMask("Enemy");
            public static readonly LayerMask WhatIsInteractable = LayerMask.GetMask("Interactable");
        }
    }
}