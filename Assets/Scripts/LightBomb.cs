using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class LightBomb : MonoBehaviour
{
    public Sprite[] numSprite;
    public float time = 3;
    public float castDistance = 0;
    public float explodeDistance = 5;

    private int lastSprite;
    private SpriteRenderer spriteRenderer;
    public GameObject fxPrefab;

    private void Start()
    {
        lastSprite = (int)math.floor(time);
        spriteRenderer = transform.Find("countdown").GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = numSprite[lastSprite];
    }

    private void Update()
    {
        time = time - Time.deltaTime;
        if (time < 0)
        {
            Explode();
            Destroy(gameObject);
        }
        int curSprite = (int)math.floor(time);
        if(curSprite != lastSprite)
        {
            spriteRenderer.sprite = numSprite[lastSprite];
            lastSprite = curSprite;
        }
    }

    private void Explode()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Enemy");
        if(gos != null)
        {
            foreach (GameObject go in gos)
            {
                if (Vector2.Distance(transform.position, go.transform.position) < explodeDistance)
                {
                    go.GetComponent<Enemy>().Die();
                }
            }

        }


        GameObject fx = GameObject.Instantiate(fxPrefab);
        fx.transform.position = transform.position;
        Destroy(gameObject);
    }
    
}
