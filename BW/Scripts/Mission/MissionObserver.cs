using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MissionObserver : MonoBehaviour
{
    public abstract void Normal(int missionId); // Normal 기본 혹은 진행 전
    public abstract void Selected(int missionId); // 코스 모드에서 진행중
    public abstract void Completed(int missionId); // 진행 완료 보상 전
    public abstract void Finished(int missionId); // 진행 완료 보상 후
}