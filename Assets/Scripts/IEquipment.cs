using UnityEngine;

public interface IEquipment
{
	Transform InteractLocation { get; }
	void Use(Animal a);
}
