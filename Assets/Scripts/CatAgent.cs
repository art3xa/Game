using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CatAgent : Monster
{
    [SerializeField]
    private float rate = 2.0F;
    [SerializeField]
    private float speed = 2.0F;

    private AgentBullet agentBullet;
    private Vector3 direction;

    private bool isFacingLeft = true;
    public Transform groundCheck;
    public LayerMask groundLayers;
    public Rigidbody2D rb;

    private SpriteRenderer sprite;

    protected void Awake()
    {
        agentBullet = Resources.Load<AgentBullet>("AgentBullet");
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    protected void Start()
    {
        InvokeRepeating("Shoot", rate, rate);
        direction = -transform.right;
    }
    protected void Update()
    {
        Move();
    }

    private void Shoot()
    {
        Vector3 position = transform.position; position.y += 0.5F;
        AgentBullet newAgentBullet = Instantiate(agentBullet, position, agentBullet.transform.rotation);
        newAgentBullet.Parent = gameObject;
        newAgentBullet.Sprite.flipX = !isFacingLeft;
        newAgentBullet.Direction = -newAgentBullet.transform.right * speed / 2;
    }

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        Unit unit = collider.GetComponent<Unit>();

        if (unit && unit is Character)
        {
            if (Mathf.Abs(unit.transform.position.x - transform.position.x) < 0.3F) ReceiveDamage();
            else unit.ReceiveDamage();
        }

        Spear spear = collider.gameObject.GetComponent<Spear>();
        if (spear && spear.Parent != gameObject)
        {
            ReceiveDamage();
        }
    }

    private void Move()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + transform.up * 0.5F + transform.right * direction.x * 0.5F, 0.1F);

        RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, 1f, groundLayers);

        if (colliders.Length > 0 && colliders.All(x => !x.GetComponent<Character>() && !x.GetComponent<AgentBullet>()) || groundInfo.collider == false)
        {
            isFacingLeft = !isFacingLeft;
            speed = -speed;
            direction *= -1.0F;
            transform.localScale = new Vector2(-transform.localScale.x, 1f);
        }
        rb.velocity = new Vector2(-speed, rb.velocity.y);

    }
}