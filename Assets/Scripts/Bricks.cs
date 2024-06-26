using System.Collections.Generic;
using UnityEngine;

public class Bricks : Environment {
    // Класс, отвечающий за кирпичные препятствия, родительский класс - Environment

    private AudioController aux;

    // Каждый кирпич имеет свой объект, для детальной разрушаемости
    [SerializeField]
    private BricksVisual brick1;
    [SerializeField]
    private BricksVisual brick2;
    [SerializeField]
    private BricksVisual brick3;
    [SerializeField]
    private BricksVisual brick4;
    [SerializeField]
    private BricksVisual brick5;
    [SerializeField]
    private BricksVisual brick6;
    [SerializeField]
    private BricksVisual brick7;
    [SerializeField]
    private BricksVisual brick8;
    [SerializeField]
    private BricksVisual brick9;
    [SerializeField]
    private BricksVisual brick10;
    [SerializeField]
    private BricksVisual brick11;
    [SerializeField]
    private BricksVisual brick12;
    [SerializeField]
    private BricksVisual brick13;
    [SerializeField]
    private BricksVisual brick14;
    [SerializeField]
    private BricksVisual brick15;
    [SerializeField]
    private BricksVisual brick16;

    private string type;
    private int bricksCount = 16;

    private BricksVisual[] bricks = new BricksVisual[16];

    private void Awake() {
        aux = GameObject.FindObjectOfType(typeof(AudioController)) as AudioController;

        bricks[0] = brick1;
        bricks[1] = brick2;
        bricks[2] = brick3;
        bricks[3] = brick4;
        bricks[4] = brick5;
        bricks[5] = brick6;
        bricks[6] = brick7;
        bricks[7] = brick8;
        bricks[8] = brick9;
        bricks[9] = brick10;
        bricks[10] = brick11;
        bricks[11] = brick12;
        bricks[12] = brick13;
        bricks[13] = brick14;
        bricks[14] = brick15;
        bricks[15] = brick16;
    }

    public void HitHandler(int brickNum, string direction, int hitPower, bool isPlayerBullet = false) {
        // Обработка попадания
        // brickNum - в какой кирпич пришлось попадание
        // direction - с какой стороны
        // hitPower - с какой силой
        // isPlayerBullet - является ли пуля пулей игрока

        int[] toDestroy = new int[2];
        int toDestroyNext = 0;

        // Кирпичи ломаются попарно, поэтому в зависимости от направления и номера кирпича, выбираем второй кирпич для уничтожения
        switch (direction) {
            case "UP":
            case "DOWN":
                if (brickNum == 13 || brickNum == 14) { toDestroy = new int[2] { 13, 14 }; toDestroyNext = (direction == "UP" ? 9 : 0); }
                if (brickNum == 9 || brickNum == 10) { toDestroy = new int[2] { 9, 10 }; toDestroyNext = (direction == "UP" ? 5 : 13); }
                if (brickNum == 5 || brickNum == 6) { toDestroy = new int[2] { 5, 6 }; toDestroyNext = (direction == "UP" ? 1 : 9); }
                if (brickNum == 1 || brickNum == 2) { toDestroy = new int[2] { 1, 2 }; toDestroyNext = (direction == "UP" ? 0 : 5); }
                if (brickNum == 15 || brickNum == 16) { toDestroy = new int[2] { 15, 16 }; toDestroyNext = (direction == "UP" ? 12 : 0); }
                if (brickNum == 11 || brickNum == 12) { toDestroy = new int[2] { 11, 12 }; toDestroyNext = (direction == "UP" ? 8 : 16); }
                if (brickNum == 7 || brickNum == 8) { toDestroy = new int[2] { 7, 8 }; toDestroyNext = (direction == "UP" ? 4 : 12); }
                if (brickNum == 3 || brickNum == 4) { toDestroy = new int[2] { 3, 4 }; toDestroyNext = (direction == "UP" ? 0 : 8); }
                break;
            case "LEFT":
            case "RIGHT":
                if (brickNum == 4 || brickNum == 8) { toDestroy = new int[2] { 4, 8 }; toDestroyNext = (direction == "RIGHT" ? 0 : 3); }
                if (brickNum == 3 || brickNum == 7) { toDestroy = new int[2] { 3, 7 }; toDestroyNext = (direction == "RIGHT" ? 4 : 2); }
                if (brickNum == 2 || brickNum == 6) { toDestroy = new int[2] { 2, 6 }; toDestroyNext = (direction == "RIGHT" ? 3 : 1); }
                if (brickNum == 1 || brickNum == 5) { toDestroy = new int[2] { 1, 5 }; toDestroyNext = (direction == "RIGHT" ? 2 : 0); }
                if (brickNum == 12 || brickNum == 16) { toDestroy = new int[2] { 12, 16 }; toDestroyNext = (direction == "RIGHT" ? 0 : 15); }
                if (brickNum == 11 || brickNum == 15) { toDestroy = new int[2] { 11, 15 }; toDestroyNext = (direction == "RIGHT" ? 16 : 14); }
                if (brickNum == 10 || brickNum == 14) { toDestroy = new int[2] { 10, 14 }; toDestroyNext = (direction == "RIGHT" ? 15 : 13); }
                if (brickNum == 9 || brickNum == 13) { toDestroy = new int[2] { 9, 13 }; toDestroyNext = (direction == "RIGHT" ? 14 : 0); }
                break;
        }

        // Уничтожаем оба кирпича
        DestroyBrick(toDestroy[0]);
        DestroyBrick(toDestroy[1]);

        // Если сила выстрела >1, то повторяем тоже самое, только со следующей парой кирпичей по направлению (чтобы уничтожить 4 кирпича вместо 2х)
        if(hitPower > 1 && toDestroyNext > 0) {
            HitHandler(toDestroyNext, direction, hitPower - 1);
        }

        // Воспроизводим звук, если пуля принадлежит игроку
        if (isPlayerBullet) aux.PlaySound(AudioController.ClipName.HitPowerful);
    }

    private void DestroyBrick(int brickNum) {
        // Скрываем кирпичи и уменьшаем количество активных кирпичей
        if(bricks[brickNum - 1].gameObject.activeSelf == true) {
            bricks[brickNum - 1].gameObject.SetActive(false);
            bricksCount--;
        }
        
        // Если видимых кирпичей не осталось, то удаляем объект
        if(bricksCount == 0) DestroyObject(1f);
    }

    public void SetType(int type) {
        // В зависимости от типа объекта, видны или не видны определённые кирпичи.
        // В этом методе определяем какие кирпичи видны при каком типе объекта.

        List<int> brickNumsToHide = null; // Содержит в себе номера кирпичей, которые нужно скрыть

        switch (type) {
            case 1: /* Здесь все активные */ break;
            case 2: brickNumsToHide = new List<int> { 1, 2, 5, 6, 9, 10, 13, 14 }; break;
            case 3: brickNumsToHide = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 }; break;
            case 4: brickNumsToHide = new List<int> { 3, 4, 7, 8, 11, 12, 15, 16 }; break;
            case 5: brickNumsToHide = new List<int> { 9, 10, 11, 12, 13, 14, 15, 16 }; break;
            case 6: brickNumsToHide = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 13, 14 }; break;
            case 7: brickNumsToHide = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 11, 12, 15, 16 }; break;
        }

        if(brickNumsToHide != null)
            foreach (int i in brickNumsToHide) {
                bricks[i-1].gameObject.SetActive(false);
                bricksCount--;
            }
    }
}