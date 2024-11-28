using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UnityEngine.InputSystem.HID;

public class HudManager : MonoBehaviour
{
    public List<HudUI> hudList { get; set; } = new List<HudUI>();
    [field: SerializeField, Range(10f, 100f)] public float MaxHudVisible { get; set; } = 30f;

    public HudUI AddHud(HudTarget hudTarget)
    {
        HudUI hudUi = null;
        
        if (hudTarget is HudTarget_Player)
        {
            hudUi = Instantiate(Resources.Load<HudUI>("Hud_Player"), Canvas_Scene.instance.View.HudPanel.transform);
        }
        else if (hudTarget is HudTarget_NPC)
        {
            hudUi = Instantiate(Resources.Load<HudUI>("Hud_NPC"), Canvas_Scene.instance.View.HudPanel.transform);
        }
        else if (hudTarget is HudTarget_Sign)
        {
            hudUi = Instantiate(Resources.Load<HudUI>("Hud_Sign"), Canvas_Scene.instance.View.HudPanel.transform);
        }
        else if (hudTarget is HudTarget_Chair)
        {
            hudUi = Instantiate(Resources.Load<HudUI>("Hud_Chair"), Canvas_Scene.instance.View.HudPanel.transform);
        }
        hudUi.HudTarget = hudTarget;
        hudUi.IsActive = true;
        hudUi.IsVisible = false;
        hudUi.Rect_Flexible.localScale = Vector3.zero;
        hudList.Add(hudUi);
        return hudUi;
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
                continue;
            }
            
            Vector3 screenPos = Camera.main.WorldToScreenPoint(hud.HudTarget.GetPosition());

            if (screenPos.z > 0f && screenPos.z < MaxHudVisible)
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

                // Set Position
                hud.Rect_Flexible.position = screenPos;
                hud.Rect_Fixed.position = screenPos;
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