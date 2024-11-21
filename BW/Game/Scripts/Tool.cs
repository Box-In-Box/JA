using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using System;

namespace BW
{
    public static class Tool
    {
#region UI
        /// <summary>
        /// UI 위에서 클릭
        /// </summary>
        public static bool IsPointerOverUI(int index = -1)
        {
            if (index == -1 && EventSystem.current.IsPointerOverGameObject()) return true;
            
            if (index != -1) {
                PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
                pointerEventData.position = Touch.activeTouches[index].screenPosition;
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerEventData, results);
                if (results.Count > 0) return true;
            }
            return false;
        }

        /// <summary>
        /// InputField 입력중
        /// </summary>
        public static bool IsInputFieldFocused()
        {
            GameObject obj = EventSystem.current.currentSelectedGameObject;
            bool isInputFieldFocused = obj != null && obj.GetComponent<TMP_InputField>() != null;
            return isInputFieldFocused;
        }
#endregion

#region Currency
        public static string GetNumberComma(long value)
        {
            return string.Format("{0: #,###; -#,###;0}",value);
        }

        public static string GetCurrencyText(long value)
        {
            string[] symbol = new string[7]{"K", "M", "G", "T", "P", "E", "Z"};
            string result = value.ToString();
            
            if (result.Length < 4) return GetNumberComma(value);

            for(int i = 0; i < symbol.Length; ++i){
                if( 4 + 3 * i <= result.Length && result.Length < 4 + 3 * ( i + 1 )) {
                    int n = result.Length % 3;
                    n = n == 0 ? 3 : n;
                    result = $"{result.Substring(0, n)}.{result.Substring(n, 1)}";
                    result += symbol[i];
                    break;
                }
            }
            return result;
        }
    }
#endregion
}