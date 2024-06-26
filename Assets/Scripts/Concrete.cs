using System.Collections.Generic;
using UnityEngine;

public class Concrete : Environment {
    // Класс, отвечающий за бетонные препятствия, родительский класс - Environment

    private AudioController aux;

    // Бетонные препятствия состоят из четырёх частей
    [SerializeField]
    private ConcreteVisual block1;
    [SerializeField]
    private ConcreteVisual block2;
    [SerializeField]
    private ConcreteVisual block3;
    [SerializeField]
    private ConcreteVisual block4;

    private int blocksCount = 4;

    private ConcreteVisual[] blocks = new ConcreteVisual[4];

    private void Awake() {
        aux = GameObject.FindObjectOfType(typeof(AudioController)) as AudioController;

        blocks[0] = block1;
        blocks[1] = block2;
        blocks[2] = block3;
        blocks[3] = block4;
    }

    public void HitHandler(int blockNum, int hitPower, bool isPlayerBullet = false) {
        // Обработка попадания по блокам
        // blockNum - в какой блок пришлось попадание
        // hitPower - с какой силой
        // isPlayerBullet - является ли пуля пулей игрока

        if (hitPower > 1) {
            if(isPlayerBullet) aux.PlaySound(AudioController.ClipName.HitPowerful);
            DestroyBlock(blockNum);
        } else {
            if (isPlayerBullet) aux.PlaySound(AudioController.ClipName.HitConcrete);
        }
    }

    private void DestroyBlock(int blockNum) {
        // Скрываем кирпичи и уменьшаем количество активных блоков
        if (blocks[blockNum - 1].gameObject.activeSelf == true) {
            blocks[blockNum - 1].gameObject.SetActive(false);
            blocksCount--;
        }

        // Если видимых блоков не осталось, то удаляем объект
        if (blocksCount == 0) DestroyObject(1f);
    }

    public void SetType(int type) {
        // В зависимости от типа объекта, видны или не видны определённые блоки.
        // В этом методе определяем какие блоки видны при каком типе объекта.

        List<int> blockNumsToHide = null;

        switch (type) {
            case 1: /* Здесь все активные */ break;
            case 2: blockNumsToHide = new List<int> { 1, 3 }; break;
            case 3: blockNumsToHide = new List<int> { 1, 2 }; break;
            case 4: blockNumsToHide = new List<int> { 2, 4 }; break;
            case 5: blockNumsToHide = new List<int> { 3, 4 }; break;
            case 6: blockNumsToHide = new List<int> { 1, 2, 3 }; break;
            case 7: blockNumsToHide = new List<int> { 1, 2, 4 }; break;
        }

        if (blockNumsToHide != null)
            foreach (int i in blockNumsToHide) {
                blocks[i - 1].gameObject.SetActive(false);
                blocksCount--;
            }
    }
}