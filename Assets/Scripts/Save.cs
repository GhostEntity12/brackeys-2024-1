using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Save : MonoBehaviour
{
	public static string Write()
	{
		SaveData data = new SaveData(GameManager.Instance.animals.Select(a => a.AnimalInfo).ToList(), GameManager.Instance.phone.requests);

		string json = JsonUtility.ToJson(data);
		File.WriteAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + "save.dat", json);
		return json;
	}

	public static SaveData Read()
	{
		SaveData data = new SaveData();
		string json;
		try
		{
			json = File.ReadAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + "save.dat");

		}
		catch (FileNotFoundException)
		{
			json = Write();
			throw;
		}

		data = (SaveData)JsonUtility.FromJson(json, typeof(SaveData));
		return data;
	}
}

public class SaveData
{
	public List<AnimalInfo> animals = new();
	public List<Request> requests = new();

	public SaveData(List<AnimalInfo> animals, List<Request> requests)
	{
		this.animals = animals;
		this.requests = requests;
	}

	public SaveData() { }
}
