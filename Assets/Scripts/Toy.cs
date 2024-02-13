using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toy : MonoBehaviour, IEquipment
{
	[SerializeField] private Transform interactLocation;
	public Transform InteractLocation => interactLocation;

	[field: SerializeField] public float UseTime { get; private set; } = 1f;
	[field: SerializeField] public float EntertainmentReward { get; private set; } = 0.4f;
}
