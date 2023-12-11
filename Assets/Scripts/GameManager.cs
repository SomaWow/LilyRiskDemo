using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    private int totalStarNum = 0;
    private int starNum = 0;
    private int bombNum = 0;
    private int deathNum = 0;

    public Transform rebirthTrans;
    
    public Button bombBtn;
    public GameObject joyStick;
    public GameObject scoreText;
    public GameObject lightBombPrefab;
    public Transform player;
    public Navigate navigate;
    private List<Enemy> enemies;

    public GameObject settlementPanel;
    public TextMeshProUGUI starNumTmp;
    public TextMeshProUGUI deathNumTmp;
    public Button ResetBtn;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        navigate = new Navigate();
        settlementPanel.SetActive(false);
        bombBtn.gameObject.SetActive(false);
        bombBtn.onClick.AddListener(UseBomb);

        totalStarNum = GameObject.Find("Stars").transform.childCount;

        enemies = new List<Enemy>(GameObject.Find("Enemies").GetComponentsInChildren<Enemy>());
        //重启游戏
        ResetBtn.GetComponent<Button>().onClick.AddListener(()=>{
            SceneManager.LoadScene("SampleScene");
        });
    }

    public void RemoveEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
    }

    public void GetBomb()
    {
        bombNum++;
        bombBtn.gameObject.SetActive(true);
        bombBtn.transform.Find("text").GetComponent<TextMeshProUGUI>().text = bombNum.ToString();

    }

    public void UseBomb()
    {
        if(bombNum > 0)
        {
            bombNum--;
            GameObject go = GameObject.Instantiate(lightBombPrefab);
            go.transform.position = player.position;
        }

        if (bombNum <= 0)
            bombBtn.gameObject.SetActive(false);
    }

    public void GetStar()
    {
        starNum++;
        scoreText.GetComponent<TextMeshProUGUI>().text = string.Format("{0}/{1}", starNum, totalStarNum);
    }

    public void PlayerDied()
    {
        deathNum++;
        bombBtn.gameObject.SetActive(false);
        joyStick.SetActive(false);
        if(enemies.Count > 0)
        {
            foreach(var e in enemies)
            {
                e.enabled = false;
            }
        }
        joyStick.GetComponent<bl_Joystick>().Reset();
    }
    public void Rebirth()
    {
        bombBtn.gameObject.SetActive(true);
        joyStick.SetActive(true);

        //玩家回到初始位置
        player.transform.position = new Vector3(0, 0, 0);
        if(enemies.Count > 0)
        {
            foreach(var e in enemies)
            {
                e.enabled = true;
                e.Reset();
            }
        }

    }

    public void GameOver()
    {
        settlementPanel.SetActive(true);
        // 设置数据
        starNumTmp.text = starNum.ToString();
        deathNumTmp.text = deathNum.ToString();
    }
}
