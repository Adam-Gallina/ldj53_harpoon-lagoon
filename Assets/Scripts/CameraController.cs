using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [SerializeField] private Transform camOffset;

    [Header("Camera Movement")]
    [SerializeField] private float minMoveDist;
    [SerializeField] private float minDist;
    [SerializeField] private float maxDist;
    [SerializeField] private float moveSpeed;
    private bool moving;
    private bool rotationFixed = false;
    private Vector3 targetRot;
    private float fixedRotDur;
    private bool anim = false;

    [Header("Camera Rotation")]
    [SerializeField] private float speedH = 2.0f;
    [SerializeField] private float speedV = 2.0f;
    private float yaw = 0.0f;
    private float pitch = 45;

    private bool camLocked = false;

    private Transform target;

    private void Awake()
    {
        Instance = this;
    }


    void Update()
    {
        transform.position = UpdatePosition();
        if (!anim && !camLocked)
            transform.eulerAngles = UpdateRotation();
    }

    private Vector3 UpdatePosition()
    {
        if (target)
        {
            return target.position;
            float dist = Vector3.Distance(transform.position, target.position);

            if (dist > minDist)
                moving = true;

            if (moving)
            {
                if (dist > maxDist)
                    return target.position + (transform.position - transform.position) * maxDist;

                if (dist < minMoveDist)
                    moving = false;
                else
                    return Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            }
        }

        return transform.position;
    }

    private Vector3 UpdateRotation()
    {
        if (rotationFixed)
        {
            Vector3 rot = targetRot + new Vector3(0, target.eulerAngles.y, 0);
            pitch = rot.x;  
            yaw = rot.y;
            return rot;
        }

        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");

        if (pitch > 90)
            pitch = 90;
        else if (pitch < 5)
            pitch = 5;

        return new Vector3(pitch, yaw, 0);
    }

    public void SetTarget(Transform t)
    {
        target = t;
    }

    public void SetCamLock(bool locked)
    {
        camLocked = locked;
    }

    public void StopFixedRotation() { SetFixedRotation(false, new Vector3(pitch, yaw, 0), fixedRotDur); }
    public void SetFixedRotation(bool rotationFixed, Vector3 rotation, float animDuration)
    {
        this.rotationFixed = rotationFixed;
        targetRot = rotation;
        fixedRotDur = animDuration;

        if (rotationFixed)
            StartCoroutine(FixedRotAnim(rotation + new Vector3(0, target.eulerAngles.y, 0), animDuration));
    }

    private IEnumerator FixedRotAnim(Vector3 targetRot, float duration)
    {
        anim = true;

        float start = Time.time;
        Quaternion startRot = transform.rotation;
        float t;
        do {
            t = (Time.time - start) / duration;
            
            transform.rotation = Quaternion.Lerp(startRot, Quaternion.Euler(targetRot), t);

            yield return null;
        } while (t < 1);

        transform.eulerAngles = targetRot;
        pitch = targetRot.x;
        yaw = targetRot.y; 

        anim = false;
    }

    public void ClearCameraOffset() { SetCameraOffset(Vector3.zero); }
    public void SetCameraOffset(Vector3 offset)
    {
        camOffset.transform.localPosition = offset;
    }
}
