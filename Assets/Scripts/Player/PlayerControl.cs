using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour
{
    public static PlayerControl Instance;

    public ParticleSystem dust;

    [Header("Movimiento Horizontal")]
    [SerializeField] private float movementSpeed = 6f;
    private Vector2 input;

    [Header("Salto")]
    [SerializeField] private float jumpForce = 13.5f;
    private int maxJumps = 1;
    [SerializeField] private int jumpCount = 0;
    [SerializeField] private float timerJumpAnimCorrection;
    public bool isGrounded = false;
    private float jumpTimer;
    public LayerMask groundLayer;

    [Header("Salto largo y corto")]
    [SerializeField] private float shortJump;
    [SerializeField] private float longJump;

    [Header("Recibir dano")]
    public bool canMove = true;
    public bool canApplyForce = true;
    [SerializeField] private Vector2 reboundSpeed;
    [SerializeField] private Vector2 reboundSpeedJumping;

    [Header("Detectar suelo")]
    [SerializeField] private Transform overlapPosition;
    [SerializeField] private Vector3 boxSize;

    [Header("Audios")]
    AudioManager audioManager;

    //Components
    private Rigidbody2D rb;
    private Animator anim;

    private void Awake() {
        audioManager = GameObject.FindGameObjectWithTag(GameTags.audio).GetComponent<AudioManager>();
        HideCursor();
    }

    private void HideCursor()
    {
        //Se oculta el cursor y se bloquea en medio de la pantalla
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Start()
    {
        this.gameObject.SetActive(true);
        Settings.isDead = false;        
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }


    void Update()
    {
        // No detecta el input si el juego está pausado
        if (MenuInGame.juegoPausado)
            return;

        //Quitar este input al terminar el juego
        if (Input.GetKeyDown(KeyCode.G))
        {
            StartCoroutine(ChangeScene());
        }

        if (canApplyForce)
        {
            MovementForces();
        }

        //Correccion del bug de la animacion de salto mientras andabamos
        if (isGrounded)
        {
            StartCoroutine(JumpAnimCorrection());
        }
    }

    private void FixedUpdate()
    {
        if (canMove && !Settings.isDead)
        {
            Movement();
        }
    }

    private void MovementForces()
    {
        //Detectamos cuando nos movemos horizontalmente y guardamos el valor en un float para usarlo m�s adelante
        input = new Vector2(Input.GetAxisRaw(GameTags.horizontalMove), 0);
        GroundDetector();
        PlayerJump();
    }

    private void GroundDetector()
    {
        //Detectamos si estamos en el suelo o no
        Collider2D overlapInfo = Physics2D.OverlapBox(overlapPosition.position, boxSize, 0, groundLayer);

        //En caso de tocar, reiniciamos los saltos que tenemos
        if (overlapInfo != null)
        {
            if (!isGrounded && jumpTimer <= 0)
            {
                isGrounded = true;
                jumpCount = maxJumps;
                anim.SetBool(GameTags.playerJumpAnim, false);
            }            
        }
        else if (isGrounded == true)
        {
            isGrounded = false;
        }

        if (isGrounded == false && jumpTimer > 0f)
        {
            jumpTimer -= Time.deltaTime;
        }
    }

    private void PlayerJump()
    {
        //Aplicamos salto cuando se pulsa bot�n y tenemos saltos disponibles
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount > 0)
        {
            audioManager.PlaySFX(audioManager.medusaJump);
            stopDust();
            isGrounded = false;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0, jumpForce));
            anim.SetBool(GameTags.playerJumpAnim, true);
            jumpCount--;
            jumpTimer = 0.02f;
        }

        //Mejora de salto, salto largo y corto.
        //El if es para que cuando se detecte que el jugador esta cayendo y aplique la fuerza de la gravedad normal, por eso el valor de saltoCorto es 1
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * longJump * Time.deltaTime;
        }
        //Si estamos saltando pero no estamos pulsando el boton, aplicamos una fuerza mayor en Y para que el jugador deje de subir antes y entre en ca�da
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * shortJump * Time.deltaTime;
        }
    }

    //Corrutina para cancelar la animacion de salto cuando estamos en el suelto. Esto nos corrige un bug que mantenia la animacion de salto mientras andabamos
    private IEnumerator JumpAnimCorrection()
    {
        yield return new WaitForSeconds(timerJumpAnimCorrection);
        anim.SetBool(GameTags.playerJumpAnim, false);
    }

    private void Movement()
    {

        if (rb.velocity.y > 0) {
            anim.SetBool(GameTags.playerJumpAnim, true);
            rb.velocity = new Vector2(input.x * movementSpeed, rb.velocity.y);
            PlayerRotation();
        } else {
            rb.velocity = new Vector2(input.x * movementSpeed, rb.velocity.y);
            PlayerRotation();
        }
        
    }

    private void PlayerRotation()
    {
       
        if (canMove)
        {
            //Detectamos hacia que lado nos movemos y aplicamos la rotacion para que el jugador mir� hacia la direccion correcta. Tambi�n detectamos la animacion de correr o idle
            if (rb.velocity.x > 0)
            {
                createDust();
                this.transform.rotation = Quaternion.Euler(0, 0, 0);
                anim.SetBool(GameTags.playerMoveAnim, true);
                
            }
            else if (rb.velocity.x < 0)
            {
                createDust();
                this.transform.rotation = Quaternion.Euler(0, 180, 0);
                anim.SetBool(GameTags.playerMoveAnim, true);
            }
            else
            {
               
                anim.SetBool(GameTags.playerMoveAnim, false);
                
            }
        }
    }

    //Genera un rebote hacia al lado opuesto de lo que chocamos
    public void Rebound(Vector2 puntoGolpe)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.velocity = new Vector2(-reboundSpeed.x * puntoGolpe.x, reboundSpeed.y); //Hay un error, si en el mismo momento que contactas con el enemigo, el jugador salta, salta mucho m�s alto de lo normal     
    }

    //Este método hará que el jugador retroceda un poco en función de hacia dónde esté mirando.
    public void ApplyRecoil(float recoilForce)
    {
        StartCoroutine(Recoil(recoilForce));
    }

    private IEnumerator Recoil(float force)
    {
        canMove = false;
        canApplyForce = false;
        // Determinamos la dirección del retroceso. Si la rotación en Y es 180, el jugador mira a la izquierda.
        float direction = (transform.rotation.eulerAngles.y == 180) ? 1f : -1f;

        // Reiniciar la velocidad antes de aplicar el retroceso
        rb.velocity = new Vector2(0, rb.velocity.y); // Mantener la velocidad Y para permitir el movimiento normal de gravedad

        // Aplicar la fuerza en la dirección opuesta a donde está mirando el jugador
        rb.AddForce(new Vector2(force * direction, 0), ForceMode2D.Impulse);

        // Debug para verificar la dirección
        //Debug.Log($"Applying recoil: direction = {direction}, force = {force * direction}");

        yield return new WaitForSeconds(0.2f);

        canMove = true;
        canApplyForce = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = UnityEngine.Color.red;
        Gizmos.DrawWireCube(overlapPosition.position, boxSize);
    }

    //Efecto para crear el efecto de particulas al moverse
    private void createDust()
    {
        dust.Play();
    }

    private void stopDust()
    {
        dust.Stop();
    }

    private IEnumerator ChangeScene()
    {
        //Se puede acceder al RigidBody del player y poner su velocidad en 0. También acceder al animator de Player y ponerlo en idle, pero creo que queda mejor así
        canMove = false;
        anim.SetTrigger(GameTags.changingScene);
        PlayerPrefs.SetString(GameTags.lastScene, SceneManager.GetActiveScene().name);
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene("B1_Scene_07");
    }
}
