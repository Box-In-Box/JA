using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public abstract class DynamicScrollView<ScrollViewData, ScrollViewItem> : MonoBehaviour where ScrollViewItem : DynamicScrollViewItem<ScrollViewData> 
{
    private ScrollRect scrollRect;

    private List<ScrollViewData> dataList = new List<ScrollViewData>();
    private List<float> itemSizeList = new List<float>();
    private List<ScrollViewItem> itemListPool = new List<ScrollViewItem>();

    private int currentItem = -1;
    private bool initialized = false;

    public abstract GameObject Prefab { get; set; }
    public abstract float MinSize { get; set; }
    public abstract float Space { get; set; }


    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    public void Init(List<ScrollViewData> datas)
    {
        if (initialized) return;
            
        initialized = true;

        scrollRect.onValueChanged.AddListener(OnValueChanged);

        var anchorMin = scrollRect.vertical ? new Vector2(0.5f, 1.0f) : new Vector2(0, 0.5f);
        var anchorMax = scrollRect.vertical ? new Vector2(0.5f, 1.0f) : new Vector2(0, 0.5f);
        var pivot = scrollRect.vertical ? new Vector2(0.5f, 1.0f) : new Vector2(0.0f, 0.5f);
        scrollRect.content.anchorMin = anchorMin;
        scrollRect.content.anchorMax = anchorMax;
        scrollRect.content.pivot = pivot;

        var count = scrollRect.vertical ? Mathf.CeilToInt(scrollRect.gameObject.GetComponent<RectTransform>().rect.height / (MinSize + Space)) + 1 : Mathf.CeilToInt(scrollRect.gameObject.GetComponent<RectTransform>().rect.width / (MinSize + Space)) + 1;
        float pos = 0;
        for (int i = 0; i < count; i++)
        {
            var obj = Instantiate(Prefab, scrollRect.content);
            var rt = obj.transform.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = pivot;

            rt.anchoredPosition = scrollRect.vertical ? new Vector3(0, -pos, 0) : new Vector3(pos, 0, 0);

            pos = scrollRect.vertical ? pos - MinSize - Space : pos + MinSize + Space;
            itemListPool.Add(obj.GetComponent<ScrollViewItem>());

            obj.SetActive(false);
        }

        foreach (var data in datas)
        {
            AddData(data);
        }

        UpdateScrollViewContentSize();
        UpdateContentsPosition();
    }

    void UpdateScrollViewContentSize()
    {
        scrollRect.content.sizeDelta = scrollRect.vertical ? new Vector2(0, GetPosition(dataList.Count)) : new Vector2(GetPosition(dataList.Count), 0);
    }

    void UpdateContentsPosition(bool forceUpdate = false)
    {
        var pos = scrollRect.vertical ? scrollRect.content.anchoredPosition.y : scrollRect.content.anchoredPosition.x;
        var index = GetIndexByPosition(pos);

        if (currentItem != index || forceUpdate)
        {
            currentItem = index;

            for (int i = 0; i < itemListPool.Count; i++)
            {
                var dataIndex = index + i;
                var poolIndex = GetPoolIndex(dataIndex);

                if (dataIndex >= dataList.Count) break;

                OnUpdate(dataList[dataIndex], itemListPool[poolIndex]);
                MoveItem(dataIndex, poolIndex);
            }
        }
    }

    public void AddData(ScrollViewData data, bool updateSize = false, bool forceMove = false)
    {
        var poolIndex = GetPoolIndex(dataList.Count);

        itemListPool[poolIndex].gameObject.SetActive(true);
        OnUpdate(data, itemListPool[poolIndex]);

        itemSizeList.Add(itemListPool[poolIndex].GetSize(scrollRect.vertical));

        MoveItem(dataList.Count, poolIndex);

        dataList.Add(data);

        if (updateSize)
        {
            UpdateScrollViewContentSize();
            UpdateContentsPosition(true);

            if (forceMove)
            {
                MoveIndex(dataList.Count);
            }  
        }
    }

    public void ResetData()
    {
        foreach (var item in itemListPool)
        {
            item.gameObject.SetActive(false);
        }

        dataList.Clear();
        itemSizeList.Clear();

        UpdateScrollViewContentSize();
        UpdateContentsPosition();
    }

    public void MoveIndex(int index)
    {
        var position = GetPosition(index);
        var adujst = 0.0f;
        if (scrollRect.vertical)
        {
            adujst = scrollRect.content.rect.height > scrollRect.viewport.rect.height ? scrollRect.viewport.rect.height : scrollRect.content.rect.height;
        }
        else
        {
            adujst = scrollRect.content.rect.width > scrollRect.viewport.rect.width ? scrollRect.viewport.rect.width : scrollRect.content.rect.width;
        } 
        scrollRect.content.anchoredPosition = scrollRect.vertical ? new Vector2(0, position - adujst) : new Vector2(-position + adujst, 0);
    }

    public void MoveItem(int index, int poolIndex)
    {
        var position = GetPosition(index);
        itemListPool[poolIndex].gameObject.GetComponent<RectTransform>().anchoredPosition = scrollRect.vertical ? new Vector3(0, -position, 0) : new Vector3(position, 0, 0);
    }

    int GetPoolIndex(int dataIndex)
    {
        return dataIndex > 0 ? dataIndex % itemListPool.Count : 0;
    }

    int GetIndexByPosition(float position)
    {
        int index = 0;
        float sum = 0;

        if (scrollRect.vertical)
        {
            for (int i = 0; i < itemSizeList.Count; i++)
            {
                sum = sum + itemSizeList[i] + Space;
                if (position < sum)
                {
                    index = i; 
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < itemSizeList.Count; i++)
            {
                sum = sum + itemSizeList[i] + Space;
                if (position > sum * -1)
                {
                    index = i;
                    break;
                }
            }
        }

        return index;
    }

    float GetPosition(int index)
    {
        float sum = 0;

        for (int i = 0; i < index; i++)
        {
            sum += itemSizeList[i] + Space;
        } 
        return sum;
    }

    protected virtual void OnUpdate(ScrollViewData data, ScrollViewItem item)
    {
        item.OnUpdate(data);

        if (scrollRect.vertical)
        {
            item.SizeFitter.SetLayoutVertical();
        }
        else
        {
            item.SizeFitter.SetLayoutVertical();
        }   
    }

    void OnValueChanged(Vector2 value)
    {
        UpdateContentsPosition();
    }
}
