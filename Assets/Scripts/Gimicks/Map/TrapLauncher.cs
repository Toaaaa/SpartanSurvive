using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapLauncher : MonoBehaviour
{
    [SerializeField] Transform RaycastPoint; // 레이저 발사 위치
    [SerializeField] Transform FirePoint; // 발사 위치
    [SerializeField] GameObject PrefabObject; // 발사할 프리팹 오브젝트

    private List<GameObject> ObjectList = new List<GameObject>();// 발사한 오브젝트를 담을 리스트 (오브젝트 풀링)
    bool isFire = false;// 발사 여부
    bool isActive = false;// 활성화 여부

    private void Update()
    {
        RayCast();
        if(!isFire)
            StartCoroutine(StartSpawning());
    }

    void RayCast()
    {
        Ray ray = new Ray(RaycastPoint.position, RaycastPoint.up);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, 5f))
        {
            if(hit.collider.CompareTag("Player"))
            {
                isActive = true;
            }
        }
    }

    void LaunchObject(GameObject obj)
    {
        obj.GetComponent<Rigidbody>().AddForce(FirePoint.right * 50f, ForceMode.Impulse);
        obj.GetComponent<PhysicsObjects>().dir = FirePoint.right;
        StartCoroutine(SetDisable(obj));
    }

    IEnumerator StartSpawning()
    {
        while (isActive)
        {
            if (ObjectList.Count > 0)
            {
                foreach (GameObject o in ObjectList)
                {
                    if (!o.activeSelf)// 사용중이 아닌 오브젝트가 있을경우 그거 사용.
                    {
                        Debug.Log("재사용");
                        isFire = true;
                        o.gameObject.SetActive(true);
                        o.transform.position = FirePoint.position;
                        o.transform.rotation = FirePoint.rotation;
                        LaunchObject(o);
                        yield return new WaitForSeconds(5f);
                        isFire = false;
                        break;
                    }
                }
            }
            else
            {
                isFire = true;
                GameObject obj = Instantiate(PrefabObject, FirePoint);
                ObjectList.Add(obj);
                obj.transform.position = FirePoint.position;
                obj.transform.rotation = FirePoint.rotation;
                obj.SetActive(true);
                LaunchObject(obj);
                yield return new WaitForSeconds(5f);
                isFire = false;
            }
        }
    }

    IEnumerator SetDisable(GameObject obj)
    {
        yield return new WaitForSeconds(5f);
        obj.SetActive(false);
    }
}
