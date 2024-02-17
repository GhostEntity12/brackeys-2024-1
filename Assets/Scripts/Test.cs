using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Test : MonoBehaviour
{
	public AnimalInfo a;
	public AnimalInfo ab;

	public string j;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.M))
		{
			a = GameManager.Instance.ChooseNewAnimal();
			j = JsonUtility.ToJson(a);
			ab = JsonUtility.FromJson<AnimalInfo>(j);
		}
	}
}
