using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffEffectController : MonoBehaviour
{
    Player player;
    [SerializeField] List<GameObject> FX;// 0: 체력 회복, 1: 방어, 2: 더블 점프.

    private void Awake()
    {
        player = GameManager.Instance.player;
        player.onStartFX += StartFX;// 이벤트 등록
    }

    public void StartFX(int index)// fx 재생
    {
        GameObject fx = Instantiate(FX[index], player.transform.position + new Vector3(0,0.1f,0), Quaternion.identity);
        float duration = fx.GetComponent<ParticleSystem>().main.duration;
        StartCoroutine(DestroyFX(fx, duration));
    }
    IEnumerator DestroyFX(GameObject fx, float duration)// duration이 끝나면 fx를 파괴.
    {
        yield return new WaitForSeconds(duration);
        Destroy(fx);
    }
}
