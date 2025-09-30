using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Tail : MonoBehaviour
{
    private Transform lastTail;
    private NetworkObject Owner;

    [SerializeField] private float distance;

    [SerializeField] private float speed;
    [SerializeField] private Vector3 targetPosition;

    // Update is called once per frame
    void Update()
    {
        if ( lastTail != null )
        {
            targetPosition = lastTail.position - lastTail.up * distance;
            //Debug.Log(this.name + "'s last tail is " + lastTail.name);
        }
        targetPosition.z = 0;
        transform.up = targetPosition - transform.position;

        transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
    }

    public void AssignOwner(NetworkObject owner) => Owner = owner;

    public NetworkObject GetOwner() => Owner;

    public void AssignLastTail(Transform tail) => lastTail = tail;
}
