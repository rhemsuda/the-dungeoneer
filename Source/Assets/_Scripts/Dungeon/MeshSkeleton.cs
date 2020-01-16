using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSkeleton : MonoBehaviour 
{
    public bool IsColliding;
	void OnTriggerEnter(Collider col)
	{
		IsColliding = true;
	}
	void OnTriggerStay(Collider col)
	{
		IsColliding = true;
	}
	void OnTriggerExit(Collider col)
	{
		IsColliding = false;
	}
}
