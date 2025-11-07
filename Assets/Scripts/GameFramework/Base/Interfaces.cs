using System;
using UnityEngine;
namespace Framework.Core
{
    public interface IUpdate
    {
        void Update(float fFrame);
    }
    public interface IFixedUpdate
    {
        void FixedUpdate(float fFrame);
    }
    public interface ILateUpdate
    {
        void LateUpdate(float fFrame);
    }
    public interface ITouchInput
    {
        void OnTouchBegin(ATouchInput.TouchData touch);
        void OnTouchMove(ATouchInput.TouchData touch);
        void OnTouchWheel(float wheel, Vector2 mouse);
        void OnTouchEnd(ATouchInput.TouchData touch);
    }
    public interface IKeyInput
    {
        void OnKeyDown(KeyCode button);
        void OnKeyUp(KeyCode button);
    }
    public interface IDrawGizmos
    {
        void DrawGizmos();
    }
    public interface IPause
    {
        void OnPause(bool bPause);
    }
    public interface IJobUpdate
    {
        bool OnJobUpdate(float fFrame, IUserData userData = null);
        int GetJob();
        void OnJobComplete(IUserData userData = null);
    }
    public interface IThreadJob
    {
        bool OnThreadUpdate(float fFrame, IUserData userData = null);
        int GetThread();
        void OnThreadComplete(IUserData userData = null);
    }
}
