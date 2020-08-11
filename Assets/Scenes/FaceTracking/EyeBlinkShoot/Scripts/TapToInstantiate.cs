using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapToInstantiate : MonoBehaviour
{
    public GameObject prefab;
    public Transform parent;
    public float nearest = 0.1f;
    public float furthest = 0.25f;

    private Camera mainCam;

	private void Awake()
	{
        mainCam = Camera.main;
	}

    // Update is called once per frame
    void Update()
    {
  //      if (Application.platform == RuntimePlatform.Android)
		//{
  //          if (Input.touches.Length > 0)
  //          {

  //          }
  //      }
  //      else
		//{

		//}
        if (Input.GetMouseButtonUp(0))
		{
            Vector3 mousePos = Input.mousePosition;
            float z = Random.Range(nearest, furthest);
            Vector3 point = mainCam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, z));
            GameObject newGO = Instantiate(prefab, parent);
            newGO.transform.position = point;
        }

    }
}
