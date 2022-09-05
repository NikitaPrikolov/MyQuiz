using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;




public class GameScript : MonoBehaviour
{

    public QuestionList[] questions;
    public Text[] answersText;
    public Text qText;
    public GameObject headPanel;
    public Button[] answerBttns = new Button[3];
    public Button buttonPlay;
    public Sprite[] TFIcons = new Sprite[3];
    public Image TFIcon;
    public Text TFText;
    public GameObject result;
    public GameObject timeBar;
    public float t = 0;
    public float tmax = 25;
    public bool isOpen;
    public int score;
    [SerializeField] Text scoreText;
    public Text hightScore;
    public AudioSource Collect;
    public AudioSource Loose;
    public AudioSource StartSound;
    public AudioSource Answers;
    public AudioSource Times;
    public AudioSource Touch;
    public AudioSource Ender;

    List<object> qList;
    QuestionList crntQ;
    int randQ;
    bool defaultColor = false, trueColor = false, falseColor = false;

    void Start()
    {
        hightScore.text = PlayerPrefs.GetInt("best", 0).ToString();
        timeBar.GetComponent<Animator>().SetTrigger("State");
        isOpen = false;
        TFIcon.GetComponent<Animator>().SetTrigger("Out");
        for (int i = 0; i < answerBttns.Length; i++) answerBttns[i].gameObject.GetComponent<Animator>().SetTrigger("State");
    }
    void Update()
    {
        scoreText.text = score.ToString();
        if (score > PlayerPrefs.GetInt("best", 0))
        {
            PlayerPrefs.SetInt("best", score);
            hightScore.text = score.ToString();
        }
        if (defaultColor) headPanel.GetComponent<Image>().color = Color.Lerp(headPanel.GetComponent<Image>().color, new Color(234 / 255.0F, 76 / 255.0F, 137 / 255.0F), 8 * Time.deltaTime);
        if (trueColor) headPanel.GetComponent<Image>().color = Color.Lerp(headPanel.GetComponent<Image>().color, new Color(104 / 255.0F, 184 / 255.0F, 89 / 255.0F), 8 * Time.deltaTime);
        if (falseColor) headPanel.GetComponent<Image>().color = Color.Lerp(headPanel.GetComponent<Image>().color, new Color(192 / 255.0F, 57 / 255.0F, 43 / 255.0F), 8 * Time.deltaTime);
        if (isOpen == true)
        {
            t = t + Time.deltaTime;
            if (t >= tmax)
            {
                StartCoroutine(Tiiime());
                timeBar.GetComponent<Animator>().SetTrigger("State");
                isOpen = false;
                t = 0;
            }
        }

    }
    public void OnClickPlay()
    {
        buttonPlay.interactable = false;
        Touch.Play();
        StartSound.Play();
        score = 0;
        qList = new List<object>(questions);
        questionGenerate();
        if (!headPanel.GetComponent<Animator>().enabled) headPanel.GetComponent<Animator>().enabled = true;
        else headPanel.GetComponent<Animator>().SetTrigger("In");
    }
    void questionGenerate()
    {
        if (qList.Count > 0)
        {
            randQ = Random.Range(0, qList.Count);
            crntQ = qList[randQ] as QuestionList;
            qText.text = crntQ.question;
            qText.gameObject.GetComponent<Animator>().SetTrigger("In");
            List<string> answers = new List<string>(crntQ.answers);
            for (int i = 0; i < crntQ.answers.Length; i++)
            {
                int rand = Random.Range(0, answers.Count);
                answersText[i].text = answers[rand];
                answers.RemoveAt(rand);
            }
            StartCoroutine(animBttns());
        }
        else
        {
            Ender.Play();
            result.GetComponent<Animator>().SetTrigger("In");
            Invoke("Out", 2.8f);
            result.GetComponent<Animator>().SetTrigger("State");
        }
    }
    void Out()
    {
        headPanel.GetComponent<Animator>().SetTrigger("Out");
        StartSound.Play();
    }
    IEnumerator animBttns()
    {
        yield return new WaitForSeconds(1);
        for (int i = 0; i < answerBttns.Length; i++) answerBttns[i].interactable = false;
        int a = 0;
        while (a < answerBttns.Length)
        {
            if (!answerBttns[a].gameObject.activeSelf) answerBttns[a].gameObject.SetActive(true);
            else answerBttns[a].gameObject.GetComponent<Animator>().SetTrigger("In");
            Answers.Play();
            a++;
            yield return new WaitForSeconds(1);
            
            
        }
        for (int i = 0; i < answerBttns.Length; i++) answerBttns[i].interactable = true;
        timeBar.GetComponent<Animator>().SetTrigger("In");
        Times.Play();
        isOpen = true;
        yield break;
    }
    IEnumerator trueOrFalse(bool check)
    {
        
        timeBar.GetComponent<Animator>().SetTrigger("State");
        isOpen = false;
        t = 0;
        defaultColor = false;
            for (int i = 0; i < answerBttns.Length; i++) answerBttns[i].interactable = false;
            yield return new WaitForSeconds(1);
            for (int i = 0; i < answerBttns.Length; i++) answerBttns[i].gameObject.GetComponent<Animator>().SetTrigger("Out");
            qText.gameObject.GetComponent<Animator>().SetTrigger("Out");
        Times.Stop();
        yield return new WaitForSeconds(0.5f);
        if (!TFIcon.gameObject.activeSelf) TFIcon.gameObject.SetActive(true);
            else TFIcon.gameObject.GetComponent<Animator>().SetTrigger("In");
            if (check)
            {
            
            score++;
                trueColor = true;
                TFIcon.sprite = TFIcons[0];
                TFText.text = "œ–¿¬»À‹Õ€… Œ“¬≈“";
            buttonPlay.interactable = true;
            Collect.Play();
            yield return new WaitForSeconds(2);
                TFIcon.gameObject.GetComponent<Animator>().SetTrigger("Out");
                qList.RemoveAt(randQ);
                questionGenerate();
                trueColor = false;
                defaultColor = true;
                yield break;
            }
            else
            {
                falseColor = true;
                TFIcon.sprite = TFIcons[1];
                TFText.text = "Õ≈œ–¿¬»À‹Õ€… Œ“¬≈“";
            buttonPlay.interactable = true;
            Loose.Play();
            yield return new WaitForSeconds(2);
                TFIcon.gameObject.GetComponent<Animator>().SetTrigger("Out");
                headPanel.GetComponent<Animator>().SetTrigger("Out");
            StartSound.Play();
            falseColor = false;
                defaultColor = true;
                yield break;
            }
    }
    IEnumerator Tiiime()
    {
        for (int i = 0; i < answerBttns.Length; i++) answerBttns[i].interactable = false;
        yield return new WaitForSeconds(1);
        for (int i = 0; i < answerBttns.Length; i++) answerBttns[i].gameObject.GetComponent<Animator>().SetTrigger("Out");
        qText.gameObject.GetComponent<Animator>().SetTrigger("Out");
        Times.Stop();
        yield return new WaitForSeconds(0.5f);
        if (!TFIcon.gameObject.activeSelf) TFIcon.gameObject.SetActive(true);
        else TFIcon.gameObject.GetComponent<Animator>().SetTrigger("In");
        falseColor = true;
        TFIcon.sprite = TFIcons[2];
        TFText.text = "¬–≈Ãﬂ ¬€ÿÀŒ";
        buttonPlay.interactable = true;
        Loose.Play();
        yield return new WaitForSeconds(2);
        TFIcon.gameObject.GetComponent<Animator>().SetTrigger("Out");
        headPanel.GetComponent<Animator>().SetTrigger("Out");
        StartSound.Play();
        falseColor = false;
        defaultColor = true;
        yield break;
        
    }
    public void AnswerBttns(int index)
    {
        Touch.Play();
        if (answersText[index].text.ToString() == crntQ.answers[0]) StartCoroutine(trueOrFalse(true));
        else StartCoroutine(trueOrFalse(false));
    }
}
[System.Serializable]
public class QuestionList
    {
        public string question;
        public string[] answers = new string[3];
    }

