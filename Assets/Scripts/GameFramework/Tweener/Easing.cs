/********************************************************************
生成日期:	1:11:2020 13:16
类    名: 	Easing
作    者:	HappLI
描    述:	晃动效果
*********************************************************************/
using UnityEngine;

namespace Framework.RtgTween
{
    public interface RtgEasing
    {
        float EaseIn(float t, float b, float c, float d);
        float EaseOut(float t, float b, float c, float d);
        float EaseInOut(float t, float b, float c, float d);
    }
    //------------------------------------------------------
    public struct RtgBack : RtgEasing
    {
        public float EaseIn(float t, float b, float c, float d)
        {
            float s = 1.70158f;
            float postFix = t /= d;
            return c * (postFix) * t * ((s + 1) * t - s) + b;
        }
        //------------------------------------------------------
        public float EaseOut(float t, float b, float c, float d)
        {
            float s = 1.70158f;
            return c * ((t = t / d - 1) * t * ((s + 1) * t + s) + 1) + b;
        }
        //------------------------------------------------------
        public float EaseInOut(float t, float b, float c, float d)
        {
            float s = 1.70158f;
            if ((t /= d / 2) < 1) return c / 2 * (t * t * (((s *= (1.525f)) + 1) * t - s)) + b;
            float postFix = t -= 2;
            return c / 2 * ((postFix) * t * (((s *= (1.525f)) + 1) * t + s) + 2) + b;
        }
    }
    //------------------------------------------------------
    public struct RtgBounce : RtgEasing
    {
        public float EaseIn(float t, float b, float c, float d)
        {
            return c - EaseOut(d - t, 0, c, d) + b;
        }
        //------------------------------------------------------
        public float EaseOut(float t, float b, float c, float d)
        {
            if ((t /= d) < (1 / 2.75f))
            {
                return c * (7.5625f * t * t) + b;
            }
            else if (t < (2 / 2.75f))
            {
                float postFix = t -= (1.5f / 2.75f);
                return c * (7.5625f * (postFix) * t + .75f) + b;
            }
            else if (t < (2.5 / 2.75))
            {
                float postFix = t -= (2.25f / 2.75f);
                return c * (7.5625f * (postFix) * t + .9375f) + b;
            }
            else
            {
                float postFix = t -= (2.625f / 2.75f);
                return c * (7.5625f * (postFix) * t + .984375f) + b;
            }
        }
        //------------------------------------------------------
        public float EaseInOut(float t, float b, float c, float d)
        {
            if (t < d / 2) return EaseIn(t * 2, 0, c, d) * .5f + b;
            else return EaseOut(t * 2 - d, 0, c, d) * .5f + c * .5f + b;
        }
    }
    //------------------------------------------------------
    public struct RtgCirc : RtgEasing
    {
        public float EaseIn(float t, float b, float c, float d)
        {
            return -c * (Mathf.Sqrt(1 - (t /= d) * t) - 1) + b;
        }
        //------------------------------------------------------
        public float EaseOut(float t, float b, float c, float d)
        {
            return c * Mathf.Sqrt(1 - (t = t / d - 1) * t) + b;
        }
        //------------------------------------------------------
        public float EaseInOut(float t, float b, float c, float d)
        {
            if ((t /= d / 2) < 1) return -c / 2 * (Mathf.Sqrt(1 - t * t) - 1) + b;
            return c / 2 * (Mathf.Sqrt(1 - t * (t -= 2)) + 1) + b;
        }
    }
    //------------------------------------------------------
    public struct RtgCubic : RtgEasing
    {
        public float EaseIn(float t, float b, float c, float d)
        {
            return c * (t /= d) * t * t + b;
        }
        //------------------------------------------------------
        public float EaseOut(float t, float b, float c, float d)
        {
            return c * ((t = t / d - 1) * t * t + 1) + b;
        }
        //------------------------------------------------------
        public float EaseInOut(float t, float b, float c, float d)
        {
            if ((t /= d / 2) < 1) return c / 2 * t * t * t + b;
            return c / 2 * ((t -= 2) * t * t + 2) + b;
        }
    }
    //------------------------------------------------------
    public struct RtgElastic : RtgEasing
    {
        public float EaseIn(float t, float b, float c, float d)
        {
            if (t == 0) return b; if ((t /= d) == 1) return b + c;
            float p = d * .3f;
            float a = c;
            float s = p / 4;
            float postFix = (float)(a * Mathf.Pow(2, 10 * (t -= 1))); // this is a fix, again, with post-increment operators
            return -(postFix * (float)Mathf.Sin((t * d - s) * (2 * Mathf.PI) / p)) + b;
        }
        //------------------------------------------------------
        public float EaseOut(float t, float b, float c, float d)
        {
            if (t == 0) return b; if ((t /= d) == 1) return b + c;
            float p = d * .3f;
            float a = c;
            float s = p / 4;
            return (float)(a * Mathf.Pow(2, -10 * t) * Mathf.Sin((t * d - s) * (2 * Mathf.PI) / p) + c + b);
        }
        //------------------------------------------------------
        public float EaseInOut(float t, float b, float c, float d)
        {
            if (t == 0) return b; if ((t /= d / 2) == 2) return b + c;
            float p = d * (.3f * 1.5f);
            float a = c;
            float s = p / 4;

            if (t < 1)
            {
                float postFix1 = (float)(a * Mathf.Pow(2, 10 * (t -= 1))); // postIncrement is evil
                return -.5f * (postFix1 * (float)Mathf.Sin((t * d - s) * (2 * Mathf.PI) / p)) + b;
            }
            float postFix = (float)(a * Mathf.Pow(2, -10 * (t -= 1))); // postIncrement is evil
            return (float)(postFix * Mathf.Sin((t * d - s) * (2 * Mathf.PI) / p) * .5f + c + b);
        }
    }
    //------------------------------------------------------
    public struct RtgExpo : RtgEasing
    {
        public float EaseIn(float t, float b, float c, float d)
        {
            return (float)((t == 0) ? b : c * Mathf.Pow(2, 10 * (t / d - 1)) + b);
        }
        //------------------------------------------------------
        public float EaseOut(float t, float b, float c, float d)
        {
            return (float)((t == d) ? b + c : c * (-Mathf.Pow(2, -10 * t / d) + 1) + b);
        }
        //------------------------------------------------------
        public float EaseInOut(float t, float b, float c, float d)
        {
            if (t == 0) return b;
            if (t == d) return b + c;
            if ((t /= d / 2) < 1) return (float)(c / 2 * Mathf.Pow(2, 10 * (t - 1)) + b);
            return (float)(c / 2 * (-Mathf.Pow(2, -10 * --t) + 2) + b);
        }
    }
    //------------------------------------------------------
    public struct RtgQuad : RtgEasing
    {
        public float EaseIn(float t, float b, float c, float d)
        {
            return c * (t /= d) * t + b;
        }
        //------------------------------------------------------
        public float EaseOut(float t, float b, float c, float d)
        {
            return -c * (t /= d) * (t - 2) + b;
        }
        //------------------------------------------------------
        public float EaseInOut(float t, float b, float c, float d)
        {
            if ((t /= d / 2) < 1) return ((c / 2) * (t * t)) + b;
            return -c / 2 * (((t - 2) * (--t)) - 1) + b;
        }
    }
    //------------------------------------------------------
    public struct RtgQuart : RtgEasing
    {
        public float EaseIn(float t, float b, float c, float d)
        {
            return c * (t /= d) * t * t * t + b;
        }
        //------------------------------------------------------
        public float EaseOut(float t, float b, float c, float d)
        {
            return -c * ((t = t / d - 1) * t * t * t - 1) + b;
        }
        //------------------------------------------------------
        public float EaseInOut(float t, float b, float c, float d)
        {
            if ((t /= d / 2) < 1) return c / 2 * t * t * t * t + b;
            return -c / 2 * ((t -= 2) * t * t * t - 2) + b;
        }
    }
    //------------------------------------------------------
    public struct RtgQuint : RtgEasing
    {
        public float EaseIn(float t, float b, float c, float d)
        {
            return c * (t /= d) * t * t * t * t + b;
        }
        //------------------------------------------------------
        public float EaseOut(float t, float b, float c, float d)
        {
            return c * ((t = t / d - 1) * t * t * t * t + 1) + b;
        }
        //------------------------------------------------------
        public float EaseInOut(float t, float b, float c, float d)
        {
            if ((t /= d / 2) < 1) return c / 2 * t * t * t * t * t + b;
            return c / 2 * ((t -= 2) * t * t * t * t + 2) + b;
        }
    }
    //------------------------------------------------------
    public struct RtgSine : RtgEasing
    {
        public float EaseIn(float t, float b, float c, float d)
        {
            return -c * (float)Mathf.Cos(t / d * (Mathf.PI / 2)) + c + b;
        }
        //------------------------------------------------------
        public float EaseOut(float t, float b, float c, float d)
        {
            return c * (float)Mathf.Sin(t / d * (Mathf.PI / 2)) + b;
        }
        //------------------------------------------------------
        public float EaseInOut(float t, float b, float c, float d)
        {
            return -c / 2 * (float)(Mathf.Cos(Mathf.PI * t / d) - 1) + b;
        }
    }
    //------------------------------------------------------
    public struct RtgLinear : RtgEasing
    {
        public float EaseIn(float t, float b, float c, float d)
        {
            return c * t / d + b;
        }
        //------------------------------------------------------
        public float EaseOut(float t, float b, float c, float d)
        {
            return c * t / d + b;
        }
        //------------------------------------------------------
        public float EaseInOut(float t, float b, float c, float d)
        {
            return c * t / d + b;
        }
    }
}

