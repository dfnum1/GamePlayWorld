using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Framework.RtgTween
{
    [UI.UIWidgetExport]
    public class TweenerAlpha : MonoBehaviour
    {
        [SerializeField]
        private float duration = 0.5f;
        //[SerializeField]
        //private float delay;
        [SerializeField]
        [Range(0, 1)]
        private float from;
        [SerializeField]
        [Range(0, 1)]
        private float to;

        //[SerializeField]
        //private bool loop;
        //[SerializeField]
        //private bool pingpong;

        Graphic[] graphics;
        void Start()
        {
            graphics = transform.GetComponentsInChildren<Graphic>();
        }

        private bool isPlay = false;
        private float speed;
        private float tm;
        private float data = 0;
        private Action callback = null;
        private bool reverse = false;
        public void Play(Action CallBack = null)
        {
            reverse = false;
            play(CallBack);
        }

        public void PlayReverse(Action CallBack = null)
        {
            reverse = true;
            play(CallBack);
            return;
        }

        private void play(Action CallBack = null)
        {
            if (graphics.Length <= 0)
            {
                CallBack?.Invoke();
                CallBack = null;
                return;
            }
            callback = CallBack;
            if (duration <= 0) duration = 0.1f;
            speed = (to - from) / duration;
            tm = 0;

            for (int i = 0; i < graphics.Length; i++)
            {
                Color color = graphics[i].color;
                color.a = reverse ? to : from;
                graphics[i].color = color;
            }
            isPlay = true;
            return;
        }


        void Update()
        {
            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    Play();
            //    return;
            //}
            //if (Input.GetKeyDown(KeyCode.A))
            //{
            //    PlayReverse();
            //    return;
            //}
            if (!isPlay) return;
            tm += Time.deltaTime;
            if (!reverse)
            {
                data = from + speed * tm;
            }
            else
            {
                data = to - speed * tm;
            }

            if (tm >= duration)
            {
                isPlay = false;
                data = !reverse ? to : from;
                callback?.Invoke();
                callback = null;
            }
            for (int i = 0; i < graphics.Length; i++)
            {
                Color color = graphics[i].color;
                color.a = data;
                graphics[i].color = color;
            }

        }
    }

}