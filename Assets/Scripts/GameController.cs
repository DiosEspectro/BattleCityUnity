using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameController : MonoBehaviour {
    // �����, ������� ��������� ���� �����

    private AudioController aux; // ���������� �����
    [SerializeField]
    private GUIController guiController; // ���������� ������������ ����������

    private int curLevel = 1; // ������� �������
    private int players = 2; // ���������� �������

    // ������� ����
    private Base basePrefab;
    private Bricks bricksPrefab;
    private Concrete concretePrefab;
    private Forest forestPrefab;
    private Water waterPrefab;
    private Player playerPrefab;
    private Enemy enemyPrefab;
    private Respawner respawnerPrefab;
    private RespawnSpot respawnSpotPrefab;
    private Explode explodePrefab;
    private BonusItem bonusItemPrefab;

    // ���������� ��� �������� ������ �� �������
    private Base baseEagle = null;
    private Player player1 = null;
    private Player player2 = null;

    private GameObject player1spot;
    private GameObject player2spot;
    private GameObject enemy1spot;
    private GameObject enemy2spot;
    private GameObject enemy3spot;
    private int curEnemySpot = 1;

    private List<GameObject> spawnSpots = new List<GameObject>();

    // ���������� �� ������
    private int[] enemysList = null;
    private Enemy[] enemysOnScreenList = null;
    private int enemysLeft = 0;
    private int enemysOnScreen = 0;
    private int enemysMaxOnScreen = 0;
    private int enemyCur = 0;

    private bool needSpawnEnemy = true;
    private bool stopCreatingEnemy = false;

    private float needSpawnEnemySecs = 0f;
    private float needSpawnEnemyMaxSecs = 3f;

    // ������ ����������� ����������
    private bool needLevelClearDelay = false;
    private float levelClearDelaySesc = 0f;
    private float levelClearDelayMaxSecs = 3f;

    private List<Environment> baseWalls = new List<Environment>();
    private List<Environment> levelObjects = new List<Environment>();

    private int player1level = 1;
    private int player2level = 1;
    private int player1lifes = 3;
    private int player2lifes = 3;
    private bool player1dead = false;
    private bool player2dead = false;

    private bool gameStarted = false;
    private bool isGameOver = false;

    private bool canControl = true;
    private bool canShoot = true;

    private bool activateBaseBonus = false;
    private float baseBonusSesc = 0f;
    private float baseBonusMaxSecs = 10f;

    private bool playEngine = false;

    private void Awake() {
        // ������������� ��������
        aux = GameObject.FindObjectOfType(typeof(AudioController)) as AudioController;

        respawnSpotPrefab = Resources.Load<RespawnSpot>("RespawnSpot");
        playerPrefab = Resources.Load<Player>("Player");
        enemyPrefab = Resources.Load<Enemy>("Enemy");
        basePrefab = Resources.Load<Base>("Base");
        bricksPrefab = Resources.Load<Bricks>("Bricks");
        concretePrefab = Resources.Load<Concrete>("Concrete");
        forestPrefab = Resources.Load<Forest>("Forest");
        waterPrefab = Resources.Load<Water>("Water");
        respawnerPrefab = Resources.Load<Respawner>("Respawner");
        explodePrefab = Resources.Load<Explode>("Explode");
        bonusItemPrefab = Resources.Load<BonusItem>("BonusItem");
    }

    public AudioController getAudioController() { return aux; }

    void Start() {
        guiController.ShowMainMenu(); // ��� ������ ���� ���������� ������� ����
    }

    void FixedUpdate() {
        // ���� �������� �������
        if (gameStarted) {
            if (needLevelClearDelay) {
                LevelClearDelayProcess(); // ������ �������� ����� ����������� ������
            } else {
                EnemyCreatingDelayProcess(); // ��������� �������� �������� �����
                EnemiesCreator(); // �������� ������
            }
        }

        // ��������� ������, ����������� ������� ����
        if(activateBaseBonus) {
            baseBonusSesc += Time.fixedDeltaTime;
            if(baseBonusSesc > baseBonusMaxSecs) {
                activateBaseBonus = false;
                CreateBaseWalls('B'); // ���� ����� ������ ���������, ���������� �������
            }
        }
    }

    public void StartGame(int players) {
        // ������ ���� ����� ������ ���������� ������� � ������� ����

        curLevel = 0;
        player1dead = false;
        player2dead = false;
        player1level = 1;
        player1lifes = 3;
        player2level = 1;
        player2lifes = 3;
        this.players = players;
        guiController.ShowHidePlayerPanel(players);
        StartLevelIntro(); // �������� �������
    }


    private void StartLevelIntro() {
        // ������ ������
        curLevel++;
        guiController.StartLevelIntro(curLevel); // ��������� ��������
    }

    public void StartLevel() {
        // ������ ��������

        // ������ ������� �������
        if (!player1dead) CreateTank("player1");
        if (players > 1 && !player2dead) CreateTank("player2");
        canControl = true;
        canShoot = true;

        // ������ ������� �����
        enemysOnScreen = 0;
        CreateEnemy(1f);
        isGameOver = false;
        needLevelClearDelay = false;
        gameStarted = true;
        playEngine = true;
    }

    public void DestroyLevelObjects(bool withExplode = false) {
        // ����� ���������� ��� ������� �� ������� ����
        // ���� withExplode = true, �� ����������� �������������� ��������

        if (withExplode) aux.PlaySound(AudioController.ClipName.ExplodeEnemy); // ���� ������

        // ����������� ������ 1
        if (player1 != null) {
            if(withExplode) Instantiate(explodePrefab, player1.transform.position, player1.transform.rotation);
            player1.DestroyObject();
        }
        
        // ����������� ������ 2
        if (player2 != null) {
            if (withExplode) Instantiate(explodePrefab, player2.transform.position, player2.transform.rotation);
            player2.DestroyObject();
        }

        // ����������� ������ �� ������
        for (int i = 0; i < enemysMaxOnScreen; i++) {
            if (enemysOnScreenList[i] != null) {
                if (withExplode) Instantiate(explodePrefab, enemysOnScreenList[i].transform.position, enemysOnScreenList[i].transform.rotation);
                enemysOnScreenList[i].DestroyObject();
            }
        }

        // ����������� ��������� (�������, �����, ���, ����)
        foreach(Environment e in levelObjects) {
            if (e != null) {
                if (withExplode) Instantiate(explodePrefab, e.transform.position, e.transform.rotation);
                e.DestroyObject(0f); // ������� ������� � ���������
            }
        }

        // ����������� ����
        if(baseEagle != null) {
            if (withExplode) Instantiate(explodePrefab, baseEagle.transform.position, baseEagle.transform.rotation);
            baseEagle.DestroyObject(0f);
        }

        // ����������� ����� ��������� ������
        foreach(GameObject spot in spawnSpots) {
            if(spot != null) {
                Destroy(spot);
            }
        }
    }

    public void InitLevel() {
        // ������������� ������

        LoadLevel(curLevel); // ��������� �������

        guiController.updatePlayerLifes(1, player1lifes);
        if (players > 1) {
            guiController.updatePlayerLifes(2, player2lifes);
            enemysMaxOnScreen = 6;
        } else enemysMaxOnScreen = 4;

        enemyCur = 0;
        curEnemySpot = 1;
        enemysOnScreenList = new Enemy[enemysMaxOnScreen];
    }

    private void setLevelClear() {
        // ������� �������

        needLevelClearDelay = true;
        levelClearDelaySesc = 0f;
        levelClearDelayMaxSecs = 3f;
    }

    private void LevelClearDelayProcess() {
        // �������� ����� ����������� ������

        levelClearDelaySesc += Time.fixedDeltaTime;
        playEngine = false;

        if (levelClearDelaySesc > levelClearDelayMaxSecs) {
            // ���� �������� ���������, �� ��������� �������� � ��������� ������
            needLevelClearDelay = false;
            gameStarted = false;

            // ��������, ���� �� ���� ���������� ������
            // ���� ����, �� ����� ���������� � ���������� ������
            // ���� ���, �� ��� ������
            TextAsset level = Resources.Load<TextAsset>("Levels\\Level" + (curLevel + 1));
            if (level != null) {
                StartLevelIntro(); // �������� ��������� �������
            } else {
                canShoot = false;
                guiController.StartWinSequence(); // �������� �������� ��������
            }
        }
    }

    public void setGameOver() {
        // ��������� ����� ����
        if (!isGameOver) {
            isGameOver = true;
            gameStarted = false;
            playEngine = false;
            canControl = false;
            canShoot = false;
            guiController.StartGameOverSequence();
        }
    }

    private void LoadLevel(int level) {
        // �������� ������ �� �����

        int l = 0;

        string[] lines = (Resources.Load("Levels\\Level" + level) as TextAsset).text.Split("\r\n"); // ���������� ������ ����� � ������

        // ������������ ������
        foreach (string s in lines) {
            l++;
            if (l <= 26) {
                // ��������� ��� �������
                string[] objects = s.Split('	'); // ��������� �������
                for (int i = 1; i <= objects.Length; i++) {
                    // ���������, �������� �� ������ "���������"
                    if (!GenerateDefaultCells(i, l)) {
                        CreateEnvironment(objects[i - 1], i, l); // ������ ������
                    }
                }
            } else {
                // ��������� ������ ������
                string[] enemys = s.Split(';'); // ��������� ������
                Array.Resize(ref enemysList, enemys.Length);
                for (int i = 0; i < enemys.Length; i++) { 
                    enemysList[i] = int.Parse(enemys[i]); // ���������� ���� ������ � ������
                }
            }
        }

        enemysLeft = enemysList.Length; // ���������� ���������� � ���, ������� ������ ��������

        // ���������� ������������ ����������
        guiController.updateLevelNum(level);
        guiController.updateEnemiesPics(enemysLeft);
    }

    private void CreateBaseWalls(char material) {
        // ������ ����� ����

        // �� ������ ��������� ������, ���� ����
        foreach (Environment e in baseWalls) {
            if(e != null) e.DestroyObject(0f);
        }

        baseWalls = new List<Environment>(); // �������������� ������ List

        // �������� ����
        CreateEnvironment(material + "-2", 11, 25, true);
        CreateEnvironment(material + "-6", 11, 23, true);
        CreateEnvironment(material + "-3", 13, 23, true);
        CreateEnvironment(material + "-7", 15, 23, true);
        CreateEnvironment(material + "-4", 15, 25, true);
    }

    
    private bool GenerateDefaultCells(int x, int y, bool isOnlyCheck = false) {
        // ���������, �������� �� ������ "����������", ���� ��, �� ���������� ��������,
        // �������� ������������ �������� ������ � ���� ������
        // ���� isOnlyCheck = true, �� �� ���������� ��������, ������ ���������� ��������� ��������

        bool ret = false;

        // ����
        if (x >= 11 && x <= 15 && y >= 23 && y <= 26) {

            if (x == 12 && y == 23) {
                if (!isOnlyCheck) {
                    CreateBaseWalls('B'); // ��������� ������ ����
                    CreateBase(13, 25); // � ���� ����
                }
            }

            ret = true;
        }

        // ����� ��������� ��� 1 ������
        if (x >= 9 && x <= 10 && y >= 24 && y <= 26) {
            if (x == 10 && y == 25) {
                if (!isOnlyCheck) {
                    player1spot = new GameObject("player1_gs");
                    player1spot.transform.position = new Vector3(GetCoordinate(true, x), GetCoordinate(false, y), 0f);
                    spawnSpots.Add(player1spot);
                }
            }

            ret = true;
        }

        // ����� ��������� ��� 2 ������ (���� �� ������������)
        if (x >= 16 && x <= 17 && y >= 24 && y <= 26) {
            if(x == 16 && y == 25) {
                if (!isOnlyCheck) {
                    player2spot = new GameObject("player2_gs");
                    player2spot.transform.position = new Vector3(GetCoordinate(true, x), GetCoordinate(false, y), 0f);
                    spawnSpots.Add(player2spot);
                }
            }
            ret = true;
        }

        // ����� ��������� ����� #1
        if (x == 1 && y == 1) {
            if (!isOnlyCheck) {
                // ���������� ����� ��������� ����� #3
                enemy3spot = new GameObject("enemy3_gs");
                enemy3spot.transform.position = new Vector3(GetCoordinate(true, x), GetCoordinate(false, y), 0f);
                spawnSpots.Add(enemy3spot);
            }
            ret = true;
        }
        else if ((x == 1 && y == 2) || (x == 2 && (y == 1 || y == 2))) ret = true;

        else

        // ����� ��������� ����� #2
        if (x == 12 && (y == 1 || y == 2)) ret = true; // �������� ����, ����� �� ����� ��������� ����� ������ �� ����������
        else if ((x == 13 && y == 2) || (x == 14 && (y == 1 || y == 2))) ret = true; // ��������� ������������ � �����
        else if (x == 13 && y == 1) {
            if (!isOnlyCheck) {
                // ���������� ����� ��������� ����� #1
                enemy1spot = new GameObject("enemy1_gs");
                enemy1spot.transform.position = new Vector3(GetCoordinate(true, x), GetCoordinate(false, y), 0f);
                spawnSpots.Add(enemy1spot);
            }
            ret = true;
        } 
        
        else

        // ����� ��������� ����� #3
        if (x == 24 && (y == 1 || y == 2)) ret = true; // �������� ����, ����� �� ����� ��������� ����� ������ �� ����������
        else if ((x == 25 && y == 2) || (x == 26 && (y == 1 || y == 2))) ret = true; // ��������� ������������ � �����
        else if (x == 25 && y == 1) {
            if (!isOnlyCheck) {
                // ���������� ����� ��������� ����� #2
                enemy2spot = new GameObject("enemy2_gs");
                enemy2spot.transform.position = new Vector3(GetCoordinate(true, x), GetCoordinate(false, y), 0f);
                spawnSpots.Add(enemy2spot);
            }
            ret = true;
        }

        return ret;
    }

    private void CreateTank(string obj) {
        // ������ ����

        GameObject resSpot = null;
        UnityEngine.Object prefab = null;
        int type = 1;
        bool isBonusTank = false;

        // � ����������� �� ���������� �����, ������������� ��������� ��� ����������
        switch (obj) {
            case "player1": resSpot = player1spot; prefab = playerPrefab; type = 1; break;
            case "player2": resSpot = player2spot; prefab = playerPrefab; type = 2; break;
            case "enemy": 
                    resSpot = curEnemySpot == 1 ? enemy1spot : curEnemySpot == 2 ? enemy2spot : enemy3spot;
                    prefab = enemyPrefab;
                    type = enemysList[enemyCur++];
                    if(enemyCur == 4 || enemyCur == 11 || enemyCur == 18) isBonusTank = true;
                break;
        }

        // ������ ���������
        Respawner resp =  Instantiate(respawnerPrefab, resSpot.transform.position, Quaternion.identity);
        resp.setRespawnSettings(prefab, resSpot, type, isBonusTank);
    }

    private void CreateEnvironment(string obj, int x, int y, bool isBaseWalls = false) {
        // ����� ������ ������ ��������� �� �������� ����������

        float ex = GetCoordinate(true, x);
        float ey = GetCoordinate(false, y);

        string[] pars = obj.Split('-');
        string obj_class = pars[0];
        int obj_type = int.Parse(pars[1]);

        switch (obj_class) {
            case "B": // Bricks (�������)
                Bricks bricks = Instantiate(bricksPrefab, new Vector3(ex, ey, 0f), Quaternion.identity);
                bricks.SetType(obj_type);
                levelObjects.Add(bricks);
                if (isBaseWalls) baseWalls.Add(bricks);
                break;

            case "C": // Concrete (�������� �����)
                Concrete concrete = Instantiate(concretePrefab, new Vector3(ex, ey, 0f), Quaternion.identity);
                concrete.SetType(obj_type);
                levelObjects.Add(concrete);
                if (isBaseWalls) baseWalls.Add(concrete);
                break;

            case "F": // Forest (���)
                Forest forest = Instantiate(forestPrefab, new Vector3(ex, ey, 0f), Quaternion.identity);
                levelObjects.Add(forest);
                break;

            case "W": // Water (����)
                Water water = Instantiate(waterPrefab, new Vector3(ex, ey, 0f), Quaternion.identity);
                levelObjects.Add(water);
                break;
        }
    }

    private void CreateBase(int x, int y) {
        // �������� ����

        float bx = GetCoordinate(true, x);
        float by = GetCoordinate(false, y);

        baseEagle = Instantiate(basePrefab, new Vector3(bx, by, 0f), Quaternion.identity);
    }

    private void CreateEnemy(float delaySecs) {
        // �������� �����. �� ����� ������������� �������, ��� ����� ������� �����.
        needSpawnEnemy = true;
        needSpawnEnemySecs = 0f;
        needSpawnEnemyMaxSecs = delaySecs;
    }

    private void EnemiesCreator() {
        // ��������� ������. ���� ����������� ��� �������, �� �������� ����
        if(enemysLeft > 0 && enemysOnScreen < enemysMaxOnScreen && !needSpawnEnemy && !stopCreatingEnemy) {
            CreateEnemy(2f);
        }
    }

    private void EnemyCreatingDelayProcess() {
        // ������� �������� ����� ��������� �����
        if (needSpawnEnemy) {
            needSpawnEnemySecs += Time.fixedDeltaTime;

            if (needSpawnEnemySecs > needSpawnEnemyMaxSecs) {
                needSpawnEnemy = false;
                stopCreatingEnemy = true;
                CreateTank("enemy");
                curEnemySpot++;
                if (curEnemySpot > 3) curEnemySpot = 1;
            }
        }
    }

    public void EnemyHasBeenCreated(Enemy enemy) {
        // �����, � ������� ��������� ������ �� ���������� �����, ������� ����� ���������
        // � �������� ������ �������� ��� ����� ������
        guiController.updateEnemiesPics(--enemysLeft);
        enemysOnScreen++;

        PutEnemyToList(enemy);

        if (enemysLeft == 0) stopCreatingEnemy = true;
            else stopCreatingEnemy = false;
    }

    public void PlayerHasBeenCreated(int playerNum, Player player) {
        // �����, � ������� ��������� ������ �� ���������� ������, ������� ����� ��������� � ��������������� ����������

        if (playerNum == 1) player1 = player;
        else if(playerNum == 2) player2 = player;
    }

    public void PlayerExploded(int player) {
        // �����, �������������� ����� ������
        if (player == 1) {
            if (player1lifes > 0) {
                player1lifes--;
                SetPlayerLevel(player, 1);
                guiController.updatePlayerLifes(player, player1lifes);
                CreateTank("player1");
            } else player1dead = true;
        } else {
            if (player2lifes > 0) {
                player2lifes--;
                SetPlayerLevel(player, 1);
                guiController.updatePlayerLifes(player, player2lifes);
                CreateTank("player2");
            } else player2dead = true;
        }

        // ���� � ������� ������ �� ��������, �� ��� ����� ����
        if (
            (players == 1 && player1dead)
            ||
            (players == 2 && player1dead && player2dead)
           ) {
            setGameOver();
        }
    }

    public void EnemyExploded() {
        // �����, �������������� ����� �����
        enemysOnScreen--;

        // ���� ������ �� ��������, �� ������� �������
        if (enemysOnScreen == 0 && enemysLeft == 0) {
            setLevelClear();
        }
    }

    private float GetCoordinate(bool isX, int val) {
        // ����������� ��������� x ��� y, � ����������� �� ������� �� ������� ����
        float ret = 0f;

        float step = 0.5f;
        if(isX) ret = val * step;
            else ret = 13 - (val * step);

        return ret;
    }

    private void PutEnemyToList(Enemy enemy) {
        // ��������� ����� � ������
        for (int i = 0; i < enemysMaxOnScreen; i++) {
            if (enemysOnScreenList[i] == null) {
                enemysOnScreenList[i] = enemy;
                break;
            }
        }
    }

    public int GetPlayerLevel(int playerNum) {
        // ���������� ������� ������
        int ret;

        if (playerNum == 1) ret = player1level;
        else ret = player2level;

        return ret;
    }

    public void SetPlayerLevel(int playerNum, int level) {
        // ������������� ������� ������

        if (playerNum == 1) player1level = level;
        else player2level = level;
    }

    public bool CanControl() { return canControl; }
    public bool CanShoot() { return canShoot; }

    public bool isPlayEngine() { return playEngine; }
    
    public void GenerateBonus() {
        // ���������� �����
        int bx = 1;
        int by = 1;

        // ���������� ��������� ����� ��������� ������, �������� "�����������" ����� �� ��������� (���� ����, ���� �������� � �.�.)
        while (GenerateDefaultCells(bx, by, true)) {
            bx = UnityEngine.Random.Range(1, 26);
            by = UnityEngine.Random.Range(1, 26);
        }

        BonusItem bonusItem = Instantiate(bonusItemPrefab, new Vector3(GetCoordinate(true, bx), GetCoordinate(false, by), 0f), Quaternion.identity);
        levelObjects.Add(bonusItem);
    }

    public void BonusActivate_BaseWals() {
        // ��������� ������ - ������ ����
        activateBaseBonus = true;
        baseBonusSesc = 0f;
        baseBonusMaxSecs = 20f;

        CreateBaseWalls('C'); // ������ ����� ���� �� ��������
    }

    public void BonusActivate_ExplodeEnemies() {
        // ��������� ������ - ����� ���� ������ �� ������

        aux.PlaySound(AudioController.ClipName.ExplodeEnemy); // ������������� ���� ������

        // ���������� ���� ������ �� ������ � ���������� ��
        for (int i = 0; i < enemysMaxOnScreen; i++) {
            if (enemysOnScreenList[i] != null) {
                EnemyExploded();
                Instantiate(explodePrefab, enemysOnScreenList[i].transform.position, enemysOnScreenList[i].transform.rotation);
                enemysOnScreenList[i].DestroyObject();
            }
        }
    }

    public void BonusActivate_Timer() {
        // ��������� ������ - ��������� ������

        for (int i = 0; i < enemysMaxOnScreen; i++) {
            if (enemysOnScreenList[i] != null) {
                enemysOnScreenList[i].StartStunning(15f);
            }
        }
    }

    public void BonusActivate_IncreaseTankLife(int playerNum) {
        // ��������� ������ - ��������� ������ �����

        if (playerNum == 1) {
            player1lifes++;
            guiController.updatePlayerLifes(playerNum, player1lifes);
        } else if (playerNum == 2) {
            player2lifes++;
            guiController.updatePlayerLifes(playerNum, player2lifes);
        }
    }
}