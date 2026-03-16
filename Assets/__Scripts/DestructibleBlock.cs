using UnityEngine;

public class DestructibleBlock : MonoBehaviour
{
    [Header("Settings")]
    public float breakVelocityThreshold = 5f;  // minimum projectile speed to break
    public AudioClip breakSound;               // sound to play when block breaks
    public float soundVolume = 1f;             // adjust volume

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            float impactSpeed = collision.relativeVelocity.magnitude;

            if (impactSpeed >= breakVelocityThreshold)
            {
                BreakBlock();
            }
        }
    }

    void BreakBlock()
    {
        if (breakSound != null)
        {
            AudioSource.PlayClipAtPoint(breakSound, transform.position, soundVolume);
        }

        Destroy(gameObject);
    }
}