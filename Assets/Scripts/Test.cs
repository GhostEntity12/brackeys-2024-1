using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [Range(0, 1)]
    public float a;
    public float b;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        b = Mathf.Lerp(1.5f, 1f, a * 3.3f);
	}
}
