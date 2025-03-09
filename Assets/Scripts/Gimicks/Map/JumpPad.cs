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
            Vector3 dir = Quaternion.Euler(8, 0, 0) * Vector3.up;// 위로 향하는 벡터를 8도 회전
            rb.AddForce(dir * 13, ForceMode.Impulse);
        }
    }
}
