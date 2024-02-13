using UnityEngine;

public class LitterTray : MonoBehaviour, IEquipment
{
	[SerializeField] private Transform interactLocation;
	[SerializeField] private float BladderReward = 0.4f;
	private int uses = 0;

	[field: SerializeField] public float UseTime { get; private set; } = 1f;

	public Transform InteractLocation => interactLocation;
	public bool Usable => uses > 0;

	public float Use()
	{
		uses--;
		return BladderReward;
	}

	[ContextMenu("Clean")]
	public void Clean()
	{
		uses = 5;
		GameManager.Instance.ReturnEquipment(this);
	}
}
