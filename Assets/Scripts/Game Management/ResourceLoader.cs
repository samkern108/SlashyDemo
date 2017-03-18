using UnityEngine;
using System.Collections;
using System.IO;

public class ResourceLoader : MonoBehaviour {

	private static string pathToPrefabs = "Prefabs/";
	private static string pathToLevels = "Prefabs/Levels/";

	public static GameObject LoadPrefab(string name)
	{
		return Resources.Load <GameObject>(pathToPrefabs + name);
	}

	public static GameObject LoadLevelPrefab(int level)
	{
		return Resources.Load<GameObject> (pathToLevels + "Level " + level);
	}
}
