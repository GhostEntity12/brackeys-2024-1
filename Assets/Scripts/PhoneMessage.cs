using TMPro;
using UnityEngine;
using UnityEngine.Events;
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
	[SerializeField] RectTransform selectionButton;
	[SerializeField] GameObject response1;
	[SerializeField] TextMeshProUGUI response1Text;
	[SerializeField] GameObject response2;
	[SerializeField] Image response2Image;

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
			UnityAction action = () => SendConfirmation(a);
			b.Setup(a, request.attributes, action);
			b.GetComponent<Button>().onClick.AddListener(action);
		}
		LeanTween.scaleY(selectionBar.gameObject, 1, 0.3f).setEaseOutBack();
	}

	public void SendConfirmation(Animal a)
	{
		selectionButton.gameObject.SetActive(false);
		response1Text.text = GameManager.confirmationMessages[Random.Range(0, GameManager.confirmationMessages.Count)];
		response2Image.sprite = GameManager.Instance.GetSpriteAtlasByName(a.AnimalInfo.spriteAtlasName).GetSprite($"Spr_{a.AnimalInfo.spriteAtlasName}_0");
		response1.SetActive(true);
		response2.SetActive(true);
		LeanTween.scaleX(response1, 1, 0.3f).setEaseOutBack().setDelay(0.7f);
		LeanTween.scaleX(response2, 1, 0.3f).setEaseOutBack().setDelay(1.5f);
		GameManager.Instance.ClearForAdoption(a);
		GameManager.Instance.phone.requests.Remove(request);
	}

	public void Clear()
	{
		while (adoptionButtonParent.childCount > 0)
		{
			Transform t = adoptionButtonParent.GetChild(0);
			t.GetComponent<Button>().onClick.RemoveListener(t.GetComponent<AdoptionButton>().onButtonPress);
			DestroyImmediate(t.gameObject);
		}
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
		canvasGroup.alpha = 0;
		selectionBarOpen = false;
		selectionButton.gameObject.SetActive(true);
		response1.SetActive(false);
		response2.SetActive(false);
		response1.transform.localScale = new(0, 1, 1);
		response2.transform.localScale = new(0, 1, 1);
	}

	public void HideSelectionBar()
	{
		selectionBar.gameObject.transform.localScale = new(1, 0, 1);
		selectionBarOpen = false;
	}
}
