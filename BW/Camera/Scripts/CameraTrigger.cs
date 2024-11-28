using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Photon.Realtime;
using Sirenix.OdinInspector;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class CameraTrigger : MonoBehaviour
{
    private CameraController cameraController;
    private PlayerCharacter playerCharacter;
    [SerializeField] private CinemachineVirtualCamera cinemachineCamera;
    [SerializeField] private CinemachineDollyCart cinemachineDollyCart;
    [SerializeField] private GameObject offsetTarget;
    [SerializeField] private Vector3 offset;

    private void Start()
    {
        var transposer = cinemachineCamera.GetCinemachineComponent<CinemachineComposer>();
        Vector3 obejctOffset = Vector3.zero;

        if (offsetTarget == null)
        {
            offsetTarget = this.gameObject;
        }

        if (offsetTarget.transform.parent.TryGetComponent<MeshRenderer>(out MeshRenderer mesh))
        {
            obejctOffset = new Vector3(0, mesh.bounds.size.y / 2, 0);
        }
        else if (offsetTarget.transform.parent.TryGetComponent<Collider>(out Collider col))
        {
            obejctOffset = new Vector3(0, col.bounds.size.y / 2, 0);
        }
        else if (offsetTarget.transform.TryGetComponent<MeshRenderer>(out MeshRenderer mesh2))
        {
            obejctOffset = new Vector3(0, mesh2.bounds.size.y / 2, 0);
        }
        else if (offsetTarget.transform.TryGetComponent<Collider>(out Collider col2))
        {
            obejctOffset = new Vector3(0, col2.bounds.size.y / 2, 0);
        }

        if (transposer != null)
        {
            transposer.m_TrackedObjectOffset = obejctOffset + offset;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (cameraController == null) cameraController = GameManager.instance.PlayerCameraController;
        if (playerCharacter == null) playerCharacter = GameManager.instance.playerCharacter;

        other.TryGetComponent<PlayerCharacter>(out PlayerCharacter player);

        if (player == playerCharacter)
        {
            if (!cameraController.cameraTriggerList.Contains(this))
            {
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

        if (player == playerCharacter)
        {
            if (cameraController.cameraTriggerList.Contains(this)) 
            {
                cameraController.cameraTriggerList.Remove(this);
            }
            CameraSelect();
        }
    }

    private void CameraSelect()
    {
        if (cameraController.cameraTriggerList.Count == 0)
        {
            SetFreeLookCameraRotation();
        }
        else
        {
            cameraController.cameraTriggerList[0].SetDollyCartVirtualCameraRotation();
        }
    }

    private void OnDisable()
    {
        if (GameManager.instance && GameManager.instance.PlayerCameraController && cameraController)
        {
            if (cameraController.cameraTriggerList.Contains(this))
            {
                cameraController.cameraTriggerList.Remove(this);
            }
            CameraSelect();
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