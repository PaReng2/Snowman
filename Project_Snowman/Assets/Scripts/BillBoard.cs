using UnityEngine;

public class BillBoard : MonoBehaviour
{
    private Transform mainCameraTransform;

    void Start()
    {
        // 씬에서 Main Camera 태그를 가진 카메라의 Transform을 찾습니다.
        // 게임 시작 시 한 번만 찾는 것이 성능에 효율적입니다.
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
        else
        {
            Debug.LogError("씬에 'MainCamera' 태그가 지정된 카메라가 없습니다.");
        }
    }

    void LateUpdate()
    {
        // LateUpdate에서 실행하여 모든 오브젝트의 이동이 끝난 후 카메라를 바라보게 합니다.
        if (mainCameraTransform != null)
        {
            
            transform.LookAt(transform.position + mainCameraTransform.rotation * Vector3.forward,
                             mainCameraTransform.rotation * Vector3.up);

            
            transform.rotation = mainCameraTransform.rotation;
        }
    }
}