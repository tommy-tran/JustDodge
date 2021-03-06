﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float runSpeed = 2f;
    public float walkSpeed = 0.2f;
    public bool isAlive;
    public GameController gameController;
    public GameObject[] hearts;
    public SoundController soundController;
    public bool invulnerable;
    public int health;

    Vector3 movement;
    Animator anim;
    Rigidbody playerRigidbody;
    
    float v;
    float h;


    void Awake()
    {
        health = 3;
        isAlive = true;
        anim = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
        updateHealth();
        invulnerable = false;
    }

    void FixedUpdate()
    {
        if (isAlive)
        {
            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");
            Move(h, v);
            Animating(h, v);
        }

    }

    void Move(float h, float v)
    {
        movement.Set(h, 0f, v);
        movement = Input.GetButton("Shift") ? movement.normalized * walkSpeed * Time.deltaTime : movement.normalized * runSpeed * Time.deltaTime;
        if (movement != Vector3.zero)
        {
            playerRigidbody.MovePosition(transform.position + movement);
            restrictMovement();
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15f);
        }
    }

    void updateHealth()
    {
        int temp = health;
        foreach (GameObject heart in hearts)
        {
            if (temp-- >= 1)
            {
                heart.SetActive(true);
            } else
            {
                heart.SetActive(false);
            }
        }
    }

    void restrictMovement()
    {
        float x = Mathf.Clamp(playerRigidbody.position.x, -8f, 8f);
        float z = Mathf.Clamp(playerRigidbody.position.z, -4.25f, 4.25f);
        playerRigidbody.position = new Vector3(x, playerRigidbody.position.y, z);
    }

    void Animating(float h, float v)
    {
        bool IsWalking = Input.GetButton("Shift");
        bool IsMoving = h != 0f || v != 0f;
        
        anim.SetBool("IsWalking", IsWalking);
        anim.SetBool("IsMoving", IsMoving);
    }

    void OnTriggerEnter(Collider other)
    {
        if (isAlive && !invulnerable)
        {
            if (other.CompareTag("Enemy"))
            {
                PlayerDamage();
            } else if (other.CompareTag("Minion"))
            {
                PlayerDamage();
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (isAlive && !invulnerable)
        {
            if (other.CompareTag("Enemy"))
            {
                PlayerDamage();
            }
            else if (other.CompareTag("Minion"))
            {
                PlayerDamage();
            }
        }
    }

    public void PlayerDamage()
    {
        soundController.playDamageSound();
        health--;
        updateHealth();
        if (health < 1)
        {
            PlayerDeath();
        } else
        {
            StartCoroutine(invulnerability());
        }

    }

    public void PlayerHeal()
    {
        if (health < 3 && isAlive)
        {
            soundController.playHealSound();
            health++;
            updateHealth();
        }
    }

    IEnumerator invulnerability()
    {
        invulnerable = true;
        Renderer renderer = gameObject.GetComponentInChildren<Renderer>();
        for (int i = 0; i < 3; i++)
        {
            renderer.enabled = false;
            yield return new WaitForSeconds(0.2f);
            renderer.enabled = true;
            yield return new WaitForSeconds(0.2f);
        }
        invulnerable = false;
    }

    void PlayerDeath()
    {
        isAlive = false;
        anim.SetBool("IsDead", true);
        gameController.GameOver();
    }

    public void retry()
    {
        health = 3;
        updateHealth();
        isAlive = true;
        anim.SetBool("IsDead", false);
    }
}