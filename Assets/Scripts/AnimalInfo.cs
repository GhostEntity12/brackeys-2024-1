using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalInfo
{
	public enum Attributes { Hungry, Cuddly, Lonely, Sleepy, Active }
	public enum Rarities { Common, Uncommon, Rare, Legendary, Mythic }

	string name;
	List<Attributes> attributes;
	float foodMod;
	float entertainmentMod;
	float attentionMod;
	float sleepMod;
	float bladderMod;
	float groomingMod;
	//appearance

	float food;
	float entertainment;
	float attention;
	float sleep;
	float bladder;
	float grooming;
}
