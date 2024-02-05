using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float explosionForce = 1000f;
    public float explosionRadius = 5f;

    void Start()
    {

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A)) // 키 눌렀을대 폭발 효과
        {
            Explode();
        }
    }

    void Explode()
    {
        // 폭발 위치
        Vector3 explosionPos = transform.position;

        // 폭발 반경 내의 Collider 배열 가져오기
        Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);

        // 각 Collider에 폭발 효과 적용
        foreach (Collider hit in colliders)
        {
            // Rigidbody 컴포넌트 가져오기
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
            {
                // 폭발 힘 적용
                rb.AddExplosionForce(explosionForce, explosionPos, explosionRadius);
            }
        }
    }
}
