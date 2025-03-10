using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : PhysicsObject
{
    public float angle;
    public float force;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 dir = Quaternion.Euler(angle, 0, 0) * Vector3.up;// 위로 향하는 벡터를  회전
            rb.velocity = Vector3.zero;
            rb.AddForce(dir * force, ForceMode.Impulse);
        }
    }
}
