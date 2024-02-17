using UnityEngine;

public class LitterTray : MonoBehaviour, IEquipment, IActionable, ILimitedUseable
{
	public int Uses { get; private set; } = 0;

	[field: SerializeField] public float BladderReward { get; private set; } = 0.4f;
	[field: SerializeField] public float UseTime { get; private set; } = 1f;
	[field: SerializeField] public Transform InteractLocation { get; private set; }
	[field: SerializeField] public Sprite Icon { get; private set; }

	public bool Usable => Uses > 0;
	public bool IsActionable => Uses != maxUses;

	[SerializeField] SpriteRenderer poop;
	[SerializeField] int maxUses = 5;

	private void Start()
	{
		poop.enabled = Uses == 0;
	}

	public float Use()
	{
		Uses--;
		if (Uses == 0)
			poop.enabled = true;
		return BladderReward;
	}

	public void Action()
	{
		Uses = maxUses;
		poop.enabled = false;
		GameManager.Instance.ReturnEquipment(this);
	}
}
