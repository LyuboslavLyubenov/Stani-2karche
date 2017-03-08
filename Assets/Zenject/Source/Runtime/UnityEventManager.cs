#if !NOT_UNITY3D

namespace Assets.Zenject.Source.Runtime
{

    using System;

    using Assets.Zenject.Source.Usage;

    using UnityEngine;

    // Note: this corresponds to the values expected in
    // Input.GetMouseButtonDown() and similar methods
    public enum MouseButtons
    {
        None,
        Left,
        Right,
        Middle,
    }

    [System.Diagnostics.DebuggerStepThrough]
    public class UnityEventManager : MonoBehaviour, ITickable
    {
        public event Action ApplicationGainedFocus = delegate { };
        public event Action ApplicationLostFocus = delegate { };
        public event Action<bool> ApplicationFocusChanged = delegate { };
        public event Action ApplicationQuit = delegate { };
        public event Action ChangingScenes = delegate { };
        public event Action DrawGizmos = delegate { };
        public event Action<MouseButtons> MouseButtonDown = delegate { };
        public event Action<MouseButtons> MouseButtonUp = delegate { };
        public event Action LeftMouseButtonDown = delegate { };
        public event Action LeftMouseButtonUp = delegate { };
        public event Action MiddleMouseButtonDown = delegate { };
        public event Action MiddleMouseButtonUp = delegate { };
        public event Action RightMouseButtonDown = delegate { };
        public event Action RightMouseButtonUp = delegate { };
        public event Action MouseMoved = delegate { };
        public event Action ScreenSizeChanged = delegate { };
        public event Action Started = delegate { };
        public event Action<float> MouseWheelMoved = delegate { };

        Vector3 _lastMousePosition;

        int _lastWidth;
        int _lastHeight;

        public bool IsFocused
        {
            get;
            private set;
        }

        void Start()
        {
            this._lastWidth = Screen.width;
            this._lastHeight = Screen.height;
            this.Started();
        }

        public void Tick()
        {
            if (Input.GetMouseButtonDown((int)MouseButtons.Left))
            {
                this.LeftMouseButtonDown();
                this.MouseButtonDown(MouseButtons.Left);
            }
            else if (Input.GetMouseButtonUp((int)MouseButtons.Left))
            {
                this.LeftMouseButtonUp();
                this.MouseButtonUp(MouseButtons.Left);
            }

            if (Input.GetMouseButtonDown((int)MouseButtons.Right))
            {
                this.RightMouseButtonDown();
                this.MouseButtonDown(MouseButtons.Right);
            }
            else if (Input.GetMouseButtonUp((int)MouseButtons.Right))
            {
                this.RightMouseButtonUp();
                this.MouseButtonUp(MouseButtons.Right);
            }

            if (Input.GetMouseButtonDown((int)MouseButtons.Middle))
            {
                this.MiddleMouseButtonDown();
                this.MouseButtonDown(MouseButtons.Middle);
            }
            else if (Input.GetMouseButtonUp((int)MouseButtons.Middle))
            {
                this.MiddleMouseButtonUp();
                this.MouseButtonUp(MouseButtons.Middle);
            }

            if (this._lastMousePosition != Input.mousePosition)
            {
                this._lastMousePosition = Input.mousePosition;
                this.MouseMoved();
            }

            // By default this event returns 1/10 for each discrete rotation
            // so correct that
            var mouseWheelDelta = 10.0f * Input.GetAxis("Mouse ScrollWheel");

            if (!Mathf.Approximately(mouseWheelDelta, 0))
            {
                this.MouseWheelMoved(mouseWheelDelta);
            }

            if (this._lastWidth != Screen.width || this._lastHeight != Screen.height)
            {
                this._lastWidth = Screen.width;
                this._lastHeight = Screen.height;
                this.ScreenSizeChanged();
            }
        }

        void OnDestroy()
        {
            this.ChangingScenes();
        }

        void OnApplicationQuit()
        {
            this.ApplicationQuit();
        }

        void OnDrawGizmos()
        {
            this.DrawGizmos();
        }

        void OnApplicationFocus(bool newIsFocused)
        {
            if (newIsFocused && !this.IsFocused)
            {
                this.IsFocused = true;
                this.ApplicationGainedFocus();
                this.ApplicationFocusChanged(true);
            }

            if (!newIsFocused && this.IsFocused)
            {
                this.IsFocused = false;
                this.ApplicationLostFocus();
                this.ApplicationFocusChanged(false);
            }
        }
    }
}

#endif
