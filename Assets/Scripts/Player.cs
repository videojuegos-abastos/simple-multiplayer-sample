using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;
using TMPro;
using Unity.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class Player : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI ballsTMP;
    [SerializeField] TextMeshProUGUI spacesTMP;
    [SerializeField] GameObject bomb;
    
    NetworkVariable<int> balls = new NetworkVariable<int>(value: 0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    NetworkVariable<int> spaces = new NetworkVariable<int>(value: 0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


    NavMeshAgent agent;
    public override void OnNetworkSpawn()
    {
        agent = GetComponent<NavMeshAgent>();

        balls.OnValueChanged += OnBallsValueChanged;
        spaces.OnValueChanged += OnSpacesValueChanged;

        if (IsOwner)
        {
            FindObjectOfType<PointerManager>().onPositionSelected.AddListener( MoveTo );
            FindObjectOfType<Cinemachine.CinemachineVirtualCameraBase>().LookAt = transform;
        }
    }

    void Update()
    {
        if (IsOwner)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                spaces.Value += 1;
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                SendMessage_ServerRpc("Hola!");
                //message.Value = new FixedString32Bytes($"Tengo {balls.Value} puntos!");
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                CreateBomb_ServerRpc();
            }
        }
    }

    void OnBallsValueChanged(int oldValue, int newValue)
    {
        ballsTMP.text = newValue.ToString();
    }

    void OnSpacesValueChanged(int oldValue, int newValue)
    {
        spacesTMP.text = newValue.ToString();
    }

    IEnumerator DisplayMessage(string _message)
    {
        ballsTMP.text = _message;

        yield return new WaitForSeconds(2f);

        ballsTMP.text = balls.Value.ToString();
    }

    [ServerRpc]
    void SendMessage_ServerRpc(string message, ServerRpcParams parameters = default)
    {
        SendMessage_ClientRpc(message);
    }

    [ClientRpc]
    void SendMessage_ClientRpc(string message, ClientRpcParams parameters = default)
    {
        spacesTMP.text = message;
    }


    [ServerRpc]
    void CreateBomb_ServerRpc(ServerRpcParams parameters = default)
    {
        var instance = Instantiate(bomb, transform.position, Quaternion.identity);
        instance.GetComponent<NetworkObject>().SpawnWithOwnership(parameters.Receive.SenderClientId);
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