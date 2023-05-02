using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class Spear : MonoBehaviour
{
    [SerializeField] private Transform model;

    [SerializeField] private float spinSpeed;

    [SerializeField] private float despawnRange;

    private Func<Spear, Transform, bool> OnSpearHit;
    private LayerMask targetLayers;

    private Coroutine spearCast;

    private void OnTriggerEnter(Collider other)
    {
        if ((1 << other.gameObject.layer & targetLayers.value) > 1)
        {
            StopCoroutine(spearCast);

            if (OnSpearHit == null || OnSpearHit?.Invoke(this, other.transform) == true)
                Destroy(gameObject);
        }
    }

    public void StopCastCoroutine()
    {
        StopCoroutine(spearCast);

        OnSpearHit = null;
        targetLayers = 0;
    }

    public void CastSpearLinear(Vector3 startPos, Vector3 dir, float speed, LayerMask target, Func<Spear, Transform, bool> OnSpearHit = null, Func<Spear, bool> OnSpearDespawn = null)
    {
        this.OnSpearHit = OnSpearHit;
        targetLayers = target;

        spearCast = StartCoroutine(CastSpearLinear(startPos, dir, speed, OnSpearDespawn));
    }

    private IEnumerator CastSpearLinear(Vector3 startPos, Vector3 dir, float speed, Func<Spear, bool> OnSpearDespawn = null)
    {
        transform.position = startPos;
        transform.up = dir;
        float spinAng = 0;

        do {
            spinAng += spinSpeed * Time.deltaTime;
            transform.position = transform.position + transform.up * speed * Time.deltaTime;
            model.localEulerAngles = new Vector3(0, spinAng, 0);

            yield return null;
        } while (Vector3.Distance(transform.position, startPos) < despawnRange);

        if (OnSpearDespawn == null || OnSpearDespawn?.Invoke(this) == true)
            Destroy(gameObject);
    }

    public void CastSpearArc(Vector3 startPos, Vector3 targetPos, float maxHeight, float duration, LayerMask target, Func<Spear, Transform, bool> OnSpearHit = null, Func<Spear, bool> OnSpearDespawn = null)
    {
        this.OnSpearHit = OnSpearHit;
        targetLayers = target;

        spearCast = StartCoroutine(CastSpearArc(startPos, targetPos, maxHeight, duration, OnSpearDespawn));
    }
    private IEnumerator CastSpearArc(Vector3 startPos, Vector3 targetPos, float maxHeight, float duration, Func<Spear, bool> OnSpearDespawn = null)
    {
        float start = Time.time;
        float spinAng = 0;

        float spearAng = Vector3.SignedAngle(Vector3.forward, targetPos - startPos, Vector3.up);

        float tx, ty;
        do
        {
            tx = (Time.time - start) / duration;
            ty = -Mathf.Pow(2 * tx - 1, 2) + 1;
            spinAng += spinSpeed * Time.deltaTime;

            transform.position = startPos
                             + (targetPos - startPos) * tx
                             + Vector3.up * maxHeight * ty;

            transform.eulerAngles = new Vector3(180 * tx, spearAng, 0);
            model.localEulerAngles = new Vector3(0, spinAng, 0);

            yield return null;
        } while (tx < 1);

        if (OnSpearDespawn == null || OnSpearDespawn?.Invoke(this) == true)
            Destroy(gameObject);
    }
}
