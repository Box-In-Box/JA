using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public string targetScene;
    public string targetSceneName;

    private void OnTriggerEnter(Collider other)
    {
        other.TryGetComponent<PlayerCharacter>(out PlayerCharacter player);

        if (player == GameManager.instance.playerCharacter)
        {
            switch (targetScene)
            {
                case "MyRoom":
                    GoToMyRoom();
                    break;
                default:
                    SceneLoadManager.instance.PopupLoadScene(targetScene, targetSceneName);
                    break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        other.TryGetComponent<PlayerCharacter>(out PlayerCharacter player);

        if (player == GameManager.instance.playerCharacter)
        {
            //PopupManager.instance.PopupClose();
        }
    }

    private void GoToMyRoom()
    {
        PopupManager.instance.Popup("���̷뿡 �����Ͻðڽ��ϱ�?", () => 
        {
            SceneLoadManager.instance.LoadScene("MyRoom");
            PhotonNetworkManager.instance.JoinRoom(PhotonNetworkManager.instance.GetMyRoomChannel());
        });
    }
}