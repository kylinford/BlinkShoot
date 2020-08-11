using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

public class Heart : MonoBehaviour
{
    private float speedRotate = 1;
    private float targetScale = 1.1f;
    private float timerScale = 1;
    private float floatSpeed = 0.1f;
    private Camera mainCam;
    private Coroutine coroutineIdle;

    public bool isBlockedByFace
    {
        get
        {
            RaycastHit[] hits = Physics.RaycastAll(mainCam.transform.position,
                transform.position - mainCam.transform.position, mainCam.farClipPlane);
            foreach (RaycastHit hit in hits)
			{
                if (hit.transform.GetComponent<ARFace>())
				{
                    return true;
				}
			}
            return false;
        }
    }


    private void Awake()
	{
        mainCam = Camera.main;
        speedRotate = Random.Range(0.5f, 1f);
        targetScale = Random.Range(0.5f, 1.5f);
        timerScale = Random.Range(0.6f, 1.5f);
    }

    void Start()
    {
        coroutineIdle = StartCoroutine(ExpandAndShrinkEnumerator());
    }

    void Update()
    {
        transform.Rotate(Vector3.up, speedRotate );
        transform.Translate(Vector3.up * Time.deltaTime * floatSpeed, Space.Self);
        gameObject.SetActive(!isBlockedByFace);
    }

    public IEnumerator ExplodeEnumerator()
	{
        StopCoroutine(coroutineIdle);
        yield return ScaleEnumerator(Vector3.one * 10f, 1);
        Destroy(gameObject);
    }

	private void OnTriggerEnter(Collider other)
	{
        //Hit by bullet
        if (other.GetComponent<Bullet>())
        {
            StartCoroutine(ExplodeEnumerator());
        }
    }

	private IEnumerator ExpandAndShrinkEnumerator()
	{
        while(true)
		{
            yield return ScaleEnumerator(Vector3.one * targetScale, timerScale);
            yield return ScaleEnumerator(Vector3.one, timerScale);
        }
        
    }

    private IEnumerator ScaleEnumerator(Vector3 target, float timer)
	{
        Vector3 initLocalScale = transform.localScale;
        Vector3 targetLocalScale = target;

        float coveredTime = 0;
        while(coveredTime < timer)
		{
            yield return new WaitForEndOfFrame();
            coveredTime += Time.deltaTime;
            float currRatio = coveredTime / timer;
            transform.localScale = initLocalScale + currRatio * (targetLocalScale - initLocalScale);
		}
        transform.localScale = targetLocalScale;
    }
}
