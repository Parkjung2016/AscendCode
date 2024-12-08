using System;
using System.Collections.Generic;
using PJH.Core;

namespace PJH.Manager
{
    public class ResourceManager
    {
        public event Action<Define.EResourceType, int> ChangedResourceEvent;
        private Dictionary<Define.EResourceType, int> _resources;

        public ResourceManager()
        {
            _resources = new();

            foreach (Define.EResourceType type in Enum.GetValues(typeof(Define.EResourceType)))
            {
                _resources.Add(type, 0);
            }
        }

        public void IncreaseResource(Define.EResourceType type, int count)
        {
            _resources[type] += count;
            ChangedResourceEvent?.Invoke(type, _resources[type]);
        }

        public void DecreaseResource(Define.EResourceType type, int count)
        {
            _resources[type] -= count;
            ChangedResourceEvent?.Invoke(type, _resources[type]);
        }

        public void SetResource(Define.EResourceType type, int count)
        {
            _resources[type] = count;
            ChangedResourceEvent?.Invoke(type, _resources[type]);
        }
    }
}