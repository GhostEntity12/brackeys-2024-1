using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhoneMessage : MonoBehaviour
{
	[SerializeField] CanvasGroup canvasGroup;
	[SerializeField] TextMeshProUGUI header;
	[SerializeField] TextMeshProUGUI requestMessage;
	[SerializeField] RectTransform selectionBar;
	[SerializeField] RectTransform adoptionButtonParent;
	[SerializeField] AdoptionButton adoptionButtonPrefab;
	[SerializeField] Button backButton;
	Request request;
	bool selectionBarOpen = false;
	public void Setup(Request request)
	{
		Clear();
		this.request = request;
		request.unread = false;
		header.text = request.name;
		requestMessage.text = request.requestMessage;
		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;
		canvasGroup.alpha = 1;
		Canvas.ForceUpdateCanvases();
	}

	public void OpenSelectionBar()
	{
		if (selectionBarOpen) return;

		selectionBarOpen = true;
		foreach (Animal a in GameManager.Instance.animals)
		{
			AdoptionButton b = Instantiate(adoptionButtonPrefab, adoptionButtonParent);
			b.Setup(a, request.attributes);
		}
		LeanTween.scaleY(selectionBar.gameObject, 1, 0.3f).setEaseOutBack();
	}
	public void Clear()
	{
		while (adoptionButtonParent.childCount > 0)
		{
			Destroy(adoptionButtonParent.GetChild(0).gameObject);
		}
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
		canvasGroup.alpha = 0;
		selectionBarOpen = false;
	}

	public void HideSelectionBar()
	{
		selectionBar.gameObject.transform.localScale = new(1, 0, 1);
		selectionBarOpen = false;
	}
}
