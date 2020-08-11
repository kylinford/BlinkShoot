using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Bullet bulletPrefab;
	public float speed;

	public void Shoot()
	{
		Bullet newBullet = Instantiate(bulletPrefab, transform);
		newBullet.transform.SetParent(null);
		newBullet.speed = speed;
	}
}
