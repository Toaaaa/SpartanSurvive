using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    virtual public void Interact()
    {
    }
    public string GetInfo()
    {
        return name;
    }
}
