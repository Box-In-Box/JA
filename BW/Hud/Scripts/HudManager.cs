using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HudManager : MonoBehaviour
{
    [SerializeField] private View_HudPanel view_hudPanel;
    public List<HudUI> hudList { get; set; } = new List<HudUI>();

    private void Awake()
    {
        if (view_hudPanel == null)
        {
            view_hudPanel = GetComponent<View_HudPanel>();
        }
    }

    public HudUI AddHud(HudTarget hudTarget)
    {
        HudUI hudUi = null;
        
        if (hudTarget is HudTarget_Player)
        {
            hudUi = Instantiate(Resources.Load<HudUI>("Hud_Player"), view_hudPanel.PlayerHudPanel);
        }
        else if (hudTarget is HudTarget_NPC)
        {
            hudUi = Instantiate(Resources.Load<HudUI>("Hud_NPC"), view_hudPanel.NPCHudPanel);
        }
        else if (hudTarget is HudTarget_Sign)
        {
            hudUi = Instantiate(Resources.Load<HudUI>("Hud_Sign"), view_hudPanel.SignHudPanel);
        }
        else if (hudTarget is HudTarget_Chair)
        {
            hudUi = Instantiate(Resources.Load<HudUI>("Hud_Chair"), view_hudPanel.ChairHudPanel);
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
            
            Vector3 screenPos = Camera.main.WorldToScreenPoint(hud.HudTarget.GetPosition());

            if (screenPos.z > 0f)
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