using UnityEngine;
using UnityEngine.Events;

public class PointerManager : MonoBehaviour
{
	[SerializeField]
	GameObject pointer;

    UnityEvent<Vector3> onPositionSelected = new UnityEvent<Vector3>();

	bool _spawned = false;
	bool spawned
	{
		get { return _spawned; }
		set { _spawned = _spawned || value; }
	}


    void Update () 
    {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		bool collided = Physics.Raycast(ray, out hit);

		if (collided)
		{

			if (!spawned)
			{
				pointer = Instantiate(pointer, hit.point, Quaternion.identity);
				spawned = true;
			}

				pointer.SetActive(true);
				pointer.transform.position = hit.point;


			if (Input.GetMouseButtonDown(0))
			{
				onPositionSelected.Invoke(pointer.transform.position);
			}

		}
		else
		{
			pointer.SetActive(false);
		}

    }
}