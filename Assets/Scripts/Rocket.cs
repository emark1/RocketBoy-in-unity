using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{

    //Movement
    [SerializeField] float rotateThrust = 10f;
    [SerializeField] float verticalThrust = 10f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip winSound;

    //Particles

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem winParticles;
    [SerializeField] ParticleSystem deathParticles;


    Rigidbody rigidbody;
    AudioSource rocketAudio;

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rocketAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive) {
            Thrust();
            Rotate();
        }
    }

    private void Thrust() {
        if (Input.GetKey(KeyCode.Space)) {
            float thrustSpeed = verticalThrust * Time.deltaTime;
            rigidbody.AddRelativeForce(Vector3.up * verticalThrust);
            if (!mainEngineParticles.isPlaying && state != State.Dying) {
                mainEngineParticles.Play();
            }
            if (!rocketAudio.isPlaying && state != State.Dying) {
                rocketAudio.PlayOneShot(mainEngine);
            } 
            // mainEngineParticles.Play();
        } else {
            rocketAudio.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void Rotate() {
        rigidbody.freezeRotation = true;
        float rotationSpeed = rotateThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.D)) {
            transform.Rotate(-Vector3.forward * rotationSpeed);
        } else if (Input.GetKey(KeyCode.A)) {
            transform.Rotate(Vector3.forward * rotationSpeed);
        }

        rigidbody.freezeRotation = false;
    }

    void OnCollisionEnter(Collision collision) {
        if (state != State.Alive) {
            return;
        }

        switch (collision.gameObject.tag) {
            case "Friendly":
                Debug.Log("Meh");
                break;
            case "Finish":
                state = State.Transcending;
                rocketAudio.Stop();
                mainEngineParticles.Stop();
                winParticles.Play();
                rocketAudio.PlayOneShot(winSound);
                Invoke("LoadNextScene", 1f);
                break;
            case "Fuel": 
                Debug.Log("BIG FUEL");
                break;
            default: 
                state = State.Dying;
                rocketAudio.Stop();
                rocketAudio.PlayOneShot(deathSound);
                mainEngineParticles.Stop();
                deathParticles.Play();
                Invoke("PlayerDeath", 3f);
                break;
        }
    }

    private void PlayerDeath() {
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene() {
        SceneManager.LoadScene(1);
    }


}
