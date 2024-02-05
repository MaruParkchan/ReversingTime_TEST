using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReversingOneObject : MonoBehaviour
{
    // Transform 데이터를 저장하는 클래스
    public class TransformData
    {
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;

        public TransformData(Vector3 position, Vector3 rotation, Vector3 scale)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }
    }

    private Vector3 previousPosition;
    private Vector3 previousRotation;
    private Vector3 previousScale;
    public Transform targetTransform;
    private bool isTimeReset = false;

    // 위치 변화를 저장할 리스트
    //private List<Vector3> positionChanges = new List<Vector3>();
    public List<TransformData> transformDataList = new List<TransformData>();

    void Start()
    {
        // 초기 Transform 값 저장
        SavePreviousTransform();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            isTimeReset = true;
            StartCoroutine(TimeReset());
        }
        if(Input.GetMouseButtonUp(0))
        {   
            isTimeReset = false;
            targetTransform.GetComponent<Rigidbody>().useGravity = true;
            StopAllCoroutines();
        }

        if(isTimeReset)
            return;

        // 현재 프레임의 Transform 값
        Vector3 currentPosition = targetTransform.position;
        Vector3 currentRotation = targetTransform.eulerAngles;
        Vector3 currentScale = targetTransform.localScale;

        // 변동이 0.01 이상인지 체크
        if (IsTransformChanged(currentPosition, currentRotation, currentScale, 0.001f))
        {
            // 변동이 발생한 경우에 수행할 작업
            // 현재 프레임의 Transform 값을 배열에 추가
            SaveTransformData(currentPosition, currentRotation, currentScale);

            // 현재 프레임의 Transform 값을 이전 값으로 업데이트
            SavePreviousTransform();
        }
    }

    void SavePreviousTransform()
    {
        // 현재 프레임의 Transform 값을 이전 값으로 저장
        previousPosition = targetTransform.position;
        previousRotation = targetTransform.eulerAngles;
        previousScale = targetTransform.localScale;
    }

    void SaveTransformData(Vector3 position, Vector3 rotation, Vector3 scale)
    {
        // 위치, 회전, 크기 변화를 리스트에 추가
        transformDataList.Add(new TransformData(position, rotation, scale));
    }

    bool IsTransformChanged(Vector3 currentPosition, Vector3 currentRotation, Vector3 currentScale, float threshold)
    {
        // 변동 여부 체크
        return Vector3.Distance(currentPosition, previousPosition) >= threshold ||
               Vector3.Distance(currentRotation, previousRotation) >= threshold ||
               Vector3.Distance(currentScale, previousScale) >= threshold;
    }

    IEnumerator TimeReset()
    {
        while(true)
        {
        if(transformDataList.Count <= 1 || isTimeReset == false)
        {
            ObjectReset();
            yield break;
        }
    
        targetTransform.position = transformDataList[transformDataList.Count -1].position;
        targetTransform.eulerAngles = transformDataList[transformDataList.Count -1].rotation;
        targetTransform.localScale = transformDataList[transformDataList.Count -1].scale;
        
        transformDataList.RemoveAt(transformDataList.Count -1);
        yield return new WaitForSeconds(0.005f);
        }
    }

    private void ObjectReset()
    {
        Rigidbody rigidbody = targetTransform.GetComponent<Rigidbody>();
        targetTransform.position = transformDataList[0].position;
        targetTransform.eulerAngles = transformDataList[0].rotation;
        targetTransform.localScale = transformDataList[0].scale;
        rigidbody.useGravity = false;
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
    }
}
