using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.UI;

public class Phone : MonoBehaviour
{
	[SerializeField] Button phoneButton;
	[SerializeField] Image phoneButtonSprite;
	[SerializeField] Sprite phoneUnread;
	[SerializeField] Sprite phoneNoMessage;
	[field: SerializeField] public PhoneMessage Message {get; private set; }
	[field: SerializeField] public PhoneContacts Contacts { get; private set; }

	public List<Request> requests = new();

	RectTransform rectTransform;
	RectTransform buttonRectTransform;
	float nextRequestTimer;
	bool unreadMessages;

	[SerializeField] AudioClip notification;

	private void Start()
	{
		nextRequestTimer = requests.Count == 0 ? 30 : 5 * Mathf.Pow(requests.Count, 1.25f) + Random.Range(100, 150);
		rectTransform = transform as RectTransform;
		buttonRectTransform = phoneButton.transform as RectTransform;
	}

	private void Update()
	{
		nextRequestTimer -= Time.deltaTime;
		if (nextRequestTimer < 0)
		{
			requests.Add(new());
			phoneButtonSprite.sprite = phoneUnread;
			unreadMessages = true;
			nextRequestTimer = 5 * Mathf.Pow(requests.Count, 1.25f) + Random.Range(100, 150);
			LeanTween.moveY(buttonRectTransform, 70, 0.2f).setEaseOutCubic().setOnComplete(() =>
			{
				LeanTween.moveY(buttonRectTransform, 40, 0.2f).setEaseInCubic();
			});
			GameManager.Instance.PlayAudioClip(notification);
		}
	}

	public void OnClickPhoneButton()
	{
		if (unreadMessages)
		{
			unreadMessages = false;
			phoneButtonSprite.sprite = phoneNoMessage;
			Message.Setup(requests[^1]);
			LeanTween.moveY(rectTransform, 0, 0.5f).setEaseOutBack();
			LeanTween.moveY(buttonRectTransform, -190, 0.3f).setEaseInBack();
		}
		else
		{
			Contacts.Setup(requests, this);
			LeanTween.moveY(rectTransform, 0, 0.5f).setEaseOutBack();
			LeanTween.moveY(buttonRectTransform, -190, 0.3f).setEaseInBack();
		}
	}

	public void OpenMessage(Request r)
	{
		Message.Setup(r);
	}

	public void OpenContacts()
	{
		Contacts.Setup(requests, this);
	}

	public void ClosePhone()
	{
		LeanTween.moveY(rectTransform, -1500, 0.5f).setEaseInBack();
		LeanTween.moveY(buttonRectTransform, 40, 0.3f).setEaseOutBack();
	}
}
