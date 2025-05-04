using System;
using PJH.Core;
using UnityEngine;


namespace Game.Events
{
    public static class GameEvents
    {
        public static readonly PlayerDead PlayerDead = new();
        public static readonly ChargeAttack ChargeAttack = new();
        public static readonly ChargeAttackReachMaxTime ChargeAttackReachMaxTime = new();
        public static readonly InteractObjectInfo InteractObjectInfo = new();
        public static readonly RightBeforeThePlayerDeath RightBeforeThePlayerDeath = new();
        public static readonly GetStoreSkill GetStoreSkill = new();
        public static readonly NextStage NextStage = new();
        public static readonly ClearStage ClearStage = new();
        public static readonly FadeIn FadeIn = new();
        public static readonly FadeOut FadeOut = new();
        public static readonly CameraPerlin CameraPerlin = new();
        public static readonly CameraImpulse CameraImpulse = new();
    }

    public class PlayerDead : GameEvent
    {
        public Action FadeEndEvent;
    }

    public class FadeIn : GameEvent
    {
        public float duration;
        public Action EndEvent;
    }

    public class FadeOut : GameEvent
    {
        public float duration;
        public Action EndEvent;
    }

    public class ChargeAttack : GameEvent
    {
        public float maxChargeAttackTime;
        public bool isAttackCharging;
    }

    public class RightBeforeThePlayerDeath : GameEvent
    {
        public bool isRightBeforeThePlayerDeath;
    }

    public class ChargeAttackReachMaxTime : GameEvent
    {
    }

    public class InteractObjectInfo : GameEvent
    {
        public Transform targetTrm;
        public MInterface.InteractInfo targetInteractInfo;
    }

    public class GetStoreSkill : GameEvent
    {
        public string skillTypeName;
        public int keyIdx;
    }

    public class NextStage : GameEvent
    {
    }

    public class ClearStage : GameEvent
    {
    }

    public class CameraPerlin : GameEvent
    {
        public float increaseDur;
        public float strength;
    }

    public class CameraImpulse : GameEvent
    {
        public float strength;
    }
}