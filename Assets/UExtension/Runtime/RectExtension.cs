using UnityEngine;

namespace UExtension
{
    public static class RectExtension
    {
        public static Rect SplitByWidth(ref Rect rRect, bool isLeft, float fWidth, float fPadding)
        {
            var newRect = rRect;
            if (isLeft)
            {
                newRect.width = fWidth;
                rRect.width -= fWidth + fPadding;
                rRect.x += fWidth + fPadding;
            }
            else
            {
                newRect.x += rRect.width - fWidth;
                newRect.width = fWidth;
                rRect.width -= fWidth + fPadding;
            }
            return newRect;
        }
        public static Rect SplitByHeight(ref Rect rRect, bool isTop, float fHeight, float fPadding)
        {
            var newRect = rRect;
            if (isTop)
            {
                newRect.height = fHeight;
                rRect.height -= fHeight + fPadding;
                rRect.y += fHeight + fPadding;
            }
            else
            {
                newRect.y += rRect.width - fHeight;
                newRect.height = fHeight;
                rRect.height -= fHeight + fPadding;
            }
            return newRect;
        }
    }
}