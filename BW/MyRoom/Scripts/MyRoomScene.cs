using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gongju.Web;

public class MyRoomScene : MonoBehaviour
{
    [SerializeField] private Button myRoomButton;
    [field : SerializeField, ReadOnly] public string myRoomTargetNickname { get; set; }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => DatabaseConnector.instance != null);
        yield return new WaitUntil(() => DatabaseConnector.instance.memberUUID > 0);

        myRoomTargetNickname = DatabaseConnector.instance.memberData.nickname;
        myRoomButton.onClick.AddListener(() => GoToMyRoomPopup());
    }

    private void GoToMyRoomPopup()
    {
        PopupManager.instance.Popup($"{myRoomTargetNickname}님의 마이룸에 입장하시겠습니까?", () => {
            SceneLoadManager.instance.LoadScene("MyRoom");
            PhotonNetworkManager.instance.JoinRoom(PhotonNetworkManager.instance.GetMyRoomChannel());
        });
    }
}