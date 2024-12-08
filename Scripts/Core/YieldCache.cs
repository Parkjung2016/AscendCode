using System;
using System.Collections.Generic;
using UnityEngine;

namespace PJH.Core
{
    static class YieldCache
    {
        class FloatComparer : IEqualityComparer<float>
        {
            bool IEqualityComparer<float>.Equals(float x, float y)
            {
                return x == y;
            }

            int IEqualityComparer<float>.GetHashCode(float obj)
            {
                return obj.GetHashCode();
            }
        }

        public static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();
        public static readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();

        private static readonly Dictionary<float, WaitForSeconds> _timeInterval =
            new(new FloatComparer());

        private static readonly Dictionary<float, WaitForSecondsRealtime> _timeIntervalReal =
            new(new FloatComparer());
        private static readonly Dictionary<Func<bool>, WaitUntil> _waitUntils = 
            new Dictionary<Func<bool>, WaitUntil>();

        public static WaitForSeconds WaitForSeconds(float seconds)
        {
            WaitForSeconds wfs;
            if (!_timeInterval.TryGetValue(seconds, out wfs))
                _timeInterval.Add(seconds, wfs = new WaitForSeconds(seconds));
            return wfs;
        }

        public static WaitForSecondsRealtime WaitForSecondsRealTime(float seconds)
        {
            WaitForSecondsRealtime wfsReal;
            if (!_timeIntervalReal.TryGetValue(seconds, out wfsReal))
                _timeIntervalReal.Add(seconds, wfsReal = new WaitForSecondsRealtime(seconds));
            return wfsReal;
        }
        public static WaitUntil WaitUntil(Func<bool> predicate)
        {
            WaitUntil wn;
            if (!_waitUntils.TryGetValue(predicate, out wn))
            {
                _waitUntils.Add(predicate, wn = new WaitUntil(predicate));
            }

            return wn;
        }
    }
}