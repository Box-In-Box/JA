using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    public SampleDynamicScrollView scrollView;
    public TMPro.TMP_InputField inputField;

    private void Start()
    {
        List<SampleDynamicScrollViewData> sampleDatas = new List<SampleDynamicScrollViewData>();

        scrollView.Init(sampleDatas);
    }

    public void OnClickAddData()
    {
        scrollView.AddData(new SampleDynamicScrollViewData() { text = inputField.text }, true, true);
    }

    public void OnClickRemoveData()
    {
        scrollView.ResetData();
    }
}
