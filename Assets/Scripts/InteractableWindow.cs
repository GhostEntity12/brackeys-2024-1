using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InteractableWindow : MonoBehaviour
{
	private Camera c;
	private bool active;
	IEquipment equipment;
	private RectTransform rectTransform;

	[SerializeField] private TextMeshProUGUI equipmentName;
	[SerializeField] private TextMeshProUGUI usesRemaining;
	[SerializeField] private Button actionButton;

	private void Start()
	{
		c = Camera.main;
		rectTransform = transform as RectTransform;
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (Physics.Raycast(c.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100f, 1 << 7) &&
				hit.transform.TryGetComponent(out IEquipment e))
			{
				Show(e);
			}
			else if (!EventSystem.current.IsPointerOverGameObject())
			{
				Hide();
			}
		}

		if (equipment != null)
		{
			rectTransform.position = c.WorldToScreenPoint(equipment.gameObject.transform.position + Vector3.up * 2);
			if (equipment is IActionable a)
				actionButton.interactable = a.IsActionable;
			if (equipment is ILimitedUseable lu)
				usesRemaining.text = $"{lu.Uses} use{(lu.Uses == 1 ? "" : "s")} remaining";

		}
	}

	public void Show(IEquipment e)
	{
		if (equipment == e) return;

		if (active)
		{
			active = false;
			// Vanish the element and call this function recursively
			LeanTween.scaleY(gameObject, 0f, 0.3f).setEaseInBack().setOnComplete(() => Show(e));
		}
		else
		{
			active = true;
			equipment = e;
			equipmentName.text = equipment.gameObject.name;
			usesRemaining.gameObject.SetActive(equipment is ILimitedUseable);
			actionButton.gameObject.SetActive(equipment is IActionable);
			if (equipment is ILimitedUseable lu)
				usesRemaining.text = $"{lu.Uses} use{(lu.Uses == 1 ? "" : "s")} remaining";
			if (equipment is IActionable a)
				actionButton.transform.GetChild(0).GetComponent<Image>().sprite = a.Icon;
			LeanTween.scaleY(gameObject, 1f, 0.3f).setEaseOutBack();
		}
	}

	public void Hide()
	{
		active = false;
		LeanTween.scaleY(gameObject, 0f, 0.3f).setEaseInBack().setOnComplete(() => equipment = null);
	}

	public void Action()
	{
		if (equipment is IActionable a)
		{
			a.Action();
		}
	}
}
