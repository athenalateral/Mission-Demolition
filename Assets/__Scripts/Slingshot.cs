using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    [Header("Inscribed")]
    public GameObject projectilePrefab;
    public float velocityMult = 10f;
    public GameObject projLinePrefab;

    public LineRenderer lineRenderer;
    public Transform leftAnchor;
    public Transform rightAnchor;

    [Header("Audio")]
    public AudioClip pullSound;
    public AudioClip releaseSound;
    private AudioSource audioSource;

    [Header("Dynamic")]
    public GameObject launchPoint;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;
    private Transform ball;

    void Awake()
    {
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;

        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 3;
            lineRenderer.enabled = false;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void OnMouseEnter()
    {
        launchPoint.SetActive(true);
    }

    void OnMouseExit()
    {
        launchPoint.SetActive(false);
    }

    void OnMouseDown()
    {
        aimingMode = true;

        // Instantiate projectile
        projectile = Instantiate(projectilePrefab);
        projectile.transform.position = launchPos;
        projectile.GetComponent<Rigidbody>().isKinematic = true;

        // Assign ball transform for LineRenderer
        ball = projectile.transform;

        // Enable LineRenderer
        if (lineRenderer != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, leftAnchor.position);
            lineRenderer.SetPosition(1, ball.position);
            lineRenderer.SetPosition(2, rightAnchor.position);
        }

        // Play pull sound
        if (audioSource != null && pullSound != null)
        {
            audioSource.clip = pullSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void Update()
    {
        if (!aimingMode || projectile == null) return;

        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        Vector3 mouseDelta = mousePos3D - launchPos;

        float maxMagnitude = 3f; 
        if (mouseDelta.magnitude > maxMagnitude)
        {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }

        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;

        if (lineRenderer != null && ball != null)
        {
            lineRenderer.SetPosition(0, leftAnchor.position);
            lineRenderer.SetPosition(1, ball.position);
            lineRenderer.SetPosition(2, rightAnchor.position);
        }

        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.pitch = 1f + mouseDelta.magnitude * 0.3f;
        }

        if (Input.GetMouseButtonUp(0))
        {
            aimingMode = false;

            if (audioSource != null && pullSound != null)
            {
                audioSource.Stop();
                audioSource.loop = false;
            }

            if (audioSource != null && releaseSound != null)
            {
                audioSource.PlayOneShot(releaseSound);
            }

            Rigidbody projRB = projectile.GetComponent<Rigidbody>();
            projRB.isKinematic = false;
            projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
            projRB.velocity = -mouseDelta * velocityMult;

            FollowCam.SWITCH_VIEW(eView.slingshot);
            FollowCam.POI = projectile;

            if (projLinePrefab != null)
            {
                Instantiate(projLinePrefab, projectile.transform);
            }

            projectile = null;
            ball = null;
            if (lineRenderer != null)
            {
                lineRenderer.enabled = false;
            }
            MissionDemolition.SHOT_FIRED();
        }
    }
}