using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bouncing : MonoBehaviour
{
    public float force;

    public Color[] colors;

    private int index;

    private void Start()
    {
        GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, -1) * force, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "Wall")
        {
            index ++;
            if (index > colors.Length)
                index = 0;
            GetComponent<SpriteRenderer>().color = colors[index];
        }
    }
}
