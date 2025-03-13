using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class QuizManager : MonoBehaviour
{
    public List<QuestionAndAnswer> Qna;
    public GameObject[] option;
    public int CurrentQuestion;

    public GameObject Quizpanel;
    public GameObject GOPanel;

    public Text QuestionTxt;
    public Text Scoretxt;

    int TotalQuestions = 0;
    public int score;

    private void Start()
    {
        TotalQuestions = Qna.Count;
        GOPanel.SetActive(false);
        generateQuestion();
    }

    public void retry(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void changescene(){
        Debug.Log("Jugador cambia a Game 1");
        SceneManager.LoadScene("Game1");
    }

    void GameOver(){
        Quizpanel.SetActive(false);
        GOPanel.SetActive(true);
        Scoretxt.text = score + "/" + TotalQuestions;
    }

    public void correct()
    {
        score += 1;
        Qna.RemoveAt(CurrentQuestion);
        generateQuestion();
    }

    public void wrong(){
        Qna.RemoveAt(CurrentQuestion);
        generateQuestion();
    }

    void setAnswer(){
        for(int i = 0; i < option.Length; i++){
            option[i].GetComponent<AnswerScript>().isCorrect = false;
            option[i].transform.GetChild(0).GetComponent<Text>().text = Qna[CurrentQuestion].Answer[i];
            if(Qna[CurrentQuestion].CorrectAnswer == i+1){
                option[i].GetComponent<AnswerScript>().isCorrect = true;
            }
        }
    }

    void generateQuestion(){
        if(Qna.Count > 0){
            CurrentQuestion = Random.Range(0, Qna.Count);
            QuestionTxt.text = Qna[CurrentQuestion].Question;
            setAnswer();
        }
        else{
            Debug.Log("Out of Question");
            GameOver();
        }
    }
}
