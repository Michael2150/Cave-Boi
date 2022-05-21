using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] Vector2 enemyVelocity;

    new Rigidbody2D rigidbody;
    new Collider2D collider;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        flipSprite();
        rigidbody.velocity = enemyVelocity;
    }

    private void flipSprite()
    {
        if (Mathf.Abs(rigidbody.velocity.x) > 0) { 
            float direction = Mathf.Sign(rigidbody.velocity.x);  // calculate direction of movement
            transform.localScale = new Vector2(direction, 1f);  // apply movement direction to sprite
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Vector2 bl = new Vector2(collider.bounds.min.x, collider.bounds.min.y);
        Vector2 tl = new Vector2(collider.bounds.min.x, collider.bounds.max.y);
        Vector2 br = new Vector2(collider.bounds.max.x, collider.bounds.min.y);
        Vector2 tr = new Vector2(collider.bounds.max.x, collider.bounds.max.y);

        bool blCol = col.OverlapPoint(bl);
        bool tlCol = col.OverlapPoint(tl);
        bool brCol = col.OverlapPoint(br);
        bool trCol = col.OverlapPoint(tr);

        Vector2 invVel = (blCol) ? new Vector2(-1, -1) 
                       : (tlCol) ? new Vector2(-1, 1) 
                       : (brCol) ? new Vector2(1, -1) 
                       : (trCol) ? new Vector2(1, 1) 
                       : Vector2.one * -1;

        enemyVelocity *= invVel;

    }
}
