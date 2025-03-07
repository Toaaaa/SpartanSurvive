using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraRaycaster : MonoBehaviour
{
    public float rayDistance = 6.6f; // Ray의 거리
    public LayerMask raycastLayerMask; // 레이캐스트를 체크할 레이어 (Object 레이어만 체크)

    [SerializeField] TextMeshProUGUI objecInfo;
    private Camera mainCamera;
    private Player player;


    void Start()
    {
        mainCamera = GetComponent<Camera>();
        player = GameManager.Instance.player;
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
                // UI에 정보 표시
                objecInfo.text = hit.collider.GetComponent<Interactable>().GetInfo();
                if(Input.GetKeyDown(KeyCode.E)) // E키를 누르면 상호작용.
                {
                    hit.collider.GetComponent<Interactable>().Interact();
                }
            }
            else if (hit.collider.CompareTag("Items"))// 테그가 Items인 경우
            {
                // UI에 정보 표시
                objecInfo.text = hit.collider.GetComponent<ItemObject>().itemData.GetInfo();
                if(Input.GetKeyDown(KeyCode.E)) // E키를 누르면 아이템 획득.
                {
                    hit.collider.GetComponent<ItemObject>().itemData.Use(player);
                    Destroy(hit.collider.gameObject);
                }
            }
            else
            {
                objecInfo.text = "";
            }
        }
        else
        {
            objecInfo.text = "";
        }
    }
}
