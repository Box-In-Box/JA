using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEngine.InputSystem.HID;
using Sirenix.OdinInspector;

public class HudManager : MonoBehaviour
{
    public List<HudUI> hudList { get; set; } = new List<HudUI>();
    [field: SerializeField, Range(10f, 100f)] public float MaxHudVisibleDistance { get; set; } = 30f;

    public HudUI AddHud(HudTarget hudTarget)
    {
        HudUI hudUI = null;
        if (hudTarget is HudTarget_Player)
        {
            Instantiate(PopupManager.instance.HudUIPrefab.Player, Canvas_Scene.instance.View.HudPanel.transform).TryGetComponent<HudUI>(out hudUI);
        }
        else if (hudTarget is HudTarget_NPC)
        {
            Instantiate(PopupManager.instance.HudUIPrefab.NPC, Canvas_Scene.instance.View.HudPanel.transform).TryGetComponent<HudUI>(out hudUI);
        }
        else if (hudTarget is HudTarget_Sign)
        {
            Instantiate(PopupManager.instance.HudUIPrefab.Sign, Canvas_Scene.instance.View.HudPanel.transform).TryGetComponent<HudUI>(out hudUI);
        }
        else if (hudTarget is HudTarget_Chair)
        {
            Instantiate(PopupManager.instance.HudUIPrefab.Chair, Canvas_Scene.instance.View.HudPanel.transform).TryGetComponent<HudUI>(out hudUI);
        }
        else if (hudTarget is HudTarget_MinigameTitle)
        {
            Instantiate(PopupManager.instance.HudUIPrefab.Minigame, Canvas_Scene.instance.View.HudPanel.transform).TryGetComponent<HudUI>(out hudUI);
        }

        if (hudUI != null)
        {
            hudUI.HudTarget = hudTarget;
            hudUI.IsActive = true;
            hudUI.IsVisible = false;
            hudUI.Rect_Flexible.localScale = Vector3.zero;
            hudList.Add(hudUI);
        }
        return hudUI;
    }

    public void RemoveHud(HudUI hud)
    {
        if (hudList.Contains(hud))
        {
            hudList.Remove(hud);
            Destroy(hud.gameObject);
        }
    }

    private void Update()
    {
        if (hudList.Count == 0) return;

        for (int i = hudList.Count - 1; i >= 0; i--)
        {
            var hud = hudList[i];

            if (hud.HudTarget == null)
            {
                RemoveHud(hud);
                continue;
            }
            else if (!hud.HudTarget.gameObject.activeInHierarchy)
            {
                if (hud.IsActive) ActiveOff(hud);
            }

            Vector3 screenPos = Camera.main.WorldToScreenPoint(hud.HudTarget.GetPosition());
            hud.Rect_Flexible.position = screenPos;
            hud.Rect_Fixed.position = screenPos;

            if (screenPos.z > 0f && screenPos.z < MaxHudVisibleDistance)
            {
                float offsetWidth = hud.Rect_Flexible.rect.size.x;
                float offsetHeight = hud.Rect_Flexible.rect.size.y;
                bool isInWidth = screenPos.x > 0f - offsetWidth && screenPos.x <= Screen.width + offsetWidth;
                bool isInHeight = screenPos.y > 0f - offsetHeight && screenPos.y <= Screen.height + offsetHeight;

                if (isInWidth && isInHeight) // On
                {
                    if (!hud.IsActive && hud.IsVisible) ActiveOn(hud);
                    else if (hud.IsActive && !hud.IsVisible) ActiveOff(hud);

                    if (!hud.Rect_Fixed.gameObject.activeSelf) hud.Rect_Fixed.gameObject.SetActive(true);
                }
                else // Off
                {
                    if (hud.IsActive) ActiveOff(hud);
                }
            }
            else // Off
            {
                if (hud.IsActive) ActiveOff(hud);
                if (hud.Rect_Fixed.gameObject.activeSelf) hud.Rect_Fixed.gameObject.SetActive(false);
            }
        }
        SortByDistanceToCamera();
    }

    public void SortByDistanceToCamera()
    {
        var hudPanel = Canvas_Scene.instance.View.HudPanel;
        if (hudPanel.transform == null || hudPanel.transform.childCount <= 1) return;

        Transform[] children = new Transform[hudPanel.transform.childCount];
        for (int i = 0; i < hudPanel.transform.childCount; i++)
        {
            children[i] = hudPanel.transform.GetChild(i);
        }

        Vector3 cameraPos = Camera.main.transform.position;

        hudList.Sort((a, b) =>
        {
            float distanceA = Vector3.Distance(cameraPos, a.HudTarget.GetPosition());
            float distanceB = Vector3.Distance(cameraPos, b.HudTarget.GetPosition());
            return distanceA.CompareTo(distanceB);
        });

        for (int i = 0; i < hudList.Count; i++)
        {
            Transform hudTransform = children.FirstOrDefault(child => child == hudList[i].transform);
            if (hudTransform != null)
            {
                hudTransform.SetSiblingIndex(children.Length - 1 - i);
            }
        }
    }

    public void ActiveOn(HudUI hud)
    {
        hud.Rect_Flexible.DOKill();
        hud.Rect_Flexible.localScale = new Vector3(.8f, .8f, .8f);
        hud.Rect_Flexible.DOScale(1f, .5f).SetEase(Ease.OutBack);
        hud.IsActive = true;
    }

    public void ActiveOff(HudUI hud)
    {
        hud.Rect_Flexible.DOKill();
        hud.Rect_Flexible.DOScale(0f, .5f).SetEase(Ease.InBack);
        hud.IsActive = false;
    }
}