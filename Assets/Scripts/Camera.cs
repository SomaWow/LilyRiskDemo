using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public float limitDistance = 2;
    public float speed = 3;

    private Transform player;
    private Vector3 vec3;
    

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("player").transform;
        vec3 = new Vector3(0, 0, transform.position.z);
    }

    private void LateUpdate()
    {
        Vector3 playerPos = player.position;

        if(Mathf.Abs(Vector2.Distance(playerPos, transform.position)) > 0.01f)
        {
            vec3.y = playerPos.y;
            float delta = playerPos.x - transform.position.x;
            float deltaAbs = Mathf.Abs(delta);
            int direction = delta > 0 ? 1 : -1;
            if (Mathf.Abs(delta) > limitDistance)
            {
                vec3.x = playerPos.x - limitDistance * direction;
            }
            else
            {
                if(deltaAbs < speed * Time.deltaTime * direction)
                    vec3.x = transform.position.x + deltaAbs;
                else
                    vec3.x = transform.position.x + speed * Time.deltaTime * direction;
            }
            transform.position = vec3;
        }
    }
}
