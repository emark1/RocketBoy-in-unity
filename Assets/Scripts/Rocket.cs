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


    //Variables
    [SerializeField] float levelLoadDelay = 2f;
    Rigidbody rigidbody;
    AudioSource rocketAudio;
    bool collisionsAreEnabled = true;

    enum State { Alive, Dying, Transcending, Immune }
    State state = State.Alive;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rocketAudio = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (state == State.Alive) {
            Thrust();
            Rotate();
        }
        if (Debug.isDebugBuild) {
            ToggleImmunity();
        }
        
    }

    private void ToggleImmunity() {
        if (Input.GetKeyDown(KeyCode.C)) {
            if (collisionsAreEnabled == true) {
                collisionsAreEnabled = false;
                Debug.Log("Collisions off");
                Debug.Log(collisionsAreEnabled);
            } else if (collisionsAreEnabled == false) {
                collisionsAreEnabled = true;
                Debug.Log("Collisions are on");
                Debug.Log(collisionsAreEnabled);
            }
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
        if (state != State.Alive || collisionsAreEnabled != true) {
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
                Invoke("LoadNextScene", levelLoadDelay);
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
                Invoke("PlayerDeath", levelLoadDelay);
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
