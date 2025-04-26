using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class playercontrols : MonoBehaviour
{
    public GameObject caughtText;
    public GameObject restartButton;
    public GameObject winText;

    public float speed = 10f;
    public float jumpForce = 5f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public int count;
    public TextMeshProUGUI countText;

    public int totalPickups = 39;

    private Rigidbody rb;
    private Vector2 movementInput;
    private bool isGrounded;
    private bool isFrozen = false;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;

    public AudioSource pickupAudioSource;
    public AudioClip pickupSound;

    public AudioSource caughtAudioSource;
    public AudioClip caughtSound;

    public AudioSource backgroundMusicSource;
    public AudioClip backgroundMusic;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        count = 0;
        SetCountText();

        if (backgroundMusicSource != null && backgroundMusic != null)
        {
            backgroundMusicSource.clip = backgroundMusic;
            backgroundMusicSource.Play();
        }
    }

    void OnEnable()
    {
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        jumpAction.performed += OnJump;
    }

    void OnDisable()
    {
        jumpAction.performed -= OnJump;
    }

    void SetCountText()
    {
        countText.text = "Picked Up: " + count.ToString() + " / " + totalPickups.ToString();

        if (count >= totalPickups)
        {
            if (winText != null)
                winText.SetActive(true);

            if (restartButton != null)
                restartButton.SetActive(true);
        }
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (isFrozen) return;

        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        if (isFrozen) return;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        movementInput = moveAction.ReadValue<Vector2>();
        Vector3 movement = new Vector3(movementInput.x, 0.0f, movementInput.y);
        rb.AddForce(movement * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count++;
            SetCountText();

            // ✅ Play Pickup Sound
            if (pickupAudioSource != null && pickupSound != null)
            {
                pickupAudioSource.PlayOneShot(pickupSound);
            }
        }
    }


    public void FreezePlayer()
    {
        isFrozen = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.Sleep();

        // ✅ Play caught sound
        if (caughtAudioSource != null && caughtSound != null)
        {
            caughtAudioSource.PlayOneShot(caughtSound);
        }

        if (caughtText != null)
            caughtText.SetActive(true);

        if (restartButton != null)
            restartButton.SetActive(true);
    }


    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}




