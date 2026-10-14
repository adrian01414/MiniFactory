using System;
using System.Collections.Generic;
using MiniFactory.Core.Analytics;
using UnityEngine;

namespace MiniFactory.Game.Services
{
    public sealed class AnalyticsService
    {
        private readonly List<IAnalyticsProvider> _providers = new List<IAnalyticsProvider>();

        public AnalyticsService(IEnumerable<IAnalyticsProvider> providers)
        {
            if (providers == null)
            {
                return;
            }

            foreach (IAnalyticsProvider provider in providers)
            {
                if (provider != null)
                {
                    _providers.Add(provider);
                }
            }
        }

        public void Initialize()
        {
            for (int i = 0; i < _providers.Count; i++)
            {
                try
                {
                    _providers[i].Initialize();
                }
                catch (Exception exception)
                {
                    Debug.LogWarning("Analytics provider initialize failed: " + exception.Message);
                }
            }
        }

        public void Track(string name, IReadOnlyDictionary<string, object> parameters)
        {
            for (int i = 0; i < _providers.Count; i++)
            {
                try
                {
                    _providers[i].SendEvent(name, parameters);
                }
                catch (Exception exception)
                {
                    Debug.LogWarning("Analytics provider send failed: " + exception.Message);
                }
            }
        }

        public void Flush()
        {
            for (int i = 0; i < _providers.Count; i++)
            {
                try
                {
                    _providers[i].Flush();
                }
                catch (Exception exception)
                {
                    Debug.LogWarning("Analytics provider flush failed: " + exception.Message);
                }
            }
        }
    }
}
