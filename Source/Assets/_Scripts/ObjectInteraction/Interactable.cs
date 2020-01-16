using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactable 
{
    Dictionary<string, System.Action<object[]>> GetActions();
    string GetObjectName();
    string GetInteractText();
}
