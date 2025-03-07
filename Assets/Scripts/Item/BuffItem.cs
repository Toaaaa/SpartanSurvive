using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum BuffType
{
    DoubleJump,
    Defense,
}

public class BuffItem : MonoBehaviour
{
    public BuffType buffType;

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.transform.tag == "Player")
        {
            switch (buffType)
            {
                case BuffType.DoubleJump:
                    collider.transform.GetComponent<Player>().DoubleJumpBuffOn();
                    break;
                case BuffType.Defense:
                    collider.transform.GetComponent<Player>().DefenceBuffOn();
                    break;
            }
            gameObject.SetActive(false);
            Invoke("EnableObject", 20f); // 20초뒤 다시 활성화.
        }
    }

    private void EnableObject()
    {
        gameObject.SetActive(true);
    }
}
