using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class CameraTrigger : MonoBehaviour
{
    private CameraController cameraController;
    private PlayerCharacter playerCharacter;
    [SerializeField] private CinemachineVirtualCamera cinemachineCamera;
    [SerializeField] private CinemachineDollyCart cinemachineDollyCart;

    private void Start()
    {
        var transposer = cinemachineCamera.GetCinemachineComponent<CinemachineComposer>();
        Vector3 offset = Vector3.zero;

        if (this.transform.parent.TryGetComponent<MeshRenderer>(out MeshRenderer mesh))
        {
            offset = new Vector3(0, mesh.bounds.size.y / 2, 0);
        }
        else if (this.transform.parent.TryGetComponent<Collider>(out Collider col))
        {
            offset = new Vector3(0, col.bounds.size.y / 2, 0);
        }

        if (transposer != null)
        {
            transposer.m_TrackedObjectOffset = offset;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (cameraController == null) cameraController = GameManager.instance.PlayerCameraController;
        if (playerCharacter == null) playerCharacter = GameManager.instance.playerCharacter;

        other.TryGetComponent<PlayerCharacter>(out PlayerCharacter player);

        if (player == playerCharacter) {
            if (!cameraController.cameraTriggerList.Contains(this)) {
                cameraController.cameraTriggerList.Add(this);
            }
            SetDollyCartVirtualCameraRotation();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (cameraController == null) cameraController = GameManager.instance.PlayerCameraController;
        if (playerCharacter == null) playerCharacter = GameManager.instance.playerCharacter;

        other.TryGetComponent<PlayerCharacter>(out PlayerCharacter player);

        if (player == playerCharacter) {
            if (cameraController.cameraTriggerList.Contains(this)) {
                cameraController.cameraTriggerList.Remove(this);
            }
            
            if (cameraController.cameraTriggerList.Count == 0) {
                SetFreeLookCameraRotation();
            }
            else {
                cameraController.cameraTriggerList[0].SetDollyCartVirtualCameraRotation();
            }
        }
    }

#region FreeLook To Virtual
    public void SetDollyCartVirtualCameraRotation()
    {
        cinemachineDollyCart.m_Position = FreeLookRotationToVirtual();

        cinemachineCamera.gameObject.SetActive(false);
        cinemachineCamera.transform.position = cinemachineDollyCart.transform.position;
        cinemachineCamera.gameObject.SetActive(true);

        cameraController.SwitchCamera(cinemachineCamera);
    }

    private float FreeLookRotationToVirtual()
    {
        float angle = cameraController.playerCamera.freeLook.m_XAxis.Value;
        float trackRtation = transform.rotation.eulerAngles.y / 360f;

        if (angle < 180) return 0.5f + ((180 - angle) / 360f) + trackRtation; // 0 ~ 180
        else return (-angle / 360f) + trackRtation; // 180 ~ 360
    }
#endregion

#region  Virtual To FreeLook
    public void SetFreeLookCameraRotation()
    {
        cameraController.playerCamera.freeLook.m_XAxis.Value = VirtualRotationToFreeLook();
        cameraController.SwitchCameraReturn();
    }

    private float VirtualRotationToFreeLook()
    {
        float angle = transform.rotation.eulerAngles.y;
        float trackRtation = transform.rotation.eulerAngles.y / 360f;

        if (angle < 180) return (-cinemachineDollyCart.m_Position * 360f) + trackRtation;
        else return (cinemachineDollyCart.m_Position * 360f) + trackRtation;
    }
#endregion
}