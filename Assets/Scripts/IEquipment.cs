using System;
using UnityEngine;

public interface IEquipment
{
	GameObject gameObject { get; }
	Transform InteractLocation { get; }
	float UseTime { get; }
}
