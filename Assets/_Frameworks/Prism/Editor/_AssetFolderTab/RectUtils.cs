using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Prism
{
    public class RectUtils
    {
        /*private static Rect baseRect = new Rect(0,0,0,0);

        public static Rect GetBaseRect()
        {
            return baseRect;
        }

        public static void SetRange(float x, float y, float width, float height)
        {
            baseRect = new Rect(x, y, width, height);
        }
        public static Rect AdjustRect(float x, float xMax, float y, float yMax)
        {
            var r = new Rect();
            r.x = x >= 0 ? baseRect.x + x : baseRect.xMax + x;
            r.y = y >= 0 ? baseRect.y + y : baseRect.yMax + y;
            r.xMax = xMax > 0 ? baseRect.x + xMax : baseRect.xMax + xMax;
            r.yMax = yMax > 0 ? baseRect.y + yMax : baseRect.yMax + yMax;
            return r;
        }*/

        public static void DrawOutline(Rect rect, float size)
        {
            Color color = new Color(0.6f, 0.6f, 0.6f, 1.333f);
            if (EditorGUIUtility.isProSkin)
            {
                color.r = 0.12f;
                color.g = 0.12f;
                color.b = 0.12f;
            }

            if (Event.current.type != EventType.repaint)
                return;

            Color orgColor = GUI.color;
            GUI.color = GUI.color * color;
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, size), EditorGUIUtility.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.yMax - size, rect.width, size), EditorGUIUtility.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.y + 1, size, rect.height - 2 * size), EditorGUIUtility.whiteTexture);
            GUI.DrawTexture(new Rect(rect.xMax - size, rect.y + 1, size, rect.height - 2 * size), EditorGUIUtility.whiteTexture);

            GUI.color = orgColor;
        }
    }
}