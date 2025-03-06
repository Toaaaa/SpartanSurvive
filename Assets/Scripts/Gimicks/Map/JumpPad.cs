using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : PhysicsObject
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 dir = Quaternion.Euler(10, 0, 0) * Vector3.up;// 100도 각도로 발사
            rb.AddForce(dir * 15, ForceMode.Impulse);
        }
    }
}
