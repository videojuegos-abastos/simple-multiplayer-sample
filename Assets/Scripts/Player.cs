using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

[RequireComponent(typeof(NavMeshAgent))]
public class Player : NetworkBehaviour
{
    float size;
    NavMeshAgent agent;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void MoveTo(Vector3 position)
    {
        agent.SetDestination(position);
    }

    void OnTriggerEnter(Collider other)
    {
        const float FACTOR = .1f;
        if (other.TryGetComponent<Food>(out Food food))
        {
            transform.localScale += Vector3.up * food.transform.localScale.magnitude * FACTOR;
            Destroy(food.gameObject);
        }
    }

}