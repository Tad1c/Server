using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : MonoBehaviour
{
    public static Dictionary<int, BasicProjectile> projectileDic = new Dictionary<int, BasicProjectile>();
    public static int nextProjectileId = 1;

    public float speed = 10.0f;
    public float range = 30.0f;

    private Rigidbody rb;
    public int byPlayerId;
    public int id;
    public int damage;

    private Vector3 finalDestination;

    public float posUpdateRate = 0.2f;

    private Vector3 hitDir;

    public void Init(Vector3 dir, int playerId)
    {
        byPlayerId = playerId;
        finalDestination = transform.TransformPoint(dir * range);
        hitDir = dir;
    }

    // Start is called before the first frame update
    void Start()
    {

        rb = GetComponent<Rigidbody>();

        id = nextProjectileId;
        nextProjectileId++;

        projectileDic.Add(id, this);

        ServerSend.InstantiateBasicProjectile(this, byPlayerId, finalDestination);
        transform.LookAt(finalDestination);
        // ServerSend.ProjectilePosition(id, finalDestination);
        //InvokeRepeating("UpdatePosition", posUpdateRate, posUpdateRate);  //1s delay, repeat every 1s

    }

    private void Update()
    {
        float step = speed * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, finalDestination, step);

        if (Vector3.Distance(transform.position, finalDestination) < 0.01f)
        {
            Destroy(gameObject);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            player.TakeDamage(damage);
            Vector3 direction = transform.position - player.transform.position; //player.transform.position - transform.position;
            direction = -direction.normalized;
            player.HitByProjectile(direction);
            Debug.Log("Transform forward: " + direction);
            Destroy(gameObject);
        }
    }


    private void OnDestroy()
    {
        CancelInvoke();
        projectileDic.Remove(id);
        ServerSend.DestroyBasicProjectile(this);
    }


}
