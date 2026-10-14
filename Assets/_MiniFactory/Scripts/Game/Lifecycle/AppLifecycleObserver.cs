using System;
using UnityEngine;

namespace MiniFactory.Game.Lifecycle
{
    public sealed class AppLifecycleObserver : MonoBehaviour
    {
        public event Action Paused;
        public event Action Resumed;
        public event Action Quitting;

        private bool _isPaused;

        private void OnApplicationPause(bool pauseStatus)
        {
            SetPaused(pauseStatus);
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SetPaused(!hasFocus);
        }

        private void OnApplicationQuit()
        {
            Quitting?.Invoke();
        }

        private void SetPaused(bool paused)
        {
            if (_isPaused == paused)
            {
                return;
            }

            _isPaused = paused;

            if (paused)
            {
                Paused?.Invoke();
            }
            else
            {
                Resumed?.Invoke();
            }
        }
    }
}
