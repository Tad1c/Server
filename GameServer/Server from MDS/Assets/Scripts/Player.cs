using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string username;

    public Rigidbody controller;
    public Transform shootOrigin;
    public float gravity = -9.81f;
    public float moveSpeed = 5f;
    public float jumpSpeed = 5f;
    public float health;
    public float maxHealht = 100;

    private bool[] inputs;
    private float yVelocity = 0;

    private bool isGrounded;

    [SerializeField]
    private float distanceCheck;

    private bool stunned = false;

    private bool isHit;
    private Vector3 hitDirection;

    private void Start()
    {
        //  gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        //   moveSpeed *= Time.fixedDeltaTime;
        //  jumpSpeed *= Time.fixedDeltaTime;
    }

    public void Initialize(int id, string username)
    {
        this.id = id;
        this.username = username;
        health = maxHealht;
        inputs = new bool[5];
    }

    private void Update()
    {
        RaycastHit hit;

        Debug.DrawRay(this.transform.position, Vector3.down, Color.red, distanceCheck);
        if (Physics.Raycast(this.transform.position, Vector3.down, out hit, distanceCheck))
        {
            if (hit.collider.CompareTag("Untagged"))
                isGrounded = true;
        }
        else
            isGrounded = false;
    }

    public void FixedUpdate()
    {
        if (health <= 0)
            return;

        Vector3 inputDirection = Vector3.zero;

        // W
        if (inputs[0])
            inputDirection.y += 1;

        // S
        if (inputs[1])
            inputDirection.y -= 1;

        // A
        if (inputs[2])
            inputDirection.x -= 1;

        // D
        if (inputs[3])
            inputDirection.x += 1;

        // if (!stunned) {

        if (stunned)
        {
            // inputDirection = hitDirection;
            // isHit = false;
            ServerSend.PlayerPosition(this);
            ServerSend.PlayerRotation(this);
        }
        else
        {
            Move(inputDirection);
        }



        // }
        // else {
        //     
        // }

    }

    private void Move(Vector2 inputDirection)
    {

        Vector3 moveDirection = new Vector3(inputDirection.x, 0f, inputDirection.y);//transform.right * inputDirection.x + transform.forward * inputDirection.y;

        moveDirection *= moveSpeed;

        if (isGrounded)
        {
            if (inputs[4])
                controller.AddForce(new Vector3(0f, jumpSpeed, 0f));
        }

        controller.velocity = new Vector3(moveDirection.x, controller.velocity.y, moveDirection.z);

        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
    }

    public void SetInputs(bool[] inputs, Quaternion rotation)
    {
        this.inputs = inputs;
        transform.rotation = rotation;
    }

    public void Shoot(Vector3 viewDirection)
    {
        if (Physics.Raycast(shootOrigin.position, viewDirection, out RaycastHit hit, 25f))
        {
            if (hit.collider.CompareTag("Player"))
            {
                hit.collider.GetComponent<Player>().TakeDamage(5);
                //hit.collider.GetComponent<Player>().MoveBackwardsOnHit(-hit.transform.forward, 200);
            }
        }
    }

    public void ShootProjectile(Vector3 shootDirection)
    {
        NetworkManager.instance.InstanciateProjectile(shootOrigin).Init(shootDirection, id);
    }

    public void HitByProjectile(Vector3 direction)
    {
        // isHit = true;
        // hitDirection = direction;
        StartCoroutine(stunCountdown(direction));
    }

    private IEnumerator stunCountdown(Vector3 dir)
    {
        stunned = true;
        controller.AddForce(dir * 200f);
        yield return new WaitForSeconds(0.5f);
        stunned = false;
    }

    public void TakeDamage(float dmg)
    {
        if (health <= 0)
            return;

        health -= dmg;
        if (health <= 0f)
        {
            health = 0f;
            controller.useGravity = false;
            transform.position = new Vector3(0f, 25f, 0f);
            ServerSend.PlayerPosition(this);
            StartCoroutine(Respawn());
        }

        ServerSend.PlayerHealth(this);
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f);

        health = maxHealht;
        controller.useGravity = true;
        ServerSend.PlayerRespawn(this);
    }

}

