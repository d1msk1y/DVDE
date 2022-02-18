using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowedGun : MonoBehaviour
{
    [SerializeField] private float _lifetime;
    private float _currentLifetime;

    private TrailRenderer _trailRenderer;

    private void Start()
    {
        StartCoroutine(DestroyGun());
    }


    private IEnumerator DestroyGun()
    {
        yield return new WaitForSeconds(1);
        while(transform.localScale != Vector3.zero)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.1f);
            if(transform.localEulerAngles == Vector3.zero)
            yield return null;
        }
        Destroy(gameObject);
    }
}
