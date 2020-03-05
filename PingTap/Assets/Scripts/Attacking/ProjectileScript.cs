﻿using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
  public GameObject impactParticle;
  public GameObject projectileParticle;
  public GameObject muzzleParticle;
  public GameObject[] trailParticles;

  [Header("Adjust if not using Sphere Collider")]
  public float colliderRadius = 1f;
  [Range(0f, 1f)]
  public float collideOffset = 0.15f;


  Rigidbody rigidbody;


  void Start()
  {
    projectileParticle = Instantiate(projectileParticle, transform.position, transform.rotation) as GameObject;
    projectileParticle.transform.parent = transform;
    if (muzzleParticle)
    {
      muzzleParticle = Instantiate(muzzleParticle, transform.position, transform.rotation) as GameObject;
      Destroy(muzzleParticle, 1.5f); // Lifetime of muzzle effect.
    }

    rigidbody = GetComponent<Rigidbody>();
    var sphereCollider = GetComponent<SphereCollider>();
    colliderRadius = sphereCollider ? sphereCollider.radius : colliderRadius;

  }

  void FixedUpdate()
  {

    Vector3 dir = rigidbody.velocity;
    if (rigidbody.useGravity) dir += Physics.gravity * Time.deltaTime;
    dir = dir.normalized;

    float dist = rigidbody.velocity.magnitude * Time.deltaTime;

    if (Physics.SphereCast(transform.position, colliderRadius, dir, out RaycastHit hit, dist))
    {

      // Move all this cleanup shit into own methods

      transform.position = hit.point + (hit.normal * collideOffset);
      GameObject impactP = Instantiate(impactParticle, transform.position, Quaternion.FromToRotation(Vector3.up, hit.normal));

      foreach (GameObject trail in trailParticles)
      {
        GameObject curTrail = transform.Find(projectileParticle.name + "/" + trail.name).gameObject;
        curTrail.transform.parent = null;
        Destroy(curTrail, 3f);
      }

      Destroy(projectileParticle, 3f);
      Destroy(impactP, 3.5f);
      Destroy(gameObject);

      ParticleSystem[] trails = GetComponentsInChildren<ParticleSystem>();
      for (int i = 1; i < trails.Length; i++)
      {

        ParticleSystem trail = trails[i];

        if (trail.gameObject.name.Contains("Trail"))
        {
          trail.transform.SetParent(null);
          Destroy(trail.gameObject, 2f);
        }
      }

      // Handle hit on target
      var targetDamageController = hit.collider.gameObject.GetComponent<DamageController>();
      if (targetDamageController)
      {
        targetDamageController.TakeDamage(10f);
      }
    }
  }
}