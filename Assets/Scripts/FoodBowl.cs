using UnityEngine;

public class FoodBowl : MonoBehaviour, IEquipment
{
	[SerializeField] private Transform interactLocation;
	[SerializeField] float foodReward = 0.4f;
	private int uses = 0;

	[field: SerializeField] public float UseTime { get; private set; } = 1f;

	public Transform InteractLocation => interactLocation;
	public bool Usable => uses > 0;
	public float FoodReward => foodReward;

	public float Use()
	{
		uses--;
		return foodReward;
	}

	[ContextMenu("Refill")]
	public void Refill()
	{
		uses = 3;
		GameManager.Instance.ReturnEquipment(this);
	}
}
