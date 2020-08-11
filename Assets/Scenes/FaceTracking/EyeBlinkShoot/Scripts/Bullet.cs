using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Bullet : MonoBehaviour
{
    public float speed = 0;
    public int lifeInSecond = 3;

    private DateTime birth;

    void Start()
    {
        birth = DateTime.Now;
    }

    void Update()
    {
        transform.Translate(-transform.forward * speed * Time.deltaTime, Space.World);
        if (DateTime.Now - birth > new TimeSpan(0,0,lifeInSecond))
		{
            Destroy(gameObject);
		}
    }

}
