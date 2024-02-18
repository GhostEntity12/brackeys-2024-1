using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static AnimalInfo;

[System.Serializable]
public class Request
{
	public string name;
	public List<Attributes> attributes = new();
	public Color iconColor;
	public string attributeString;
	public string requestMessage;
	public bool unread = true;
	public Request()
	{
		name = GameManager.humanNames[GameManager.rand.Next(GameManager.humanNames.Count)];

		int attributeCount = GameManager.rand.NextDouble() switch
		{
			< 0.2f => 3,
			< 0.9f => 2,
			_ => 1
		};

		// Generate attributes
		int[] attributeIndexes = new int[attributeCount];
		int numberOfAttributes = System.Enum.GetNames(typeof(Attributes)).Length;
		for (int i = 0; i < attributeCount; i++)
		{
			int index;
			do
			{
				index = GameManager.rand.Next(1, numberOfAttributes + 1);
			} while (attributeIndexes.Contains(index));
			attributeIndexes[i] = index;
			attributes.Add((Attributes)index);
		}

		iconColor = Color.HSVToRGB((float)GameManager.rand.NextDouble(), 0.4f, 0.9f);

		switch (attributeCount)
		{
			case 1:
				attributeString = attributes[0].ToString();
				break;
			case 2:
				attributeString = string.Join(" or ", attributes);
				break;
			default:
				attributeString = string.Join(", ", attributes.GetRange(0, attributeCount - 1));
				attributeString += $" or {attributes[^1]}";
				break;
		}
		requestMessage = string.Format(GameManager.requestMessages[GameManager.rand.Next(GameManager.requestMessages.Count)], attributeString);
	}
}
