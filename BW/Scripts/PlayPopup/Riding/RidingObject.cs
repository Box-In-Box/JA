using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonObject))]
public class RidingObject : MonoBehaviour, IWheel
{
    private Animator animator;
    private Animator playerAnimator;

    [field : SerializeField] public Transform[] wheels { get; set; }
    [field : SerializeField] public float wheelSpeed  { get; set; } = 10f;

    private string ownerName = "";
    private PhotonPlayerData photonPlayerData;
    private bool isMine = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        isMine = this.GetComponent<PhotonObject>().photonView.IsMine;
    }

    private void OnEnable()
    {
        this.GetComponent<PhotonObject>().Subscribe_OnPhotonInstantiate(GetPlayer);
        StartCoroutine(GetPlayerCoroutine());
    }

    private void OnDisable()
    {
        this.GetComponent<PhotonObject>().UnSubscribe_OnPhotonInstantiate(GetPlayer);
    }

    private void GetPlayer(PhotonMessageInfo info)
    {
        ownerName = info.Sender.NickName;
    }

    private IEnumerator GetPlayerCoroutine()
    {
        if (SceneLoadManager.instance.targetSceneType == SceneType.Online || SceneLoadManager.instance.targetSceneType == SceneType.Myroom) {
            yield return new WaitUntil(() => ownerName != "");
            yield return new WaitUntil(() => PhotonNetworkManager.instance.photonPlayerDic.TryGetValue(ownerName, out photonPlayerData));
            yield return new WaitUntil(() => photonPlayerData.playerCharacter);
            playerAnimator = PhotonNetworkManager.instance.photonPlayerDic[ownerName].playerCharacter.Animator;
        }
        else if (SceneLoadManager.instance.targetSceneType == SceneType.Offline) {
            Debug.Log("!");
            yield return new WaitUntil(() => GameManager.instance.playerCharacter);
            playerAnimator = GameManager.instance.playerAnimator;
            Debug.Log("!!!");
        }
    }

    public void Update()
    {
        if (playerAnimator == null) return;

        var angleVal = playerAnimator.GetFloat("angleVal");
        var moveVal = playerAnimator.GetFloat("moveVal");

        this.animator.SetFloat("angleVal", angleVal);

        foreach (Transform wheel in wheels) {
            if (isMine && GameManager.instance.playerController.PlayerInputSystem.InputVector.y == 0) return;
            if (SceneLoadManager.instance.targetSceneType == SceneType.Online || SceneLoadManager.instance.targetSceneType == SceneType.Myroom) {
                if (!isMine && this.photonPlayerData.playerMoveState != PlayerMoveState.Move) return;
            }
            wheel.Rotate(new Vector3(0f, 0f, moveVal * wheelSpeed * Time.deltaTime * -360f));
        }
    }  
}