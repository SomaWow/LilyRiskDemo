using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Point[] patrolPoints;
    public float relaxTime = 2;
    public float alertDiatance = 5;
    public float speed = 3;

    private int patrolIndex = 0; //下一个巡逻点
    private float curRelaxTime = 0; //目前的休息时间
    private string ground; //当前地面的名字
    private State state;
    private SubState subSate;
    private Point[] chasePoints;
    private float height = 0.5f;
    private Transform player;
    private GameObject exclamationPoint;
    private int direction = 1;
    private Vector3 originalPos;
    public GameObject fxPrefab;

    // Start is called before the first frame update
    void Start()
    {
        state = State.Patrol;
        exclamationPoint = transform.Find("exclamationPoint").gameObject;
        originalPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Relax:
                if(CheckPlayer()){
                    state = State.Chase;
                    subSate = SubState.Start;
                    return;
                };
                curRelaxTime += Time.deltaTime;
                if(curRelaxTime >= relaxTime)
                {
                    curRelaxTime = 0;
                    state = State.Patrol;
                }
                break;
            case State.Patrol:
                if (CheckPlayer()) {
                    state = State.Chase;
                    subSate = SubState.Start;
                    return;
                };
                VerticalMove(patrolPoints[patrolIndex].transform.position);
                if (Vector2.Distance(patrolPoints[patrolIndex].transform.position + new Vector3(0, height, 0), transform.position) < 0.1f)
                {
                    state = State.Relax;
                    patrolIndex++;
                    if (patrolIndex >= patrolPoints.Length) patrolIndex = 0;
                }
                break;
            case State.Chase:
                if (!exclamationPoint.activeSelf) exclamationPoint.SetActive(true);
                switch (subSate)
                {
                    case SubState.Start:
                        GetChasePoints();
                        if(chasePoints != null && chasePoints.Length > 0)
                        {
                            subSate = SubState.VerticalMove;
                        }
                        break;
                    case SubState.VerticalMove:
                        if (VerticalMove(chasePoints[0].transform.position))
                        {
                            if(chasePoints[0].pointType == PointType.LadderPoint)
                            {
                                subSate = SubState.ClimbLadder;
                            }
                            else
                            {
                                subSate = SubState.Start;
                            }
                        }
                        break;
                    case SubState.Jump:
                        if (ground != null) subSate = SubState.Start;
                        break;
                    case SubState.ClimbLadder:
                        if (ClimbLadder(chasePoints[1].transform.position)) subSate = SubState.Start;
                        break;
                }
                break;
            default:
                break;
        }
    }

    public void Reset()
    {
        state = State.Patrol;
        exclamationPoint.SetActive(false);
        transform.position = originalPos;
    }

    private void GetChasePoints()
    {
        chasePoints = GameManager.instance.navigate.GetPoints(GetComponent<Point>(), player.GetComponent<Point>());
    }

    private bool VerticalMove(Vector3 targetPos)
    {
        float delta = targetPos.x - transform.position.x;
        if (delta > 0)
        {
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            direction = 1;
        }
        else
        {
            transform.rotation = Quaternion.Euler(0.0f, 180f, 0.0f);
            direction = -1;
        }
        float deltaAbs = Mathf.Abs(delta);
        if (deltaAbs > Time.deltaTime * speed)
        {
            Vector3 translate = new Vector3(Time.deltaTime * speed, 0, 0);
            transform.Translate(translate);
            return false;
        }
        else
        {
            Vector3 translate = new Vector3(deltaAbs, 0, 0);
            transform.Translate(translate);
            return true;
        }
    }

    private bool ClimbLadder(Vector3 targetPos)
    {
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
        float delta = (targetPos.y + height) - transform.position.y;
        int d = delta > 0 ? 1 : -1;
        if (Mathf.Abs(delta) > Time.deltaTime * speed)
        {
            Vector3 translate = new Vector3(0, d, 0) * Time.deltaTime * speed;
            transform.Translate(translate);
            return false;
        }
        else
        {
            Vector3 translate = new Vector3(0, delta, 0);
            transform.Translate(translate);
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 1;
            return true;
        }
    }

    private bool CheckPlayer()
    {
        if(player == null)
            player = GameManager.instance.player;
        Vector3 vec3 = transform.position + new Vector3(alertDiatance * direction, 0, 0);
        Vector3 playerPos = player.position;
        if ((playerPos.x - transform.position.x) * (playerPos.x - vec3.x) < 0  && playerPos.y < transform.position.y + 1 && playerPos.y > transform.position.y - 1)
        {
            return true;
        }
        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "JumpBlock")
            subSate = SubState.Jump;
            ground = null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().Die();
        }
        if(collision.gameObject.tag == "Ground")
        {
            ground = collision.gameObject.name;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        ground = null;
    }

    public void Die()
    {
        GameManager.instance.RemoveEnemy(this);
        GameObject fx = GameObject.Instantiate(fxPrefab);
        fx.transform.position = transform.position;
        Destroy(gameObject);
    }

    public enum State
    {
        Relax,
        Patrol,
        Chase,
    }

    public enum SubState
    {
        Start,
        VerticalMove,
        Jump,
        ClimbLadder,
    }
}
