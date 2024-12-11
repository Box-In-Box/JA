using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    private IPlayerState currentState = null;
    private bool isSceneState = false;

    // 공용 State
    public void SetState<T>() where T : IPlayerState
    {
        if(currentState?.ThisState().GetType().Name == typeof(T).Name) return;

        currentState?.OperateExit();

        if (isSceneState) {
            // 씬 컨트롤러 => Disable
            currentState.ThisComponent().enabled = false;
        }
        else {
            // 공용 컨트롤러 => Destroy
            Destroy(currentState?.ThisComponent());
        }

        currentState = this.gameObject.AddComponent(typeof(T)) as IPlayerState;
        
        currentState.OperateEnter();

        isSceneState = false;
    }
    
    // 씬 State
    public void SetStates<T>(T state) where T : IPlayerState
    {
        if(currentState?.ThisState().GetType().Name == typeof(T).Name) return;

        currentState.OperateExit();

        if (isSceneState) {
            // 씬 컨트롤러 => Disable
            currentState.ThisComponent().enabled = false;
        }
        else {
            // 공용 컨트롤러 => Destroy
            Destroy(currentState?.ThisComponent());
        }

        currentState = state;
        
        currentState.OperateEnter();

        isSceneState = true;
    }
}
