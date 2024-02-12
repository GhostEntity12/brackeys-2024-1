using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LitterTray : MonoBehaviour, IEquipment
{
	[SerializeField] private Transform interactLocation;
	public Transform InteractLocation => InteractLocation;

	[SerializeField] float useTime = 1f;
	[SerializeField] float foodReward = 0.4f;
	float useTimer;

	void Update()
	{
		if (useTimer > 0)
		{
			useTimer -= Time.deltaTime;
			if (useTimer <= 0)
			{

			}
		}
	}

	public void Use(Animal a)
	{
		useTimer = useTime;
		a.OnFinishPoop += 
	}
}
