using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shooter : MonoBehaviour
{
    public InputActionReference triggerInputActionReference;
    public Transform ShotOrigin;
    public ParticleSystem ShotParticles;
    public GameObject MetalImpactEffect;
    public GameObject FleshImpactEffect;
    public GameObject StoneImpactEffect;
    GameObject CurrentImpactEffect;
    GameObject HitPoint;
    AudioSource audioSource;
    public AudioClip shotSound;
    public float cooldown = .33f;
    bool isOnCooldown = false;
    bool isHoldingTrigger = false;

    [SerializeField] LayerMask layersToShoot;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (triggerInputActionReference.action.ReadValue<float>() == 1)
        {
            if (!isOnCooldown && !isHoldingTrigger)
            {
                isHoldingTrigger = true;
                StartCoroutine(ManageCooldown());
                Shoot();
            }
        }
        else if (isHoldingTrigger)
        {
            isHoldingTrigger = false;
        }
    }

    void Shoot()
    {
        RaycastHit hit;

        ShotParticles.Play();
        audioSource.PlayOneShot(shotSound);
        //if (Physics.Raycast(ShotOrigin.position, ShotOrigin.TransformDirection(Vector3.forward), out hit, 500, LayerMask.GetMask("Enemy")))
        if (Physics.Raycast(ShotOrigin.position, ShotOrigin.TransformDirection(Vector3.forward), out hit, 500, layersToShoot, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.CompareTag("Door")) return;

            GameObject ImpactEffectInstance;
            int effectLifetime = 15;

            if (hit.transform.CompareTag("Enemy"))
            {
                CurrentImpactEffect = FleshImpactEffect;
                hit.transform.GetComponent<EnemyHit>().GetHit();
                effectLifetime = 2;
            }
            else
            {
                CurrentImpactEffect = hit.transform.CompareTag("Stone") ? StoneImpactEffect : MetalImpactEffect;

                Rigidbody target = hit.rigidbody;
                if (target != null)
                {
                    target.AddForce(Vector3.forward * 5, ForceMode.Impulse);
                }
            }

            ImpactEffectInstance = Instantiate(CurrentImpactEffect, hit.point, Quaternion.LookRotation(hit.normal) * Quaternion.Euler(90, 0, 0)) as GameObject;
            ImpactEffectInstance.transform.SetParent(hit.collider.gameObject.transform);
            Destroy(ImpactEffectInstance, effectLifetime);
        }
    }

    IEnumerator ManageCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldown);
        isOnCooldown = false;
    }
}
