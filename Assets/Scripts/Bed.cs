using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : MonoBehaviour, IEquipment
{
	[SerializeField] private Transform interactLocation;
	public Transform InteractLocation => InteractLocation;
	public void Use(Animal a)
	{
		throw new System.NotImplementedException();
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
