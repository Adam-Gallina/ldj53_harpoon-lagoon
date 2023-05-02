using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUI : MonoBehaviour
{
    [SerializeField] private Vector3 menuCamRot;
    [SerializeField] private Vector3 startCamRot;

    void Start()
    {
        CameraController.Instance.SetTarget(BoatController.Instance.transform);
        CameraController.Instance.SetFixedRotation(true, menuCamRot, 0);
        BoatController.Instance.SetBoatMode(BoatMode.Frozen);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void PressStartBtn()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        CameraController.Instance.SetFixedRotation(true, startCamRot, Constants.FixedRotationDur * 2);
        Invoke(nameof(StartPlayer), Constants.FixedRotationDur * 2);
        BoatController.Instance.GetComponent<AudioSource>().Play();
    }

    private void StartPlayer()
    {
        CameraController.Instance.StopFixedRotation();
        BoatController.Instance.SetBoatMode(BoatMode.Normal);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
