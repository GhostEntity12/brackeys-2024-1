using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Contact : MonoBehaviour
{
	[SerializeField] Image unreadDot;
	[SerializeField] Image icon;
	[SerializeField] TextMeshProUGUI contactName;
	[SerializeField] TextMeshProUGUI lookingFor;
	public UnityAction onButtonPress;
	public void Setup(Request r, UnityAction action)
	{
		unreadDot.enabled = r.unread;
		icon.color = r.iconColor;
		contactName.text = r.name;
		lookingFor.text = $"Looking for {r.attributeString}";
		onButtonPress = action;
	}
}
