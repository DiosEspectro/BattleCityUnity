using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour { 
    // ���������� ������������ ����������

    private GameController game; // ���������� ����
    private AudioController aux; // ���������� �����

    // ������� � ���������� ��� �������� ����
    [SerializeField]
    private GameObject mainMenuObject;
    [SerializeField]
    private GameObject mainMenu_TitleScreen;
    [SerializeField]
    private GameObject mainMenu_Cursor;
    [SerializeField]
    private GameObject mainMenu_Player1Controls;
    [SerializeField]
    private GameObject mainMenu_Player2Controls;

    private bool showMenu = true;
    private int mainMenu_players = 1;

    // ������� � ���������� ��� ������������ ����������
    [SerializeField]
    private Image introBoxUp;
    [SerializeField]
    private Image introBoxBottom;
    [SerializeField]
    private TextMeshProUGUI introText;

    private bool introNeed = false;
    private float introSecs = 0f;
    private float introMaxSecs = 2f;
    private bool playLevel = false;

    private bool introLoading = false;

    [SerializeField]
    private Image blackout;
    [SerializeField]
    private TextMeshProUGUI winText;
    [SerializeField]
    private TextMeshProUGUI gameoverText;

    private bool winNeed = false;
    private int winSequencePhase = 0;
    private float winSecs = 0f;
    private float winMaxSecs = 2f;

    private bool gameoverNeed = false;
    private int gameoverSequencePhase = 0;
    private float gameoverSecs = 0f;
    private float gameoverMaxSecs = 2f;
    private bool gameoverMusicPlayed = false;

    [SerializeField]
    private Image guiEnemyPic_1;
    [SerializeField]
    private Image guiEnemyPic_2;
    [SerializeField]
    private Image guiEnemyPic_3;
    [SerializeField]
    private Image guiEnemyPic_4;
    [SerializeField]
    private Image guiEnemyPic_5;
    [SerializeField]
    private Image guiEnemyPic_6;
    [SerializeField]
    private Image guiEnemyPic_7;
    [SerializeField]
    private Image guiEnemyPic_8;
    [SerializeField]
    private Image guiEnemyPic_9;
    [SerializeField]
    private Image guiEnemyPic_10;
    [SerializeField]
    private Image guiEnemyPic_11;
    [SerializeField]
    private Image guiEnemyPic_12;
    [SerializeField]
    private Image guiEnemyPic_13;
    [SerializeField]
    private Image guiEnemyPic_14;
    [SerializeField]
    private Image guiEnemyPic_15;
    [SerializeField]
    private Image guiEnemyPic_16;
    [SerializeField]
    private Image guiEnemyPic_17;
    [SerializeField]
    private Image guiEnemyPic_18;
    [SerializeField]
    private Image guiEnemyPic_19;
    [SerializeField]
    private Image guiEnemyPic_20;
    [SerializeField]
    private Image guiEnemyPic_21;

    [SerializeField]
    private TextMeshProUGUI levelText;

    [SerializeField]
    private TextMeshProUGUI player1lifes;
    [SerializeField]
    private TextMeshProUGUI player2lifes;

    [SerializeField]
    private GameObject player1panel;
    [SerializeField]
    private GameObject player2panel;

    private Image[] guiEnemiesPics = new Image[21];

    void Start()
    {
        game = GameObject.FindObjectOfType(typeof(GameController)) as GameController;
        aux = GameObject.FindObjectOfType(typeof(AudioController)) as AudioController;

        // ���������� ��� ������ ������ � ������
        guiEnemiesPics[0] = guiEnemyPic_1;
        guiEnemiesPics[1] = guiEnemyPic_2;
        guiEnemiesPics[2] = guiEnemyPic_3;
        guiEnemiesPics[3] = guiEnemyPic_4;
        guiEnemiesPics[4] = guiEnemyPic_5;
        guiEnemiesPics[5] = guiEnemyPic_6;
        guiEnemiesPics[6] = guiEnemyPic_7;
        guiEnemiesPics[7] = guiEnemyPic_8;
        guiEnemiesPics[8] = guiEnemyPic_9;
        guiEnemiesPics[9] = guiEnemyPic_10;
        guiEnemiesPics[10] = guiEnemyPic_11;
        guiEnemiesPics[11] = guiEnemyPic_12;
        guiEnemiesPics[12] = guiEnemyPic_13;
        guiEnemiesPics[13] = guiEnemyPic_14;
        guiEnemiesPics[14] = guiEnemyPic_15;
        guiEnemiesPics[15] = guiEnemyPic_16;
        guiEnemiesPics[16] = guiEnemyPic_17;
        guiEnemiesPics[17] = guiEnemyPic_18;
        guiEnemiesPics[18] = guiEnemyPic_19;
        guiEnemiesPics[19] = guiEnemyPic_20;
        guiEnemiesPics[20] = guiEnemyPic_21;
    }

    void FixedUpdate() {
        
    }

    void Update() {
        if (showMenu) MainMenuProcess(); // ���� ����� �������� ����, ���������� ����
        if (introNeed) LevelIntroProcess(); // ���� ����� ���������� �������� ������ ������, ����������

        if (gameoverNeed) GameOverSequenceProcess(); // ���� ����� ���������� �������� ���������
        else 
        if (winNeed) WinSequenceProcess(); // ���� ����� ���������� �������� ������
    }

    public void ShowMainMenu() {
        // ���������� ������� ���� (��������� ��������)

        winNeed = false;
        gameoverNeed = false;

        blackout.color = new Color(blackout.color.r, blackout.color.g, blackout.color.b, 0f);

        mainMenuObject.SetActive(true);
        mainMenu_TitleScreen.transform.localPosition = new Vector3(mainMenu_TitleScreen.transform.localPosition.x, -700f, mainMenu_TitleScreen.transform.localPosition.z);
        MainMenuSetCursor(mainMenu_players);
        mainMenuObject.SetActive(true);
        showMenu = true;
    }

    private void MainMenuProcess() {
        // ������� ����������� � ���������� ������� ����

        if(mainMenu_TitleScreen.transform.localPosition.y < 0) {
            // ���� ���� ���� ���������� y=0, ��� ��������� �����

            Vector3 mm_tspos = mainMenu_TitleScreen.transform.localPosition;
            mainMenu_TitleScreen.transform.localPosition = new Vector3(mm_tspos.x, mm_tspos.y + 5f, mm_tspos.z);

            if(mainMenu_TitleScreen.transform.localPosition.y >= 0) mainMenu_TitleScreen.transform.localPosition = new Vector3(mm_tspos.x, 0f, mm_tspos.z);
        } else {
            // ����� ���� �� �����, ��������� ������� ������ ����������

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)) {
                // ����� ������ ����

                aux.PlaySound(AudioController.ClipName.MenuCursor);
                if (mainMenu_players == 1) mainMenu_players = 2; else mainMenu_players = 1;
                MainMenuSetCursor(mainMenu_players);
            } else if(Input.GetKeyDown(KeyCode.Space)) {
                // ������ ����

                aux.PlaySound(AudioController.ClipName.MenuSelect);
                game.StartGame(mainMenu_players); // ����� �������� ����
                showMenu = false;
            }
        }
    }

    private void MainMenuSetCursor(int players) {
        // ��������� ������� �� ������ �����

        float x = -154f;
        float y;
        if (players == 1) y = -43f;
        else y = -113f;

        mainMenu_Cursor.transform.localPosition = new Vector3(x, y, 0f);

        // ������ ����� �������� ���������� �� ����������
        if(players == 1) {
            mainMenu_Player2Controls.SetActive(false);
            mainMenu_Player1Controls.transform.localPosition = new Vector3(0, mainMenu_Player1Controls.transform.localPosition.y, mainMenu_Player1Controls.transform.localPosition.z);
        } else {
            mainMenu_Player2Controls.SetActive(true);
            mainMenu_Player1Controls.transform.localPosition = new Vector3(-250f, mainMenu_Player1Controls.transform.localPosition.y, mainMenu_Player1Controls.transform.localPosition.z);
        }
    }

    public void StartLevelIntro(int level) {
        // ��������� �������� ������

        Image im = introBoxUp.GetComponent<Image>();
        im.color = new Color(im.color.r, im.color.g, im.color.b, 1f);

        im = introBoxBottom.GetComponent<Image>();
        im.color = new Color(im.color.r, im.color.g, im.color.b, 1f);

        introBoxUp.rectTransform.sizeDelta = new Vector2(introBoxUp.rectTransform.sizeDelta.x, 0f);
        introBoxBottom.rectTransform.sizeDelta = new Vector2(introBoxBottom.rectTransform.sizeDelta.x, 0f);

        playLevel = false;
        introNeed = true;
        introSecs = 0f;
        introMaxSecs = 2f;
        introText.enabled = false;
        introText.text = "������� " + level;

        introLoading = false;
    }

    private void LevelIntroProcess() {
        // �����, �������������� �������� ������ ������

        if (introSecs == 0f && introBoxUp.rectTransform.sizeDelta.y < 384f) {
            // ������� �������

            introBoxUp.rectTransform.sizeDelta = new Vector2(introBoxUp.rectTransform.sizeDelta.x, introBoxUp.rectTransform.sizeDelta.y + 15f);
            introBoxBottom.rectTransform.sizeDelta = new Vector2(introBoxBottom.rectTransform.sizeDelta.x, introBoxBottom.rectTransform.sizeDelta.y + 15f);
        } else {
            if (!introLoading) {
                // ���� ����� �������� ������, �� ���������

                aux.PlaySound(AudioController.ClipName.LevelStart); // ������������� ������ ������ ������
                introLoading = true;
                mainMenuObject.SetActive(false);
                game.DestroyLevelObjects(); // ���������� ������ ������� ������
                game.InitLevel(); // ��������� �������
            }

            introSecs += Time.fixedDeltaTime;

            if (introSecs >= introMaxSecs) {
                // ���������� �������

                introText.enabled = false;

                introBoxUp.rectTransform.sizeDelta = new Vector2(introBoxUp.rectTransform.sizeDelta.x, Mathf.Lerp(introBoxUp.rectTransform.sizeDelta.y, 0, Time.deltaTime * 3f));
                introBoxBottom.rectTransform.sizeDelta = new Vector2(introBoxBottom.rectTransform.sizeDelta.x, Mathf.Lerp(introBoxBottom.rectTransform.sizeDelta.y, 0, Time.deltaTime * 3f));

                if (introBoxUp.rectTransform.sizeDelta.y <= 100f) {
                    // ����� ����� ���� ���� ����, ��� �������� ��������� � �������� ������
                    if (!playLevel) {
                        game.StartLevel();
                        playLevel = true;
                    }

                    Image im = introBoxUp.GetComponent<Image>();
                    im.color = new Color(im.color.r, im.color.g, im.color.b, im.color.a - 0.01f);

                    im = introBoxBottom.GetComponent<Image>();
                    im.color = new Color(im.color.r, im.color.g, im.color.b, im.color.a - 0.01f);

                    if (im.color.a <= 0f) introNeed = false;
                }
            } else introText.enabled = true;
        }
    }

    public void StartWinSequence() {
        // ������ �������� ������

        winNeed = true;
        winSecs = 0f;
        winMaxSecs = 6f;
        winSequencePhase = 0;
    }

    private void WinSequenceProcess() {
        // ������� �������� �������� ��������

        // ������ ���� - ������� �������� ������� ������
        if (winSequencePhase == 0) {
            Vector3 wt_tspos = winText.transform.localPosition;
            winText.transform.localPosition = new Vector3(wt_tspos.x, wt_tspos.y + 5f, wt_tspos.z);

            if (winText.transform.localPosition.y >= 0f) {
                winSequencePhase++;
                aux.PlaySound(AudioController.ClipName.Win);
                game.DestroyLevelObjects(true);
                aux.PlayerMoveStopSound(1);
                aux.PlayerMoveStopSound(2);
            }
        } else
        // ������ ���� - ���������� ������ ��� ��������
        if (winSequencePhase == 1) {
            Color bo_color = blackout.color;
            float alpha = bo_color.a;

            if (alpha < 1f) {
                alpha += 0.025f;
                if (alpha > 1f) alpha = 1f;

                blackout.color = new Color(bo_color.r, bo_color.g, bo_color.b, alpha);
            } else winSequencePhase++;
        } else
        // ������ ���� - ������� �� �������� �������
        if (winSequencePhase == 2) {
            winSecs += Time.fixedDeltaTime;

            if (winSecs >= winMaxSecs) {
                winSequencePhase++;
            }
        } else
        // �������� ���� - ������� �������� ������� ����
        if (winSequencePhase == 3) {
            Vector3 wt_tspos = winText.transform.localPosition;
            winText.transform.localPosition = new Vector3(wt_tspos.x, wt_tspos.y - 5f, wt_tspos.z);

            if (winText.transform.localPosition.y <= -420f) {
                winSecs = winMaxSecs / 2;
                winSequencePhase++;
            }
        } else
        // ����� ���� - ������� �� ������ �����
        if (winSequencePhase == 4) {
            winSecs += Time.fixedDeltaTime;

            if (winSecs >= winMaxSecs) {
                winSequencePhase = 0;
                winNeed = false;
                game.DestroyLevelObjects(); // ���������� ��� �������
                ShowMainMenu(); // ���������� ����
            }
        }
    }

    public void StartGameOverSequence() {
        // ������ �������� ���������

        gameoverNeed = true;
        gameoverSecs = 0f;
        gameoverMaxSecs = 4f;
        gameoverSequencePhase = 0;
    }

    private void GameOverSequenceProcess() {
        // ������� �������� ����������� ��������

        // ������ ���� - ������� ����������� ������� ������
        if (gameoverSequencePhase == 0) {
            aux.PlayerMoveStopSound(1);
            aux.PlayerMoveStopSound(2);

            Vector3 got_tspos = gameoverText.transform.localPosition;
            gameoverText.transform.localPosition = new Vector3(got_tspos.x, got_tspos.y + 5f, got_tspos.z);

            if (gameoverText.transform.localPosition.y >= 0f) {
                gameoverSequencePhase++;
            }
        } else
        // ������ ���� - ������� �� ����������� �������
        if (gameoverSequencePhase == 1) {
            gameoverSecs += Time.fixedDeltaTime;

            if (gameoverSecs >= gameoverMaxSecs) {
                gameoverSecs = 0f;
                gameoverSequencePhase++;
            }
        } else
        // ������ ���� - ��������� ����� ��� �������� � ������������� ������
        if (gameoverSequencePhase == 2) {
            Color bo_color = blackout.color;
            float alpha = bo_color.a;

            if (!gameoverMusicPlayed) {
                aux.PlaySound(AudioController.ClipName.GameOver);
                gameoverMusicPlayed = true;
            }

            if (alpha < 1f) {
                alpha += 0.025f;
                if (alpha > 1f) alpha = 1f;

                blackout.color = new Color(bo_color.r, bo_color.g, bo_color.b, alpha);
            } else {
                gameoverMusicPlayed = false;
                gameoverSequencePhase++;
            }
        } else
        // �������� ���� - ������� �� ����������� �������
        if (gameoverSequencePhase == 3) {
            gameoverSecs += Time.fixedDeltaTime;

            if (gameoverSecs >= gameoverMaxSecs) {
                gameoverSecs = 0f;
                gameoverSequencePhase++;
            }
        } else
        // ����� ���� - ������� ����������� ������� ����
        if (gameoverSequencePhase == 4) {
            Vector3 got_tspos = gameoverText.transform.localPosition;
            gameoverText.transform.localPosition = new Vector3(got_tspos.x, got_tspos.y - 5f, got_tspos.z);

            if (gameoverText.transform.localPosition.y <= -420f) {
                gameoverSequencePhase++;
                gameoverSecs = gameoverMaxSecs/2;
            }
        } else
        // ����� ���� - ������� �� ����������� �������
        if (gameoverSequencePhase == 5) {
            gameoverSecs += Time.fixedDeltaTime;

            if (gameoverSecs >= gameoverMaxSecs) {
                gameoverSequencePhase = 0;
                gameoverNeed = false;
                game.DestroyLevelObjects(); // ���������� ��� �������
                ShowMainMenu(); // ���������� ����
            }
        }
    }

    public void updateEnemiesPics(int enemies) {
        // ���������� ���������� ������ ������ �� ���������� ���������� ������
        for (int i = 0; i < guiEnemiesPics.Length; i++) {
            guiEnemiesPics[i].enabled = (i <= enemies - 1);
        }
    }

    public void updateLevelNum(int level) {
        // ���������� ������ ������
        levelText.text = level.ToString();
    }

    public void updatePlayerLifes(int player, int lifes) {
        // ���������� ���������� ���������� ������ � �������
        if (player == 1) player1lifes.text = lifes.ToString();
            else player2lifes.text = lifes.ToString();
    }

    public void ShowHidePlayerPanel(int player) {
        // ��������/������ ������ �������, ��� ���������� ���������� ������
        player1panel.gameObject.SetActive(true);
        player2panel.gameObject.SetActive(player == 2);
    }
}