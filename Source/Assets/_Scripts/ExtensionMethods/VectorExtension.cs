using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtension 
{
	public static float DistanceXZ(this Vector3 vec1, Vector3 target)
	{
		float xDif = vec1.x - target.x;
		float zDif = vec1.z - target.z;
		float distance = Mathf.Sqrt ((xDif * xDif) + (zDif * zDif));
		return distance;
	}
}
