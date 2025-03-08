using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderClimb : MonoBehaviour
{
    private Vector3 ladderRotation;
    
    public void SetLadderRotation(GameObject ladder)
    {
        ladderRotation = ladder.transform.forward;
    }

    public void LookAtLadder()
    {
        Vector3 up = transform.rotation * Vector3.up;

        transform.rotation = Quaternion.AngleAxis(Vector3.SignedAngle(transform.forward, ladderRotation,up),up) * transform.rotation;
    }
}
