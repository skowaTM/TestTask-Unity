using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    private GameManager gameManager;
    private MyInputSystem myInputSystem;

    [Header("Game Objects")]
    [SerializeField] private GameObject cameraObject;

    [Header("Player Settings")]
    [SerializeField, Range(1f, 10f)] private float movementSpeed = 5f;          // player movement speed
    [SerializeField, Range(5f, 10f)] private float maxHealth = 5;               // maximum player healh points

    [Header("Camera Settings")]
    [SerializeField, Range(5f, 20f)] private float cameraDistance = 10f;        // distance between player and camera
    [SerializeField, Range(1f, 5f)] private float cameraMouseSpeed = 3f;        // camrea rotating speed by mouse
    [SerializeField, Range(1f, 5f)] private float cameraStickSpeed = 3f;        // camrea rotating speed by joystick

    private float startFaceYAngle;          // 'face' direction angle on start
    private float defaultMovementSpeed;     // default player movement speed
    private float currentHealth;            // current player HP

    public float BoosterTime { get; private set; } = 0f;


    private void Awake()
    {
        startFaceYAngle = transform.localEulerAngles.y;
        defaultMovementSpeed = movementSpeed;
        currentHealth = maxHealth;
    }


    private void Start()
    {
        gameManager = GameManager.Instance;
        myInputSystem = new MyInputSystem(
            gameManager.UIController.StickMovement.transform,
            gameManager.UIController.StickLook.transform,
            gameManager.UIController.StickRange
        );

        gameManager.UIController.UpdateHealthBar(currentHealth, maxHealth);
    }


    private void Update()
    {
        myInputSystem.CheckInputs();

        BoosterTime = Mathf.Clamp(BoosterTime - Time.deltaTime, 0f, BoosterTime);
    }


    private void FixedUpdate()
    {
        ProcessPlayerMovement();
    }


    private void LateUpdate()
    {
        if (!gameManager.PauseManager.IsPaused)
        {
            ProccesCameraMovement();
        }
    }


    // Precessing of player movement
    private void ProcessPlayerMovement()
    {
        Vector2 input = myInputSystem.KeyboardInput + myInputSystem.StickMovementInput;

        if (input != Vector2.zero)
        {
            // Projection on plane of direction vector between player and camera
            Vector3 cameraDir = Vector3.ProjectOnPlane(transform.position - cameraObject.transform.position, Vector3.up).normalized;

            // Angle of input direction vector
            float alpha = Mathf.Deg2Rad * (Mathf.Sign(input.y) * Vector2.Angle(Vector2.right, input) + startFaceYAngle);
            // Rotation of 'cameraDir' vector by 'alpha' degree on XZ axis
            float x = Mathf.Cos(alpha) * cameraDir.x - Mathf.Sin(alpha) * cameraDir.z;
            float z = Mathf.Sin(alpha) * cameraDir.x + Mathf.Cos(alpha) * cameraDir.z;

            Vector3 moveDir = new Vector3(x, 0f, z);            // movement direction
            moveDir *= Mathf.Min(input.magnitude, 1f);          // scaling by 'input' vector length
            Vector3 newPos = Vector3.Lerp(                      // supposed new position
                transform.position, 
                transform.position + movementSpeed * moveDir, 
                Time.deltaTime
            );
            // clamping newPos coords according to walking area bounds
            newPos.x = Mathf.Clamp(newPos.x, gameManager.WalkingAreaBounds["min_x"], gameManager.WalkingAreaBounds["max_x"]);
            newPos.z = Mathf.Clamp(newPos.z, gameManager.WalkingAreaBounds["min_z"], gameManager.WalkingAreaBounds["max_z"]);

            // Setting new position and 'face' direction angle
            transform.position = newPos;
            transform.localEulerAngles = new Vector3(0f, -Mathf.Sign(moveDir.z) * Vector3.Angle(Vector3.right, moveDir));
        }
    }


    // Processing of camera movement
    private void ProccesCameraMovement()
    {
        if (myInputSystem.StickMovementInput == Vector2.zero || myInputSystem.StickLookInput != Vector2.zero)
        {
            // Getting rotation directions
            float dirX = cameraMouseSpeed * myInputSystem.MouseInput.x;
            float dirY = cameraMouseSpeed * myInputSystem.MouseInput.y;
            if (myInputSystem.StickLookInput != Vector2.zero)
            {
                dirX = myInputSystem.StickLookInput.x * cameraStickSpeed / 5f;
                dirY = -myInputSystem.StickLookInput.y * cameraStickSpeed / 5f;
            }

            // Rotation angles
            float x = cameraObject.transform.localEulerAngles.y + dirX;
            float y = cameraObject.transform.localEulerAngles.x + dirY;
            y = Mathf.Clamp(y, 0f, 80f);        // clamping Y angle so camera won't move under feet or behind head

            // Setting calculating rotation angles
            cameraObject.transform.localEulerAngles = new Vector3(y, x, 0f);
        }

        // Setting camera position according to its rotation and player position
        cameraObject.transform.position = cameraObject.transform.localRotation * new Vector3(0f, 0f, -cameraDistance) + transform.position;
    }


    // Called when player takes booster item
    public void TookBooster(float multiplier, float duration)
    {
        IEnumerator Wrapper()
        {
            BoosterTime += duration;
            while (BoosterTime > 0f)
            {
                movementSpeed = multiplier * defaultMovementSpeed;
                yield return new WaitForSeconds(BoosterTime);
                movementSpeed = defaultMovementSpeed;
            }
        }

        StartCoroutine(Wrapper());
    }

    // Called when player takes damage item
    public void TookDamage(float damage)
    {
        currentHealth -= damage;
        gameManager.UIController.UpdateHealthBar(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            StartCoroutine(gameManager.OnPlayerDeath());
        }
    }

    // Called when player takes note item
    public void TookNote(string noteText)
    {
        gameManager.OnToShowNote(noteText);
    }
}
