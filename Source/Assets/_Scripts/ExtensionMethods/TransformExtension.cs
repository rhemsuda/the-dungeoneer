using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtension
{
	public static List<GameObject> FindGameObjectsByChildTag(this Transform parent, string tag)
	{
		List<GameObject> childObjects = new List<GameObject> ();

		for (int i = 0; i < parent.childCount; i++) 
		{
			Transform child = parent.GetChild (i);
			if (child.tag.Equals (tag)) 
			{
				childObjects.Add (child.gameObject);
			}
			if (child.childCount > 0)
			{
				childObjects.AddRange (FindGameObjectsByChildTag (child, tag));
			}
		}

		return childObjects;
	}

    public static List<GameObject> FindGameObjectsByChildName(this Transform parent, string name)
    {
        List<GameObject> childObjects = new List<GameObject>();

        for(int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if(child.name.Equals(name))
            {
                childObjects.Add(child.gameObject);
            }
            if (child.childCount > 0)
            {
                childObjects.AddRange(FindGameObjectsByChildName(child, name));
            }
        }

        return childObjects;
    }


    public static Transform FindChildInChildren(this Transform aParent, string name)
    {
        var result = aParent.Find(name);
        if (result != null)
            return result;

        foreach (Transform child in aParent)
        {
            result = child.FindChildInChildren(name);
            if (result != null)
                return result;
        }
        return null;
    }
}
