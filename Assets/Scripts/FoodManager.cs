using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class FoodManager : NetworkBehaviour
{

    [SerializeField] Vector3Int center = Vector3Int.zero;
    [SerializeField] Vector2Int size = Vector2Int.one;
    [SerializeField] Vector2 minMaxFoodScale = new Vector2(.5f, 2f);
    [SerializeField] float waitTime = 2f;
    [SerializeField] GameObject food;


    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            Destroy(gameObject);
            return;
        }

        StartCoroutine(nameof(SpawnFood));
    }

    IEnumerator SpawnFood()
    {
        while (isActiveAndEnabled)
        {
            Vector3 position = new Vector3(Random.Range(-size.x / 2, size.x / 2), 0, Random.Range(-size.y / 2, size.y / 2));
            float scale = Random.Range(minMaxFoodScale.x, minMaxFoodScale.y);
            GameObject instance = Instantiate(food, center + position, Quaternion.identity);
            instance.transform.localScale *= scale;

            instance.GetComponent<NetworkObject>().Spawn();

            yield return new WaitForSeconds(waitTime);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(center, new Vector3(size.x, 0, size.y));
    }

}
