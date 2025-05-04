using System;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum Ailment : int
{
    None = 0,
    Ignited = 1,
    Chilled = 2,
    Shocked = 4
}

public delegate void AilmentChange(Ailment newAilment, float duration);

public delegate void AilmentDotDamageEvent(Ailment ailmentType, int damage);

[Serializable]
public class AilmentStat
{
    private Dictionary<Ailment, float> _ailmentTimerDictionary;
    private Dictionary<Ailment, int> _ailmentDamageDictionary;


    public Ailment currentAilment;

    public event AilmentDotDamageEvent DotDamageEvent;
    public event AilmentChange AilmentChangeEvent;

    private float _igniteTimer;
    private float _igniteDamageCooldown = 1f;

    public AilmentStat()
    {
        _ailmentTimerDictionary = new Dictionary<Ailment, float>();
        _ailmentDamageDictionary = new Dictionary<Ailment, int>();

        foreach (Ailment ailment in Enum.GetValues(typeof(Ailment)))
        {
            if (ailment != Ailment.None)
            {
                _ailmentTimerDictionary.Add(ailment, 0f);
                _ailmentDamageDictionary.Add(ailment, 0);
            }
        }
    }

    public void UpdateAilment()
    {
        foreach (Ailment ailment in Enum.GetValues(typeof(Ailment)))
        {
            if (ailment == Ailment.None) continue;

            if (_ailmentTimerDictionary[ailment] > 0)
            {
                _ailmentTimerDictionary[ailment] -= Time.deltaTime;
                if (_ailmentTimerDictionary[ailment] <= 0)
                {
                    currentAilment ^= ailment;
                    AilmentChangeEvent?.Invoke(currentAilment, 0);
                }
            }
        }

        IgniteTimer();
    }

    private void IgniteTimer()
    {
        if ((currentAilment & Ailment.Ignited) == 0) return;

        _igniteTimer += Time.deltaTime;
        if (_ailmentTimerDictionary[Ailment.Ignited] > 0 && _igniteTimer > _igniteDamageCooldown)
        {
            _igniteTimer = 0;
            DotDamageEvent?.Invoke(Ailment.Ignited, _ailmentDamageDictionary[Ailment.Ignited]);
        }
    }

    public bool HasAilment(Ailment ailment)
    {
        return (currentAilment & ailment) > 0;
    }

    public void ApplyAilments(Ailment value, float duration, int damage)
    {
        Ailment oldValue = currentAilment;
        currentAilment |= value;

        if (oldValue != currentAilment)
            AilmentChangeEvent?.Invoke(currentAilment, duration);

        if ((value & Ailment.Ignited) > 0)
        {
            SetAilment(Ailment.Ignited, duration: duration, damage: damage);
        }
        else if ((value & Ailment.Chilled) > 0)
        {
            SetAilment(Ailment.Chilled, duration, damage);
        }
        else if ((value & Ailment.Shocked) > 0)
        {
            SetAilment(Ailment.Shocked, duration, damage);
        }
    }

    private void SetAilment(Ailment ailment, float duration, int damage)
    {
        _ailmentTimerDictionary[ailment] = duration;
        _ailmentDamageDictionary[ailment] = damage;
    }
}