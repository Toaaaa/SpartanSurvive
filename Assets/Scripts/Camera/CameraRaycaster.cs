using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRaycaster : MonoBehaviour
{
    public float rayDistance = 6.6f; // Ray의 거리
    public LayerMask raycastLayerMask; // 레이캐스트를 체크할 레이어 (Object 레이어만 체크)

    private Camera mainCamera;

    void Start()
    {
        mainCamera = GetComponent<Camera>();
    }

    void Update()
    {
        RaycastFromCameraCenter();
    }

    void RaycastFromCameraCenter()
    {
        if (mainCamera == null) return;

        // 카메라 중앙에서 레이 생성
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance, raycastLayerMask))
        {
            if (hit.collider.CompareTag("Interactable"))// 테그가 Interactable인 경우
            {
                Debug.Log("Interactable Object 발견");
            }
        }
    }
}
