using DG.Tweening;
using Gongju.Web;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class Treasure : MonoBehaviour
{
    [field: SerializeField] public int missionId { get; set; }
    [field: SerializeField] public string treasureTitle { get; set; }
    [field: SerializeField, Range(0, 4)] public int index { get; set; }

    [field: Title("[ Item ]")]
    [field: SerializeField] private GameObject treasureObj;
    [field: SerializeField] private ParticleEffect particleEffect;
    private bool isInit = false;


    private IEnumerator Start()
    {
        yield return null;
        if (GameManager.instance.isGuest)
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            MissionManager.instance.treasureMissionManager.treasures.Add(this);
            yield return new WaitUntil(() => MissionManager.instance.isMissionInit);

            var data = MissionManager.instance.treasureMissionManager.treasureDatas.Find(x => x.treasureTitle == this.treasureTitle);
            if (data != null)
            {
                Setting(data.missionId, data.value);
            }
        }
    }

    public void Setting(int missionId, bool[] value)
    {
        if (missionId == this.missionId)
        {
            if (value[index] == true)
            {
                if (!isInit)
                {
                    this.gameObject.SetActive(false);
                    treasureObj.transform.DOKill();
                    isInit = true;
                }
            }
            else
            {
                if (!isInit)
                {
                    this.gameObject.SetActive(true);
                    particleEffect.AddPlayEvent(() => treasureObj.transform.DOKill());
                    particleEffect.AddPlayEvent(() => treasureObj.SetActive(false));
                    particleEffect.AddStopEvent(() => this.gameObject.SetActive(false));
                    Animation();
                    isInit = true;
                }
            }
        }
    }

    public void Animation()
    {
        treasureObj.transform.DOKill();
        treasureObj.transform.DOMoveY(this.transform.position.y - 0.25f, 2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        treasureObj.transform.DORotate(new Vector3(0f, 360f, 0f), 10f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
    }

    private void OnTriggerEnter(Collider other)
    {
        other.TryGetComponent<PlayerCharacter>(out PlayerCharacter player);

        if (player == GameManager.instance.playerCharacter)
        {
            MissionManager.instance.treasureMissionManager.FindTreasure(missionId, index);
            particleEffect.Play();
        }  
    }
}