﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{

    [SerializeField] float rotateThrust = 10f;
    [SerializeField] float verticalThrust = 10f;

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
        Thrust();
        Rotate();
    }

    private void Thrust() {
        if (Input.GetKey(KeyCode.Space)) {
            float thrustSpeed = verticalThrust * Time.deltaTime;
            rigidbody.AddRelativeForce(Vector3.up * verticalThrust);
            if (!rocketAudio.isPlaying) {
                rocketAudio.Play();
            } 
        } else {
            rocketAudio.Stop();
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
        switch (collision.gameObject.tag) {
            case "Friendly":
                Debug.Log("Meh");
                break;
            case "Finish":
                Debug.Log("Mission complete");
                Invoke("LoadNextScene", 1f);
                break;
            case "Fuel": 
                Debug.Log("BIG FUEL");
                break;
            default: 
                SceneManager.LoadScene(0);
                break;
        }
    }

    private void LoadNextScene() {
        SceneManager.LoadScene(1);
    }

}
