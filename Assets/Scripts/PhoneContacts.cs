using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PhoneContacts : MonoBehaviour
{
	[SerializeField] Contact contactPrefab;
	[SerializeField] RectTransform contactsParent;
	// Start is called before the first frame update
	public void Setup(List<Request> requests, Phone p)
	{
		Clear();
		foreach (Request request in requests)
		{
			Contact c = Instantiate(contactPrefab, contactsParent);
			UnityAction action = () => p.OpenMessage(request);
			c.Setup(request, action);
			c.GetComponent<Button>().onClick.AddListener(action);
		}
	}

	public void Clear()
	{
		while (contactsParent.childCount > 0)
		{
			DestroyImmediate(contactsParent.GetChild(0).gameObject);
		}
	}
}
