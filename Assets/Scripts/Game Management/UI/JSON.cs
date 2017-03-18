using UnityEngine;
using System.Collections;

[System.Serializable]
public class Example
{
	public string fileName { get; set; }
	public string displayName { get; set; }
	public int uid { get; set; }

	public Example2 example { get; set; }
}

[System.Serializable]
public class Example2
{
	public float floatex { get; set; }
	public bool boolex = false;
}