using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Follow : MonoBehaviour
{
    public List<Transform> positions = new List<Transform>();
    public GameObject player;
    [SerializeField]
    private int currentPosition = 0;

    NavMeshAgent agent;
    public Transform target;
    public float followRange = 20f;
    public float stopDistance = 10f;

    // Shooting variables
    public Transform shotOrigin;
    public ParticleSystem shotParticles;
    public GameObject metalImpactEffect;
    public GameObject stoneImpactEffect;
    private GameObject currentImpactEffect;
    private AudioSource audioSource;
    public AudioClip shotSound;
    public float shootRange = 15f;
    public float fireRate = 1f;
    private bool isOnCooldown = false;

    [SerializeField] LayerMask layersToShoot;


    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();

        player = GameObject.Find("VR Character IK");
    }

    void Update()
    {
        if (player != null && !animator.GetBool("isGettingHit") && !animator.GetBool("isDying"))
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, agent.transform.position);

            if (distanceToPlayer <= followRange)
            {
                if (distanceToPlayer > stopDistance)
                {
                    agent.isStopped = false;
                    agent.destination = player.transform.position;
                }
                else
                {
                    agent.isStopped = true;

                    if (distanceToPlayer <= shootRange)
                    {
                        Vector3 direction = (player.transform.position - transform.position).normalized;
                        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

                        if (!isOnCooldown)
                        {
                            Shoot();
                            StartCoroutine(ManageCooldown());
                        }
                    }
                }
            }
            else
            {
                Patrol();
                animator.SetBool("IsWalking", true);
            }
        }
    }

    void Shoot()
    {
        animator.SetTrigger("IsGunplay");

        RaycastHit hit;

        shotParticles.Play();
        audioSource.PlayOneShot(shotSound);

        if (Physics.Raycast(shotOrigin.position, shotOrigin.TransformDirection(Vector3.forward), out hit, 500, layersToShoot, QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.CompareTag("Door")) return;

            GameObject impactEffectInstance;

            currentImpactEffect = hit.transform.CompareTag("Stone") ? stoneImpactEffect : metalImpactEffect;

            impactEffectInstance = Instantiate(currentImpactEffect, hit.point, Quaternion.LookRotation(hit.normal) * Quaternion.Euler(90, 0, 0));
            impactEffectInstance.transform.SetParent(hit.collider.gameObject.transform);
            Destroy(impactEffectInstance, 15);
        }
    }

    IEnumerator ManageCooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(fireRate);
        isOnCooldown = false;
    }

    void Patrol()
    {
        if (positions.Count == 0)
            return;

        agent.isStopped = false;
        agent.destination = positions[currentPosition].position;

        if (!agent.pathPending && agent.remainingDistance < 0.6f)
        {
            currentPosition++;
            if (currentPosition >= positions.Count)
            {
                currentPosition = 0;
            }
        }
    }
}
