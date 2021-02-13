using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : MonoBehaviour
{
    public static Dictionary<int, BasicProjectile> projectileDic = new Dictionary<int, BasicProjectile>();
    public static int nextProjectileId = 1;

    private Rigidbody rb;
    public int byPlayerId;
    public int id;
    public int damage;

    private Vector3 initialForce;

    public void Init(Vector3 dir, int force, int playerId)
    {
        byPlayerId = playerId;
        initialForce = dir * force;
    }

    // Start is called before the first frame update
    void Start()
    {
  
        rb = GetComponent<Rigidbody>();
     
        rb.AddForce(initialForce);

        id = nextProjectileId;
        nextProjectileId++;

        ServerSend.InstantiateBasicProjectile(this, byPlayerId);
        projectileDic.Add(id, this);

        Destroy(gameObject, 3f);
        
    }

    private void FixedUpdate()
    {
        ServerSend.ProjectilePosition(this);
    }

    private void OnDestroy()
    {
        projectileDic.Remove(id);
        ServerSend.DestroyBasicProjectile(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            Player player = other.GetComponent<Player>();
            player.TakeDamage(damage);
            Vector3 direction = (player.transform.position - transform.position).normalized;
            player.HitByProjectile(new Vector3(direction.x, 0f, direction.z));
            Debug.Log("Transform forward: " + direction);
            Destroy(gameObject);
        }
    }

   
}
