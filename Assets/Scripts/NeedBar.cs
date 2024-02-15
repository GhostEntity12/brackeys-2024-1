using UnityEngine;
using UnityEngine.UI;

public class NeedBar : MonoBehaviour
{
	private float fillAmount;
	private float tempAmount;

	[SerializeField] Gradient barColor;
	[SerializeField] Image bar;
	[SerializeField] float fillSpeed = 1f;

	public float FillAmount { set => fillAmount = value; }

	private void Update()
	{
		if (tempAmount != fillAmount)
		{
			tempAmount = fillAmount > tempAmount ?
				Mathf.Min(tempAmount + fillSpeed, fillAmount) :
				Mathf.Max(tempAmount - fillSpeed, fillAmount);

			bar.fillAmount = tempAmount;
			bar.color = barColor.Evaluate(fillAmount);
		}
	}
}
