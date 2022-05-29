using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Item properties")]
    public float interactRadius;
    public float lifeTime = 15;
    public bool isDestroyable;
    public bool destroyByLifetime;

    [Header("Other")]
    internal GameObject player;
    internal Vector3 vel;
    internal float distance;

    protected void Start()
    {
        if (destroyByLifetime)
            Destroy(gameObject, lifeTime);
        player = PlayerController.instance.gameObject;
    }

    protected void Update()
    {
        if (player == null || !player.transform.hasChanged)
            return;

        distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance < interactRadius)
        {
            OnReachZoneEnter();
        }
        else
            OnReachZoneExit();
    }

    public virtual void PickUp()
    {
        if (isDestroyable)
            Destroy(gameObject);
    }

    public virtual void OnReachZoneEnter()
    {
    }

    public virtual void OnReachZoneExit()
    {
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }

}
