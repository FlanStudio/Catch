using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class PlayerController : NetworkBehaviour
{
    private Animator animator;
    private Camera mainCamera;
    public TextMesh nameLabel;

    const float RUNNING_SPEED = 10.0f;
    const float ROTATION_SPEED = 10f;

    public uint playerPrefabID = 0u;
    public GameObject player1Prefab;
    public GameObject player2Prefab;

    //// Name sync /////////////////////////////////////

    [SyncVar(hook = "SyncNameChanged")]  
    public string playerName = "Player";

    [Command]
    void CmdChangeName(string name) { playerName = name; }
     
    void SyncNameChanged(string name) { nameLabel.text = name; }

    //// OnGUI /////////////////////////////////////////

    private void OnGUI()
    {
        if (isLocalPlayer)
        {
            //GUILayout.BeginArea(new Rect(Screen.width - 260, 10, 250, Screen.height - 20));

            //string prevPlayerName = playerName;
            //playerName = GUILayout.TextField(playerName);
            //if (playerName != prevPlayerName)
            //{
            //    if (nameLabel != null)
            //    {
            //        CmdChangeName(playerName);
            //    }
            //}

            //GUILayout.EndArea();
        }      
    }


    // Animation sync ////////////////////////////////

    string animationName;

    void setAnimation(string animName)
    {
        if (animationName == animName) return;
        animationName = animName;

        animator.SetBool("Idling", false);
        animator.SetBool("Running", false);
        animator.SetBool("Running backwards", false);
        animator.ResetTrigger("Jumping");
        animator.ResetTrigger("Kicking");

        if (animationName == "Idling") animator.SetBool("Idling", true);
        else if (animationName == "Running") animator.SetBool("Running", true);
        else if (animationName == "Running backwards") animator.SetBool("Running backwards", true);
        else if (animationName == "Jumping") animator.SetTrigger("Jumping");
        else if (animationName == "Kicking") animator.SetTrigger("Kicking");
    }


    // Lifecycle methods ////////////////////////////

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;

        if (isLocalPlayer)
        {
            mainCamera.GetComponent<CameraController>().SetTarget(transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isLocalPlayer)
        {
            #region MOVEMENT

            float horizontalAxis = Input.GetAxis("Horizontal");
            float verticalAxis = Input.GetAxis("Vertical");

            Vector3 desiredRotation = Vector3.zero;

            Vector2 move = new Vector2(horizontalAxis, verticalAxis);
            if(move != Vector2.zero)
            {
                setAnimation("Running");
                desiredRotation = (mainCamera.transform.forward * verticalAxis + mainCamera.transform.right * horizontalAxis) / 2;
                desiredRotation.y = 0;

                if (desiredRotation != Vector3.zero)
                {
                    Vector3 playerRotation = transform.forward;

                    float angle = Vector3.SignedAngle(playerRotation, desiredRotation, Vector3.up);
                    transform.Rotate(Vector3.up, angle * ROTATION_SPEED * Time.deltaTime);
                }

                transform.Translate(Vector3.forward * Time.deltaTime * RUNNING_SPEED);
            }
            else
            {
                setAnimation("Idling");               
            }
              
            //if (Input.GetButtonDown("Jump"))
            //{
            //    setAnimation("Jumping");
            //}

            //if (Input.GetButtonDown("Fire1"))
            //{
            //    setAnimation("Kicking");
            //}

            if(Input.GetKeyDown(KeyCode.F4))
            {
                Respawn();
            }

            #endregion
        }
    }

    public void Respawn()
    {
        var newPlayer = Instantiate(playerPrefabID == 0 ? player1Prefab : player2Prefab);

        NetworkServer.ReplacePlayerForConnection(connectionToClient, newPlayer, 0);

        Destroy(gameObject);
    }
}
