using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReversingAllObject : MonoBehaviour
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

    public List<Transform> targetTransforms; // 다중 오브젝트를 위한 리스트
    private List<Vector3> previousPositions = new List<Vector3>();
    private List<Vector3> previousRotations = new List<Vector3>();
    private List<Vector3> previousScales = new List<Vector3>();
    public bool isTimeReset = false;
    public float threshold = 1.0f; // 오브젝트 위치, 회전, 크기가  이 변수값 설정보다 이상으로 변경될때

    // 위치, 회전, 크기 변화를 리스트에 추가
    private List<List<TransformData>> transformDataLists = new List<List<TransformData>>();

    void Start()
    {
        // 초기 Transform 값 저장

        SavePreviousTransforms();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isTimeReset = true;
            StartCoroutine(TimeReset());
        }
        if (Input.GetMouseButtonUp(0))
        {
            isTimeReset = false;
            StopAllCoroutines();
        }

        if (isTimeReset)
            return;

        // 현재 프레임의 Transform 값
        Vector3[] currentPositions = new Vector3[targetTransforms.Count];
        Vector3[] currentRotations = new Vector3[targetTransforms.Count];
        Vector3[] currentScales = new Vector3[targetTransforms.Count];

        for (int i = 0; i < targetTransforms.Count; i++)
        {
            currentPositions[i] = targetTransforms[i].position;
            currentRotations[i] = targetTransforms[i].eulerAngles;
            currentScales[i] = targetTransforms[i].localScale;
        }

        // 변동이 0.01 이상인지 체크
        if (IsTransformChanged(currentPositions, currentRotations, currentScales))
        {
            // 변동이 발생한 경우에 수행할 작업
            // 현재 프레임의 Transform 값을 배열에 추가
            SaveTransformData(currentPositions, currentRotations, currentScales);

            // 현재 프레임의 Transform 값을 이전 값으로 업데이트
            SavePreviousTransforms();
        }
    }

    void SavePreviousTransforms()
    {
        // 현재 프레임의 Transform 값을 이전 값으로 저장
        previousPositions.Clear();
        previousRotations.Clear();
        previousScales.Clear();

        foreach (Transform targetTransform in targetTransforms)
        {
            previousPositions.Add(targetTransform.position);
            previousRotations.Add(targetTransform.eulerAngles);
            previousScales.Add(targetTransform.localScale);
        }
    }

    void SaveTransformData(Vector3[] positions, Vector3[] rotations, Vector3[] scales)
    {
        // 위치, 회전, 크기 변화를 리스트에 추가
        List<TransformData> frameTransformData = new List<TransformData>();

        for (int i = 0; i < targetTransforms.Count; i++)
        {
            frameTransformData.Add(new TransformData(positions[i], rotations[i], scales[i]));
        }

        transformDataLists.Add(frameTransformData);
    }

    bool IsTransformChanged(Vector3[] currentPositions, Vector3[] currentRotations, Vector3[] currentScales)
    {
        // 변동 여부 체크
        for (int i = 0; i < targetTransforms.Count; i++)
        {
            if (Vector3.Distance(currentPositions[i], previousPositions[i]) >= threshold ||
                Vector3.Distance(currentRotations[i], previousRotations[i]) >= threshold ||
                Vector3.Distance(currentScales[i], previousScales[i]) >= threshold)
            {
                Debug.Log(Vector3.Distance(currentPositions[i], previousPositions[i]) + Vector3.Distance(currentRotations[i], previousRotations[i]) + Vector3.Distance(currentScales[i], previousScales[i]));
                return true;
            }
        }

        return false;
    }

    IEnumerator TimeReset()
    {
        while (true)
        {
            if (transformDataLists.Count <= 1 || isTimeReset == false)
            {
                ObjectReset();
                yield break;
            }

            for (int i = 0; i < targetTransforms.Count; i++)
            {
                targetTransforms[i].position = transformDataLists[transformDataLists.Count - 1][i].position;
                targetTransforms[i].eulerAngles = transformDataLists[transformDataLists.Count - 1][i].rotation;
                targetTransforms[i].localScale = transformDataLists[transformDataLists.Count - 1][i].scale;
            }

            transformDataLists.RemoveAt(transformDataLists.Count - 1);
            yield return new WaitForSeconds(0.0005f);
        }
    }

    private void ObjectReset()
    {
        for (int i = 0; i < targetTransforms.Count; i++)
        {
            targetTransforms[i].position = transformDataLists[0][i].position;
            targetTransforms[i].eulerAngles = transformDataLists[0][i].rotation;
            targetTransforms[i].localScale = transformDataLists[0][i].scale;
        }

        Rigidbody[] rigidbodies = new Rigidbody[targetTransforms.Count];
        for (int i = 0; i < targetTransforms.Count; i++)
        {
            rigidbodies[i] = targetTransforms[i].GetComponent<Rigidbody>();
            rigidbodies[i].velocity = Vector3.zero;
            rigidbodies[i].angularVelocity = Vector3.zero;
        }
    }
}
