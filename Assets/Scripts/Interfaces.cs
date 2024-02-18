using System;
using UnityEngine;

public interface IEquipment
{
	GameObject gameObject { get; }
	Transform InteractLocation { get; }
	float UseTime { get; }
}

public interface IActionable
{
	Sprite Icon { get; }
	bool IsActionable { get; }
	void Action();
}

public interface ILimitedUseable
{
	int Uses { get; }
	float Use();
}
