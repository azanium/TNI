using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QAController : MonoBehaviour
{
    #region MemVars & Props

    static private QAController qaController;

    public enum QuestionCategory
    {
        Easy,
        Medium,
        Hard
    }

	public enum Choices
	{
		A,
		B, 
		C, 
		D, 
		E
	}

	public MainGameController gameController;

    public QuestionDialogController questionDialog;
    public YourTurnController yourTurnDialog;
    public QuestionCategoryController questionCategoryDialog;

    public UILabel question;
    public UILabel additionalInfo;
    public UISlider timeBar;
    public UIButton[] choices;

    private bool _enableTimer = false;
    private float _oldTime = 0f;
    private int _counterTime = 0;

    private List<QuestionData.question> usedQuestions;
    protected QuestionData.question activeQuestion;

    /// <summary>
    /// Map our question type with it's specific type, so we can randomize it based on its type
    /// </summary>
    protected Dictionary<GameData.PillarType, List<QuestionData.question>> questionsMap;


    // Choose Category Panel
    public GameObject chooseCategoryHint;
    public GameObject chooseCategoryButtons;

	public event System.Action<QuestionCategory> OnCategorySelected = null;
	public event System.Action<bool> OnChoiceSelected = null;

    #endregion


    #region MonoBehavior's Methods

    protected void Awake()
    {
        qaController = this;

        GameData.Load();
        QuestionData.Load("Xml/QnA");

        usedQuestions = new List<QuestionData.question>();

        QuestionData.current.EmitQuestionsMap(out questionsMap);
    }

	protected void Start() 
    {
        if (choices == null)
        {
            Debug.LogError("Choice buttons must never null");
            return;
        }

        foreach (UIButton choice in choices)
        {
            choice.enabled = false;
        }
	}
	
	protected void Update() 
    {
        if (Time.time - _oldTime >= 1 && _enableTimer)
        {
            _oldTime = Time.time;

            _counterTime++;
            if (_counterTime > GameData.current.GetCurrentDataSlot().timeLimit)
            {
                _counterTime = 0;
                StartCoroutine(ValidateChoice(-1));
            }
        }

        if (timeBar != null)
        {
            float value = (GameData.current.GetCurrentDataSlot().timeLimit - _counterTime) / GameData.current.GetCurrentDataSlot().timeLimit;
            timeBar.sliderValue = value;
            
        }
    }

    #endregion


    #region Internal Logic Methods

    private QuestionData.question GetNextQuestion(Node node)
    {
        return GetNextQuestion(node, GameData.PillarType.Red, false);
    }

    private QuestionData.question GetNextQuestion(Node node, GameData.PillarType defaultPillar, bool useDefaultPillar)
    {
        QuestionData.question result = null;
        
        // First get the pillar color code from the node that being selected by the player
        GameData.PillarType qtype;
        if (node != null)
        {
            qtype = GameData.GetColorTypeFromNodeType(node.nodeType);
        }
        else
        {
            qtype = defaultPillar;
        }

        if (useDefaultPillar)
        {
            qtype = defaultPillar;
        }

        // Create temp question list to fetch on
        List<QuestionData.question> questions = new List<QuestionData.question>();

        // Find on which colors are we about to take the question list from
        if (questionsMap.ContainsKey(qtype))
        {
            questions = questionsMap[qtype];
        }
        else // We are having QuestionType == Random here
        {
            // If we don't have the pillar color mapped on the questionsMap, just random the section

            int index = Random.Range(0, questionsMap.Keys.Count - 1);

            // Search our random key to get the list of the questions
            int i = 0;
            foreach (var qsValues in questionsMap.Keys)
            {
                if (index == i)
                {
                    questions = questionsMap[qsValues];
                    break;
                }
                i++;
            }
        }

        bool found = false;
        // We loop until we found the question that not being used yet, it's very dangerous, might end up infinite loop
        // TODO: DON'T USE LOOP
        while (!found)
        {
            int qRandomNo = Random.Range(0, questions.Count - 1);
            var que = questions[qRandomNo];
            if (usedQuestions.Contains(que) == false)
            {
                result = que;
                usedQuestions.Add(que);

                found = true;
            }
        }

        if (result == null)
        {
            usedQuestions.Clear();
            result = QuestionData.current.questions[0];
        }

        return result;
    }

    protected void StartQuestion(QuestionCategory category)
    {
        if (timeBar != null)
        {
            UIWidget widget = timeBar.foreground.GetComponent<UIWidget>();
            if (widget == null)
            {
                Debug.LogError("No Time Bar foreground found");
                return;
            }
            
            switch (category)
            {
                case QuestionCategory.Easy:
                    widget.color = new Color(0.43f, 0.7f, 0.47f);
                    break;

                case QuestionCategory.Medium:
                    widget.color = new Color(0.4f, 0.49f, 0.75f);
                    break;

                case QuestionCategory.Hard:
                    widget.color = new Color(0.98f, 0.53f, 0.53f);
                    break;
            }
        }
    }

    protected void HideAllChoices()
    {
        foreach (UIButton button in choices)
        {
            button.gameObject.SetActive(false);
        }
    }
	
    protected void PrepareQuestion(QuestionData.question q)
    {
        HideAllChoices();

        activeQuestion = q;

        question.text = q.content;
		additionalInfo.text = "";

        //foreach (QuestionData.answer answer in q.answers)
        for (int i = 0; i < q.answers.Count; i++)
        {
            QuestionData.answer answer = q.answers[i];
            if (i < choices.Length)
            {
                UIButton choice = choices[i];
                ChangeButtonColor(choice, Color.white);

                UILabel label = choice.transform.FindChild("Label").GetComponent<UILabel>();

                if (label != null) 
                {
                    label.text = answer.text;
                }
                choice.gameObject.SetActive(true);
            }
        }

        _enableTimer = true;
    }

    protected IEnumerator ValidateChoice(int choice)
    {
        _enableTimer = false;
        _counterTime = 0;
        bool isCorrect = false;
        
        SetButtonEnabled(false);

        List<int> corrects = new List<int>();

        for (int index = 0; index < activeQuestion.answers.Count; index++)
        {
            QuestionData.answer answer = activeQuestion.answers[index];
            if (answer.correct)
            {
                corrects.Add(index);
            }

            if (index == choice && answer.correct && !isCorrect)
            {
                isCorrect = true; 
            }
        }

        if (choice < 0)
        {
            isCorrect = false;
        }

        if (isCorrect == false && choice >= 0)
        {
            UIButton button = choices[choice];
            ChangeButtonColor(button, Color.red);
        }

        for (int cIndex = 0; cIndex < corrects.Count; cIndex++)
        {
            int correctIndex = corrects[cIndex];
            ChangeButtonColor(choices[correctIndex], Color.green);
        }

		float waitTime = 3;
		if (activeQuestion != null)
		{
			additionalInfo.text = activeQuestion.info;
			if (activeQuestion.info.Trim() != "")
			{
				waitTime = 6;
			}
		}

        yield return new WaitForSeconds(waitTime);

		activeQuestion = null;

		UINavigationController.DismissAllControllers();
		UINavigationController.DismissBackground("/MainBackground");
		if (OnChoiceSelected != null)
		{
			OnChoiceSelected(isCorrect);
		}
        //GameBoard.EndTurn(isCorrect);
    }

    protected void ChangeButtonColor(UIButton button, Color color)
    {
        var sprite = button.transform.FindChild("Background").GetComponent<UISlicedSprite>();
        if (sprite != null)
        {
            sprite.color = color;
        }
    }


    #endregion


    #region Event Methods

	protected void EndGame()
	{
		MainGameController.EndGame();
	}
    
    protected void OnEasyCategory()
    {
        //StartQuestion(QuestionCategory.Easy);
        //GameBoard.HighlightBlock(1);
		if (OnCategorySelected != null)
		{
			OnCategorySelected(QuestionCategory.Easy);
		}
    }

    protected void OnMediumCategory()
    {
        //StartQuestion(QuestionCategory.Medium);
        //GameBoard.HighlightBlock(2);
		if (OnCategorySelected != null)
		{
			OnCategorySelected(QuestionCategory.Medium);
		}
    }

    protected void OnHardCategory()
    {
        //StartQuestion(QuestionCategory.Hard);
        //GameBoard.HighlightBlock(3);
		if (OnCategorySelected != null)
		{
			OnCategorySelected(QuestionCategory.Hard);
		}
    }

    #endregion


    #region Public Methods

	public void StartTurn(MainGameController.PlayerInfo playerInfo)
	{
		var profile = playerInfo.playerData.GetPlayerProfile();
		Debug.Log("StartTurn: " + profile.Name);

		yourTurnDialog.ChangeText(profile.Name+System.Environment.NewLine+"TURN");
		UINavigationController.PushController(yourTurnDialog);
	}

    public void ShowQuestion(Node node)
    {
        QuestionData.question q = GetNextQuestion(node);

        ShowQuestionsPanel(q);
    }

    public void ShowCategory(Node node)
    {
		MainGameController.ShowHud(false);
		UINavigationController.PushBackground("/MainBackground");
		UINavigationController.PushController(questionCategoryDialog);
    }

    public void ShowQuestionsPanel(QuestionData.question q)
    {
        SetButtonEnabled(true);

		MainGameController.ShowHud(false);
		UINavigationController.PushBackground("/MainBackground");
		UINavigationController.PushController(questionDialog);

		PrepareQuestion(q);
    }

    /// <summary>
    /// Make the choice buttons clickable or not
    /// </summary>
    /// <param name="enabled"></param>
    public void SetButtonEnabled(bool enabled)
    {
        foreach (UIButton button in choices)
        {
            Collider collider = button.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = enabled;
            }
        }
    }
	
    #endregion


    #region Choose Category Events

    protected GameData.PillarType GetPillarTypeFromColorString(string color)
    {
        var pillarType = GameData.PillarType.Blue;

        switch (color)
        {
            case "red":
                pillarType = GameData.PillarType.Red;
                break;
            case "green":
                pillarType = GameData.PillarType.Green;
                break;
            case "yellow":
                pillarType = GameData.PillarType.Yellow;
                break;
            case "purple":
                pillarType = GameData.PillarType.Purple;
                break;
            case "orange":
                pillarType = GameData.PillarType.Orange;
                break;
        }

        return pillarType;
    }

    protected void OnCategorySet()
    {
        var btn = UIHelper.GetSelectedButton(chooseCategoryButtons);

        var pillarType = GetPillarTypeFromColorString(btn.tagText.ToLower());

        if (chooseCategoryHint != null)
        {
            UILabel label = UIHelper.GetLabel(chooseCategoryHint);
            if (label != null)
            {

                var questionType = GameData.GetQuestionTypeFromPillarType(pillarType);
                label.text = questionType;
            }
        }
    }

    protected void OnCategoryContinue()
    {
        var btn = UIHelper.GetSelectedButton(chooseCategoryButtons);
        
        var pillarType = GetPillarTypeFromColorString(btn.tagText.ToLower());

        QuestionData.question q = qaController.GetNextQuestion(null, pillarType, true);

        ShowQuestionsPanel(q);
    }

	public void OnChoiceA()
	{
		StartCoroutine(ValidateChoice(0));
	}
	
	public void OnChoiceB()
	{
		StartCoroutine(ValidateChoice(1));
	}
	
	public void OnChoiceC()
	{
		StartCoroutine(ValidateChoice(2));
	}
	
	public void OnChoiceD()
	{
		StartCoroutine(ValidateChoice(3));
	}
	
	public void OnChoiceE()
	{
		StartCoroutine(ValidateChoice(4));
	}

	public void NewGame()
	{
		//UINavigationController.DismissAllControllers();
		//UINavigationController.ClearControllers();

		EndGame();
		UINavigationController.PushController("/Play");
	}


    #endregion
}
