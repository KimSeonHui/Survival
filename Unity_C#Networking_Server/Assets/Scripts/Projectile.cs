﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>폭탄</summary>
public class Projectile : MonoBehaviour
{
    public static Dictionary<int, Projectile> projectiles = new Dictionary<int, Projectile>();
    private static int nextProjectileId = 1;

    public int id;
    public Rigidbody rigidBody;             
    public int thrownByPlayer;              
    public Vector3 initialForce;            //발사벡터
    public float explosionRadius = 1.5f;    //폭발범위
    public float explosionDamage = 75f;     //폭발데미지
    public bool EMPInstallFinished;         //EMPZone에 emp 설치 완료

    private void Start()
    {
        id = nextProjectileId;
        nextProjectileId++;
        projectiles.Add(id, this);

        ServerSend.SpawnProjectile(this, thrownByPlayer);

        rigidBody.AddForce(initialForce);
        StartCoroutine(ExplodeAfterTime());
    }

    private void FixedUpdate()
    {
        ServerSend.ProjectilePosition(this);
    }

    /// <summary>충돌체크 후 폭발</summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        try
        {
            if (collision.gameObject.GetComponent<Player>().playerType == PlayerType.CREATURE)
            {
                Explode();
                ServerSend.KeyChange(collision.gameObject.GetComponentInParent<Player>().id);
            }
        }
        catch (System.Exception e)
        {
            Explode();
        }
        /*
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            if (collision.gameObject.GetComponent<Player>().playerType == PlayerType.CREATURE)
            {
                Explode();
            }
        }*/
    }

    /// <summary>초기 발사상태</summary>
    /// <param name="_initialMovementDirection">초기 방향</param>
    /// <param name="_initialForceStrength">초기 세기</param>
    /// <param name="_thrownByPlayer">발사한 사람</param>
    public void Initialize(Vector3 _initialMovementDirection, float _initialForceStrength, int _thrownByPlayer)
    {
        initialForce = _initialMovementDirection * _initialForceStrength;
        thrownByPlayer = _thrownByPlayer;
    }

    /// <summary>폭발</summary>
    private void Explode()
    {
        ServerSend.ProjectileExploded(this);

        Collider[] _colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        /*foreach (Collider _collider in _colliders)
        {
            if (_collider.GetComponentInParent<Player>() != null && _collider.GetComponentInParent<Player>().playerType == PlayerType.CREATURE)
            {
                //_collider.GetComponent<Player>().TakeDamage(explosionDamage);
                ServerSend.KeyChange(_collider.GetComponentInParent<Player>().id);
            }
        }*/

        projectiles.Remove(id); //폭탄 dictionary 삭제
        Destroy(gameObject);    //폭탄 object 파괴
    }

    /// <summary>딜레이시간 후 폭발</summary>
    /// <returns></returns>
    private IEnumerator ExplodeAfterTime()
    {
        yield return new WaitForSeconds(10f);

        Explode();
    }
}
