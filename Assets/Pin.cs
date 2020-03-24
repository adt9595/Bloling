using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pin : MonoBehaviour
{
    AudioSource pinAudio;
    public AudioClip pinHit;
    public AudioClip pinHitShort;
    private bool overrideHitSound = false;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        pinAudio = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = Vector3.zero;
        rb.centerOfMass += Vector3.up * 0.022f;
    }

    public void setOverrideHitSound(bool _overrideHitSound) {
        overrideHitSound = _overrideHitSound;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pin"))
        {
            pinAudio.volume = Random.Range(0.08f, 0.2f);
            pinAudio.pitch = Random.Range(0.85f, 0.95f);
            pinAudio.PlayOneShot(pinHitShort);
            Debug.Log(pinAudio.volume);

        }
        else if (collision.gameObject.CompareTag("Ball")) {
            if ((collision.gameObject.transform.position - transform.position).sqrMagnitude < 0.38f && !overrideHitSound) {
                Debug.Log(Vector3.Distance(collision.gameObject.transform.position, transform.position));
                pinAudio.volume = Random.Range(0.68f, 0.85f);
                pinAudio.pitch = Random.Range(0.85f, 1.00f);
                pinAudio.PlayOneShot(pinHit);
                overrideHitSound = true;
            }
        }
    }


}
