using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour { 
    // Контроллер графического интерфейса

    private GameController game; // Контроллер игры
    private AudioController aux; // Контроллер звука

    // Ресурсы и переменные для главного меню
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

    // Объекты и переменные для графического интерфейса
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

        // Перегоняем все иконки врагов в массив
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
        if (showMenu) MainMenuProcess(); // Если нужно показать меню, показываем меню
        if (introNeed) LevelIntroProcess(); // Если нужно показывать анимацию начала уровня, показываем

        if (gameoverNeed) GameOverSequenceProcess(); // Если нужно показывать анимацию поражения
        else 
        if (winNeed) WinSequenceProcess(); // Если нужно показывать анимацию победы
    }

    public void ShowMainMenu() {
        // Показываем главное меню (запускаем анимацию)

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
        // Процесс отображения и управления главным меню

        if(mainMenu_TitleScreen.transform.localPosition.y < 0) {
            // Пока меню ниже координаты y=0, оно двигается вверх

            Vector3 mm_tspos = mainMenu_TitleScreen.transform.localPosition;
            mainMenu_TitleScreen.transform.localPosition = new Vector3(mm_tspos.x, mm_tspos.y + 5f, mm_tspos.z);

            if(mainMenu_TitleScreen.transform.localPosition.y >= 0) mainMenu_TitleScreen.transform.localPosition = new Vector3(mm_tspos.x, 0f, mm_tspos.z);
        } else {
            // Когда меню на месте, считываем нажатие клавиш клавиатуры

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)) {
                // Выбор пункта меню

                aux.PlaySound(AudioController.ClipName.MenuCursor);
                if (mainMenu_players == 1) mainMenu_players = 2; else mainMenu_players = 1;
                MainMenuSetCursor(mainMenu_players);
            } else if(Input.GetKeyDown(KeyCode.Space)) {
                // Запуск игры

                aux.PlaySound(AudioController.ClipName.MenuSelect);
                game.StartGame(mainMenu_players); // Здесь начинаем игру
                showMenu = false;
            }
        }
    }

    private void MainMenuSetCursor(int players) {
        // Установка курсора на нужный пункт

        float x = -154f;
        float y;
        if (players == 1) y = -43f;
        else y = -113f;

        mainMenu_Cursor.transform.localPosition = new Vector3(x, y, 0f);

        // Заодно можно изменить информацию об управлении
        if(players == 1) {
            mainMenu_Player2Controls.SetActive(false);
            mainMenu_Player1Controls.transform.localPosition = new Vector3(0, mainMenu_Player1Controls.transform.localPosition.y, mainMenu_Player1Controls.transform.localPosition.z);
        } else {
            mainMenu_Player2Controls.SetActive(true);
            mainMenu_Player1Controls.transform.localPosition = new Vector3(-250f, mainMenu_Player1Controls.transform.localPosition.y, mainMenu_Player1Controls.transform.localPosition.z);
        }
    }

    public void StartLevelIntro(int level) {
        // Запускает заставку уровня

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
        introText.text = "Уровень " + level;

        introLoading = false;
    }

    private void LevelIntroProcess() {
        // Метод, обрабатывающий анимацию начала уровня

        if (introSecs == 0f && introBoxUp.rectTransform.sizeDelta.y < 384f) {
            // Сужение панелей

            introBoxUp.rectTransform.sizeDelta = new Vector2(introBoxUp.rectTransform.sizeDelta.x, introBoxUp.rectTransform.sizeDelta.y + 15f);
            introBoxBottom.rectTransform.sizeDelta = new Vector2(introBoxBottom.rectTransform.sizeDelta.x, introBoxBottom.rectTransform.sizeDelta.y + 15f);
        } else {
            if (!introLoading) {
                // Если нужна загрузка данных, то загружаем

                aux.PlaySound(AudioController.ClipName.LevelStart); // Воспроизводим музыку начала уровня
                introLoading = true;
                mainMenuObject.SetActive(false);
                game.DestroyLevelObjects(); // Уничтожаем старые объекты уровня
                game.InitLevel(); // Загружаем уровень
            }

            introSecs += Time.fixedDeltaTime;

            if (introSecs >= introMaxSecs) {
                // Размыкание панелей

                introText.enabled = false;

                introBoxUp.rectTransform.sizeDelta = new Vector2(introBoxUp.rectTransform.sizeDelta.x, Mathf.Lerp(introBoxUp.rectTransform.sizeDelta.y, 0, Time.deltaTime * 3f));
                introBoxBottom.rectTransform.sizeDelta = new Vector2(introBoxBottom.rectTransform.sizeDelta.x, Mathf.Lerp(introBoxBottom.rectTransform.sizeDelta.y, 0, Time.deltaTime * 3f));

                if (introBoxUp.rectTransform.sizeDelta.y <= 100f) {
                    // Здесь нужно дать игре знак, что анимация закончена и скрывать панели
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
        // Запуск анимации победы

        winNeed = true;
        winSecs = 0f;
        winMaxSecs = 6f;
        winSequencePhase = 0;
    }

    private void WinSequenceProcess() {
        // Цепочка действий победной анимации

        // Первая фаза - двигаем победную надпись наверх
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
        // Вторая фаза - затемнение экрана под надписью
        if (winSequencePhase == 1) {
            Color bo_color = blackout.color;
            float alpha = bo_color.a;

            if (alpha < 1f) {
                alpha += 0.025f;
                if (alpha > 1f) alpha = 1f;

                blackout.color = new Color(bo_color.r, bo_color.g, bo_color.b, alpha);
            } else winSequencePhase++;
        } else
        // Третья фаза - смотрим на победную надпись
        if (winSequencePhase == 2) {
            winSecs += Time.fixedDeltaTime;

            if (winSecs >= winMaxSecs) {
                winSequencePhase++;
            }
        } else
        // Четвёртая фаза - двигаем победную надпись вниз
        if (winSequencePhase == 3) {
            Vector3 wt_tspos = winText.transform.localPosition;
            winText.transform.localPosition = new Vector3(wt_tspos.x, wt_tspos.y - 5f, wt_tspos.z);

            if (winText.transform.localPosition.y <= -420f) {
                winSecs = winMaxSecs / 2;
                winSequencePhase++;
            }
        } else
        // Пятая фаза - смотрим на чёрный экран
        if (winSequencePhase == 4) {
            winSecs += Time.fixedDeltaTime;

            if (winSecs >= winMaxSecs) {
                winSequencePhase = 0;
                winNeed = false;
                game.DestroyLevelObjects(); // Уничтожаем все объекты
                ShowMainMenu(); // Показываем меню
            }
        }
    }

    public void StartGameOverSequence() {
        // Запуск анимации поражения

        gameoverNeed = true;
        gameoverSecs = 0f;
        gameoverMaxSecs = 4f;
        gameoverSequencePhase = 0;
    }

    private void GameOverSequenceProcess() {
        // Цепочка действий проигрышной анимации

        // Первая фаза - двигаем проигрышную надпись наверх
        if (gameoverSequencePhase == 0) {
            aux.PlayerMoveStopSound(1);
            aux.PlayerMoveStopSound(2);

            Vector3 got_tspos = gameoverText.transform.localPosition;
            gameoverText.transform.localPosition = new Vector3(got_tspos.x, got_tspos.y + 5f, got_tspos.z);

            if (gameoverText.transform.localPosition.y >= 0f) {
                gameoverSequencePhase++;
            }
        } else
        // Вторая фаза - смотрим на проигрышную надпись
        if (gameoverSequencePhase == 1) {
            gameoverSecs += Time.fixedDeltaTime;

            if (gameoverSecs >= gameoverMaxSecs) {
                gameoverSecs = 0f;
                gameoverSequencePhase++;
            }
        } else
        // Третья фаза - затемняем экран под надписью и воспроизводим музыку
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
        // Четвёртая фаза - смотрим на проигрышную надпись
        if (gameoverSequencePhase == 3) {
            gameoverSecs += Time.fixedDeltaTime;

            if (gameoverSecs >= gameoverMaxSecs) {
                gameoverSecs = 0f;
                gameoverSequencePhase++;
            }
        } else
        // Пятая фаза - двигаем проигрышную надпись вниз
        if (gameoverSequencePhase == 4) {
            Vector3 got_tspos = gameoverText.transform.localPosition;
            gameoverText.transform.localPosition = new Vector3(got_tspos.x, got_tspos.y - 5f, got_tspos.z);

            if (gameoverText.transform.localPosition.y <= -420f) {
                gameoverSequencePhase++;
                gameoverSecs = gameoverMaxSecs/2;
            }
        } else
        // Пятая фаза - смотрим на проигрышную надпись
        if (gameoverSequencePhase == 5) {
            gameoverSecs += Time.fixedDeltaTime;

            if (gameoverSecs >= gameoverMaxSecs) {
                gameoverSequencePhase = 0;
                gameoverNeed = false;
                game.DestroyLevelObjects(); // Уничтожаем все объекты
                ShowMainMenu(); // Показываем меню
            }
        }
    }

    public void updateEnemiesPics(int enemies) {
        // Обновление пиктограмм врагов исходя из количества оставшихся врагов
        for (int i = 0; i < guiEnemiesPics.Length; i++) {
            guiEnemiesPics[i].enabled = (i <= enemies - 1);
        }
    }

    public void updateLevelNum(int level) {
        // Обновление номера уровня
        levelText.text = level.ToString();
    }

    public void updatePlayerLifes(int player, int lifes) {
        // Обновление количества оставшихся жизней у игроков
        if (player == 1) player1lifes.text = lifes.ToString();
            else player2lifes.text = lifes.ToString();
    }

    public void ShowHidePlayerPanel(int player) {
        // Показать/скрыть панели игроков, где отображено количество жизней
        player1panel.gameObject.SetActive(true);
        player2panel.gameObject.SetActive(player == 2);
    }
}