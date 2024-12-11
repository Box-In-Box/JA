using System;
using UnityEngine;
using System.Reflection;
using BW;

public class AnimationStateMachine : StateMachineBehaviour
{   
    public Action<AnimatorStateInfo> OnStateEnterAction;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 새로운 상태로 변할 때 실행
        OnStateEnterAction?.Invoke(stateInfo);
    }

    public Action<AnimatorStateInfo> OnStateUpdateAction;
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 처음과 마지막 프레임을 제외한 각 프레임 단위로 실행
        OnStateUpdateAction?.Invoke(stateInfo);
    }

    public Action<AnimatorStateInfo> OnStateExitAction;
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 상태가 다음 상태로 바뀌기 직전에 실행
        OnStateExitAction?.Invoke(stateInfo);
    }

    public Action<AnimatorStateInfo> OnStateMoveAction;
    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // MonoBehaviour.OnAnimatorMove 직후에 실행
        OnStateMoveAction?.Invoke(stateInfo);
    }

    public Action<AnimatorStateInfo> OnStateIKAction;
    public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // MonoBehaviour.OnAnimatorIK 직후에 실행
        OnStateIKAction?.Invoke(stateInfo);
    }

    public Action OnStateMachineEnterrAction;
    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        // 스크립트가 부착된 상태 기계로 전환이 왔을때 실행
        OnStateMachineEnterrAction?.Invoke();
    }

    public Action OnStateMachineExitAction;
    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        // 스크립트가 부착된 상태 기계에서 빠져나올때 실행
        OnStateMachineExitAction?.Invoke();
    }
}