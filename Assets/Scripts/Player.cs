using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;
using TMPro;

[RequireComponent(typeof(NavMeshAgent))]
public class Player : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI TMP;
    
    NetworkVariable<int> balls = new NetworkVariable<int>(value: 0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    NavMeshAgent agent;
    public override void OnNetworkSpawn()
    {
        agent = GetComponent<NavMeshAgent>();


        balls.OnValueChanged += OnBallsValueChanged;


        if (IsOwner)
        {
            FindObjectOfType<PointerManager>().onPositionSelected.AddListener( MoveTo );
        }
    }

    void OnBallsValueChanged(int oldValue, int newValue)
    {
        TMP.text = newValue.ToString();
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

            if (IsOwner)
            {
                balls.Value += 1;
            }
        }
    }
}