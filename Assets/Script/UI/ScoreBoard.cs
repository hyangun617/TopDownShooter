using UnityEngine;
using TMPro;

public class ScoreBoard : MonoBehaviour
{
    private float elapsedTime = 0f;        // 누적 시간.
    private float timer = 0f;

    // 점수
    private static int score = 0;
    public static int Score => score;

    private TMP_Text tmpText;

    private void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
    }

    private void Update() 
    {
        timer += Time.deltaTime;
        elapsedTime += Time.deltaTime;

        if(timer >= 1f)
        {
            timer -= 1f;
            OnSecondPassed();
        }

        PrintScore(score);
    }

    private void OnSecondPassed()
    {
        score = GameManager.Instance.Score + 1;
        GameManager.Instance.SetScore(score);
    }

    public void AddScore(int value)
    {
        score += value;
    }

    private void PrintScore(float value)
    {
        tmpText.text = value.ToString();
    }
}