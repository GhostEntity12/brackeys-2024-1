using UnityEngine;
using UnityEngine.UI;

public class Bed : MonoBehaviour, IEquipment
{
	[field: SerializeField] public float SleepReward { get; private set; } = 0.4f;
	[field: SerializeField] public float UseTime { get; private set; } = 1f;
	[field: SerializeField] public Transform InteractLocation { get; private set; }
}
