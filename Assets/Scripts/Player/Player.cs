using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] public float hp { get; set; } = 100;
    [SerializeField] public float maxHp { get; set; } = 100;
}
