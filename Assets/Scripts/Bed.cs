using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : MonoBehaviour, IEquipment
{
	[SerializeField] private Transform interactLocation;
	public Transform InteractLocation => interactLocation;

	[field: SerializeField] public float UseTime { get; private set; } = 1f;
	[field: SerializeField] public float SleepReward { get; private set; } = 0.4f;
}
