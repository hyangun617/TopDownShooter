using UnityEngine;
using TMPro;

public class TimeBoard : MonoBehaviour
{
    private float endTime = 0f;
    private float timer = 0;
    private TMP_Text timeText;

    private int minute = 0;

    void Awake()
    {
        timeText = GetComponent<TMP_Text>();
    }

    void Start()
    {
        endTime = GameManager.Instance.EndTime;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        minute = (int)(timer / 60f);
        
        timeText.text = $"{minute:D2}:" + timer.ToString("00.00");

        // 시간이 넘어가면 게임 종료.
        if(timer >= endTime) EndGame();
    }

    private void EndGame()
    {
        GameManager.Instance.ChangeState(GameState.Result);
    } 
}
