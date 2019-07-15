using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class AstronautController : MonoBehaviour
{
    //TO DELETE

    //SAVED FOR POSSIBLE LATER USE
    //private int count;
    //public Text countText;
    //public Text winText;

    //JUMP Variables
    public int jumps;
    public static int playerJumps;
    public float jumpForce;
    public bool grounded;
    public float fallMultiplier = 9f;
    public float lowJumpMultiplier = 4f;

    private AudioSource jumpAudio;
    private bool playJumpAudio;

    //public Collider playerCollider;

    private Rigidbody rb;

    private Vector3 player_position;
    private Vector3 camera_position;

    private Vector3 halfSpin; //to store transform.rotation.y to rotate player

    private Animator anim;

    private bool orientation;

    private float circleAngleRadian;
    private float circleAngleDegree;
    private float circleX;
    private float circleZ;

    private bool wallOnLeft;
    private bool wallOnRight;
    private bool goingLeft;
    private bool goingRight;

    private float magnitudeUpper = 1.0f;
    private float magnitudeLower = 0.4f;
    private bool jumped;

    private bool doubleBackLeft;
    private bool wasFlyingRight;
    private bool inAirInactive;

    private bool jumpedStraight;
    private bool jumpedRight;
    private bool jumpedLeft;

    private bool justWallJumped;
    private float wallJumpDelay;
    private bool wallJumpContDelay;

    private Vector3 updatePosition;

    public float wallJumpTime;
    private float wallJumpCircleAngle;
    private float wallJumpCircleRadian;
    private float wallJumpCircleX;
    private float wallJumpCircleZ;
    private Vector3 wallJumpEndPos;
    private Vector3 wallJumpStartPos;

    private bool jumpedOffWall;

    //private float distToGround;
    private PlayerHealth playerHealth;
    private LevelSelect levelSelect;

    //private bool direction; // for movement control
    public GameObject jetpackTrailPrefab;
    private ParticleSystem jetpackTrail;
    private string scenename1;
    private string scenename4;
    private string finishedscene;
    private float startTime;

    void Start()
    {
        startTime = Time.time;

        rb = GetComponent<Rigidbody>();
        jumps = 1;
        playerJumps = 0;
        camera_position = new Vector3(0, 0, 0);
        scenename1 = "LevelOne - Haig";
        scenename4 = "5.17.19-9.13.pm-AddingTimer";
        finishedscene = "FinishScreen";
        anim = gameObject.GetComponentInChildren<Animator>();

        //John Trying This
        playerHealth = GetComponent<PlayerHealth>();
        levelSelect = GetComponent<LevelSelect>();
        //playerJumps = GetComponent<JumpManager>();
        //done

        halfSpin = new Vector3(0, 180, 0); //value to turn by
        orientation = false; //for evaluating turning left and right

        circleAngleDegree = 0;
        circleAngleRadian = 0;
        circleX = 0;
        circleZ = 0;

        player_position = new Vector3(44f, 18.25f, 0f);
        transform.position = player_position;
        updatePosition = player_position;
        //print("player position = " + player_position.ToString() + "\n");
        //print("transform position = " + transform.position.ToString() + "\n");
        //print("Update position = " + updatePosition.ToString() + "\n");

        wallOnLeft = false;
        wallOnRight = false;
        goingLeft = false;
        goingRight = false;

        //WallJump time
        wallJumpEndPos = new Vector3(0, 0, 0);
        wallJumpStartPos = new Vector3(0, 0, 0);

        jumped = false;
        doubleBackLeft = false;
        wasFlyingRight = false;
        inAirInactive = false;

        jumpedStraight = false;
        jumpedRight = false;
        jumpedLeft = false;

        justWallJumped = false;
        wallJumpDelay = 0f;
        wallJumpContDelay = false;

        jumpAudio = GetComponent<AudioSource>();
        //Ensure the toggle is set to true for the music to play at start-up
        playJumpAudio = false;


        //jetpackTrail = Instantiate(jetpackTrailPrefab).GetComponent<ParticleSystem>();
        //jetpackTrail.gameObject.SetActive(false);

        //Grounded Float
        //distToGround = playerCollider.bounds.extents.y;


        //orientation = new Vector3 (0f, 180f, 0f);

        //playerCollider = gameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();
        //groundCollider = gameObject.FindGameObjectWithTag("Ground").GetComponent<Collider2D>();
        //SetCountText();
        //winText.text = "";
    }

    void Update()
    {
        //print("UPDATE CALLED: \n");
        //print("player position = " + player_position.ToString() + "\n");
        //print("transform position = " + transform.position.ToString() + "\n");
        //print("Update position = " + updatePosition.ToString() + "\n");

        player_position = transform.position;
        updatePosition = player_position;
        camera_position.y = transform.position.y;
        anim.SetInteger("AnimationPar", 0);

        //Debug.Log("prev = " + LevelSelect.prevscenename);


        //trying reset functionality, need to find out scene name since there is only one level,
        //which is a problem
        if (Input.GetKeyDown(KeyCode.R))
        {
            //or whatever number your scene is
            //LoadA(scenename1);
            //Debug.Log("sceneName = " + levelSelect.prevscenename);
           
            levelSelect.RetryCorrectLevel();
        }

        if (!Input.anyKey && grounded && !(wallOnLeft || wallOnRight))
        {
            goingLeft = false;
            goingRight = false;
        }

        //HANDLE MOVEMENT
        if (Input.GetKey(KeyCode.RightArrow) == true && grounded)
        {
            RunRight();
        }
        else if (Input.GetKey(KeyCode.LeftArrow) == true && grounded)
        {
            RunLeft();
        }

        //----JUMP----
        if (Input.GetKeyDown(KeyCode.Space) && (grounded == true) && (jumps == 1) && (Input.GetKey(KeyCode.RightArrow) == false) && (Input.GetKey(KeyCode.LeftArrow) == false))
        {
            Jump();
            jumped = true;
            //inAirInactive = true;
            //print("jumped at stop\n");
            jumpedStraight = true;
            goingLeft = false;
            goingRight = false;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && grounded == true && jumps == 1 && Input.GetKey(KeyCode.RightArrow))
        {
            Jump();
            jumped = true;
            //inAirInactive = true;
            //print("jumped going right\n");
            jumpedRight = true;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && grounded == true && jumps == 1 && Input.GetKey(KeyCode.LeftArrow))
        {
            Jump();
            jumped = true;
            //inAirInactive = true;
            //print("jumped going left\n");
            jumpedLeft = true;
        }

        //----WALLJUMP----
        if ((!grounded) && wallOnRight == true && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine("WallJumpWait");
            transform.Rotate(halfSpin, Space.Self);
            orientation = !orientation;
            Jump();
            jumpedLeft = true;
            jumpedRight = false;
            //print("in walljump w wall on right, calling flying going to the left\n");
            Flying(-1, 1);
        }
        if ((!grounded) && wallOnLeft == true && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine("WallJumpWait");
            transform.Rotate(halfSpin, Space.Self);
            orientation = !orientation;
            Jump();
            jumpedRight = true;
            jumpedLeft = false;
            //print("in walljump w wall on left, calling flying going to the right\n");
            Flying(1, 1);
        }

        //----FLYRIGHT----
        if (!grounded && goingRight && !wallOnRight && (Input.GetKey(KeyCode.RightArrow) == true) && jumpedRight && (wallJumpContDelay == false))
        {
            //print("constant jump called\n");
            Flying(1, 1);
        }
        else if (!grounded && goingRight && !wallOnRight && jumpedRight)
        {
            //print("MagnitudeDie Called\n");
            wasFlyingRight = true;
            if (magnitudeUpper > 0.0f)
            {
                MagnitudeDie();
            }
            else
            {
                magnitudeUpper = 0.0f;
            }
            Flying(1, magnitudeUpper);
        }

        //----FLYLEFT----
        if (!grounded && goingLeft && !wallOnLeft && (Input.GetKey(KeyCode.LeftArrow) == true) && jumpedLeft && (wallJumpContDelay == false))
        {
            //print("in flyleft if\n");
            Flying(-1, 1);
        }
        else if (!grounded && goingLeft && !wallOnLeft && jumpedLeft)
        {
            //print("in flyleft else if\n");
            //print("MagnitudeDie Called\n");
            if (magnitudeUpper > 0.0f)
            {
                MagnitudeDie();
            }
            else
            {
                magnitudeUpper = 0.0f;
            }
            Flying(-1, magnitudeUpper);
        }

        //----DOUBLE BACK LEFT----
        if ((Input.GetKey(KeyCode.LeftArrow) == true) && !wallOnLeft && !grounded && (jumpedRight || jumpedStraight) && justWallJumped == false)
        {
            //print("in double back left\n");
            if (!orientation)
            {
                transform.Rotate(halfSpin, Space.Self);
                orientation = true;
            }
            doubleBackLeft = true;
            //print("In left double back code\n");
            if (magnitudeLower < 0.7f)
            {
                MagnitudeGain();
            }
            else
            {
                //print("In double back else\n");
                magnitudeLower = 0.7f;
            }
            //print("doubling back left, magnitude = " + magnitudeLower.ToString() + "\n");
            Flying(-1, magnitudeLower);
        }

        //----DOUBLE BACK RIGHT----
        if ((Input.GetKey(KeyCode.RightArrow) == true) && !wallOnRight && !grounded && (jumpedLeft || jumpedStraight) && justWallJumped == false)
        {
            //print("in double back right\n");
            if (orientation)
            {
                transform.Rotate(halfSpin, Space.Self);
                orientation = false;
            }
            //doubleBackLeft = true;
            //print("In left double back code\n");
            if (magnitudeLower < 0.7f)
            {
                MagnitudeGain();
            }
            else
            {
                //print("In double back else\n");
                magnitudeLower = 0.7f;
            }
            //print("doubling back right, magnitude = " + magnitudeLower.ToString() + "\n");
            Flying(1, magnitudeLower);
        }

        //----FALL MULTIPLIER----
        HandleGravity();

        //int t = Time.time - startTime;
        //if(Time.time - t == 1)
        {
            playerHealth.TakeDamage(0.025f);
        }
        //playerHealth.TakeDamage(1);



        //escape to main menu, will hopefully be a pause menu
        //of sorts later
        if (Input.GetKey(KeyCode.Escape) == true)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    private void FixedUpdate()
    {
        //print("FIXED UPDATE CALLED: \n");
        //print("player position = " + player_position.ToString() + "\n");
        //print("transform position = " + transform.position.ToString() + "\n");
        //print("Update position = " + updatePosition.ToString() + "\n");

        rb.MovePosition(updatePosition);

        //print("FIXED UPDATE CALLED, AFTER RBMOVE: \n");
        //print("player position = " + player_position.ToString() + "\n");
        //print("transform position = " + transform.position.ToString() + "\n");
        //print("Update position = " + updatePosition.ToString() + "\n");
    }


    void RunRight()
    {
        if (wallOnRight == false)
        {
            goingRight = true;
            goingLeft = false;
            //wallOnLeft = false;

            anim.SetInteger("AnimationPar", 1);
            if (orientation)
            {
                //print("in ! direction statement");
                transform.Rotate(halfSpin, Space.Self);
                orientation = false;
            }
            //rotatearound implementation
            //transform.RotateAround(camera_position, y_axis, runSpeed);

            //circle implementation
            circleAngleDegree = circleAngleDegree - 0.5f;
            circleAngleRadian = circleAngleDegree * Mathf.Deg2Rad;
            circleX = 0 + (44 * Mathf.Cos(circleAngleRadian));
            circleZ = 0 + (44 * Mathf.Sin(circleAngleRadian));

            //print("CircleAngleDegree = " + circleAngleDegree.ToString() + "\n");
            //print("Degree x = " + circleX.ToString() + "\n");
            //print("Degree z = " + circleZ.ToString() + "\n");


            updatePosition.x = circleX;
            updatePosition.z = circleZ;
            updatePosition.y = transform.position.y;

            //transform.position = updatePosition;
            //rb.MovePosition(updatePosition); //HEEEEERRRRRRREEEEEEEEEEEEEEEEE
            //rb.velocity += (updatePosition - (rb.position));

            transform.Rotate(0f, 0.5f, 0f, Space.Self);
        }
        else
        {
            //print("onWall true, approached from left\n");
        }
    }

    void RunLeft()
    {
        if (wallOnLeft == false)
        {
            goingLeft = true;
            goingRight = false;
            //wallOnRight = false;


            //print("onWall false\n");
            anim.SetInteger("AnimationPar", 1);
            if (!orientation)
            {
                //print("in ! direction statement");
                transform.Rotate(halfSpin, Space.Self);
                orientation = true;
            }
            //rotateAround implementation
            //transform.RotateAround(camera_position, y_axis, -runSpeed);

            //circle implementation
            circleAngleDegree = circleAngleDegree + 0.5f;
            circleAngleRadian = circleAngleDegree * Mathf.Deg2Rad;
            circleX = 0 + (44 * Mathf.Cos(circleAngleRadian));
            circleZ = 0 + (44 * Mathf.Sin(circleAngleRadian));

            //print("CircleAngleDegree = " + circleAngleDegree.ToString() + "\n");
            //print("Degree x = " + circleX.ToString()+ "\n");
            //print("Degree z = " + circleZ.ToString() + "\n");


            updatePosition.x = circleX;
            updatePosition.z = circleZ;
            updatePosition.y = transform.position.y;

            //transform implementation
            //transform.position = updatePosition;
            //rb.move implementation
            //rb.MovePosition(updatePosition);//HEEEEERRRRRRREEEEEEEEEEEEEEEEE

            transform.Rotate(0f, -0.5f, 0f, Space.Self);
        }
    }


    void Flying(int directionNum, float strength)
    {
        //print("Flying called with strength: " + strength.ToString() + "\n");
        if (directionNum > 0)
        {
            goingRight = true;
            goingLeft = false;
        }
        if (directionNum < 0)
        {
            goingLeft = true;
            goingRight = false;
        }

        circleAngleDegree = circleAngleDegree - 0.5f * directionNum * strength;
        circleAngleRadian = circleAngleDegree * Mathf.Deg2Rad;
        circleX = 0 + (44 * Mathf.Cos(circleAngleRadian));
        circleZ = 0 + (44 * Mathf.Sin(circleAngleRadian));

        //print("CircleAngleDegree = " + circleAngleDegree.ToString() + "\n");
        //print("Degree x = " + circleX.ToString() + "\n");
        //print("Degree z = " + circleZ.ToString() + "\n");


        updatePosition.x = circleX;
        updatePosition.z = circleZ;
        updatePosition.y = transform.position.y;

        //transform.position = updatePosition;
        //rb.MovePosition(updatePosition);//HEEEEERRRRRRREEEEEEEEEEEEEEEEE

        transform.Rotate(0f, 0.5f * directionNum * strength, 0f, Space.Self);
    }

    void MagnitudeDie()
    {
        if (magnitudeUpper > 0.0f)
        {
            magnitudeUpper = Mathf.Round((magnitudeUpper - 0.01f) * 100f) / 100.0f;
            //print("in magnitudedie while, magnitude = " + magnitudeUpper.ToString() + "\n");
        }
        else
        {
            //print("magnitude reached 0f\n");
        }
    }
    void MagnitudeGain()
    {
        if (magnitudeLower < 0.7f)
        {
            magnitudeLower = Mathf.Round((magnitudeLower + 0.01f) * 100f) / 100.0f;
            //print("in magnitudegain while, magnitudeLower = " + magnitudeLower.ToString() + "\n");
        }
        else
        {
            //print("magnitudelower reached 0.5f\n");
        }
    }

    void Jump()
    {
        playJumpAudio = true;
        //print("in jump\n");
        if (playJumpAudio == true)
        {
            jumpAudio.Play();
            playJumpAudio = false;
        }

        rb.velocity = Vector3.up * jumpForce;
        grounded = false;

        anim.SetTrigger("Jump");
        anim.SetBool("Grounded", false);

        jumps = jumps - 1;
        playerJumps = playerJumps + 1;

        //jetpackTrail.gameObject.SetActive(true);
        //jetpackTrail.Play();
    }

    IEnumerator WallJumpWait()
    {
        //print("WallJumpDelay set\n");
        //wallJumpDelay = 1f;
        justWallJumped = true;

        yield return new WaitForSeconds(1f);

        justWallJumped = false;
    }
    IEnumerator WallJumpContWait()
    {
        //print("WallJumpDelay set\n");
        //wallJumpDelay = 1f;
        wallJumpContDelay = true;

        yield return new WaitForSeconds(0.5f);

        wallJumpContDelay = false;
    }


    void HandleGravity()
    {
        if (rb.velocity.y < 0 && !grounded)
        {
            //print("in velocity < 0, not grounded and velocity less than zero\n");
            rb.AddForce(Physics.gravity * (fallMultiplier - 1));
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space) && !grounded)
        {
            //print("in VELOCITY > 0\n");
            rb.AddForce(Physics.gravity * (lowJumpMultiplier - 1));
        }
        //POTENTIALLY USE A RAYCAST SO GROUNDED IS MORE ACCURATE
        /*else if (rb.velocity.y < 0 && grounded)
        {
            print("velocity < 0 and grounded still true\n");
            rb.AddForce(Physics.gravity * (fallMultiplier - 1));

        }*/
    }

    //CODE FOR PICKUP OBJECTS (POWERUPS)
    /*void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pick Up"))
        {
            other.gameObject.SetActive(false);
            count = count + 1;
            //SetCountText();
        }
        if (other.gameObject.CompareTag("Ground"))
        {
            print("ontriggerenter ground collider statement activated\n");
            //other.gameObject.SetActive(false);
            //count = count + 1;
            //SetCountText();
            jumps = 1;
        }
    }*/


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            grounded = true;
            jumps = 1;
            anim.SetBool("Grounded", true);
            magnitudeUpper = 1f;
            magnitudeLower = 0.1f;
            doubleBackLeft = false;

            jumpedStraight = false;
            jumpedRight = false;
            jumpedLeft = false;
            //added taking damage from each ground touch(jump) here
            //playerHealth.TakeDamage(1);
        }

        if (collision.gameObject.tag == "Wall")
        {
            //print("wall hit\n");
            magnitudeUpper = 1f;
            magnitudeLower = 0.4f;
            if (goingRight == true)
            {
                wallOnRight = true;
                //print("Wall on right = true\n");
            }
            if (goingLeft == true)
            {
                wallOnLeft = true;
                //print("Wall on left = true\n");
            }
        }

        if (collision.gameObject.tag == "Finish")
        {
            //print("Level Finished, new screen /n");

            //string thistime;
            //thistime = TimerController.yourtime;

            SceneManager.LoadScene(finishedscene);


        }

        if (collision.gameObject.CompareTag("Capsule"))
        {
            playerHealth.gainOxygen();
            collision.gameObject.SetActive(false);

        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            grounded = false;
            jumps = 0;
            if (Input.GetKey(KeyCode.RightArrow))
            {
                jumpedRight = true;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                jumpedLeft = true;
            }
            //jumpedStraight = true;
            //anim.SetBool("Grounded", true);
        }

        if (collision.gameObject.tag == "Wall")
        {
           //print("not touching walls\n");
            wallOnLeft = false;
            wallOnRight = false;
        }

        //if (collision.gameObject.tag == "Finish")
        //{
        //    print("Level Finished, new screen /n");
        //    SceneManager.LoadScene(finishedscene);


        //}
            
        if (collision.gameObject.CompareTag("Capsule"))
        {
            collision.gameObject.SetActive(false);

        }
    }


        //public static void LoadScene(string sceneName, UnityEngine.SceneManagement.LoadSceneMode mode = LoadSceneMode.Single)
        //{

        //}
        //public void LoadA(string scenename)
        //{
        //    Debug.Log("sceneName to load: " + scenename);
        //    SceneManager.LoadScene(scenename);
        //}

        //CODE FOR UI
        /*void SetCountText()
        {
            countText.text = "Count: " + count.ToString();
            if (count >= 10)
            {
                winText.text = "You Win!!";
            }
        }*/



    }
