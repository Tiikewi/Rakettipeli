using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

    Rigidbody rigidBody;
    AudioSource audioSource;

    [SerializeField] float thrustSpeed = 20f; 
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip death;
    [SerializeField] AudioClip success;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem successParticles;

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;
    bool collisionDisabled = false;


    // Use this for initialization
    void Start () {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

    }
	
	// Update is called once per frame
	void Update () {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            ResponToRotateInput();
        }
        if (Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionDisabled = !collisionDisabled; //toggle.
        }
    }

    void OnCollisionEnter(Collision collision)
    {

        if (state != State.Alive || collisionDisabled) { return; }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                // Do nothing.
                break;
            case "Finish":
                StartSuccessSequence();
                break;

            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        deathParticles.Play();
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    private void StartSuccessSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        successParticles.Play();
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        int numberOfScenes = SceneManager.sceneCountInBuildSettings;

        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex);
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * thrustSpeed * Time.deltaTime);

        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }

    private void ResponToRotateInput()
    {


        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            rigidBody.freezeRotation = true; // Take manual control of rotation.
            transform.Rotate(Vector3.forward * rotationThisFrame);
            rigidBody.freezeRotation = false; // Resume physichs control rotation.
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rigidBody.freezeRotation = true; // Resume physichs control rotation.
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        rigidBody.freezeRotation = false; // Resume physichs control rotation.
        }

    }



}
