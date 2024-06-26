using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {
    // ����� ������ ��� ��������������� ������ � ����

    private GameController game;

    // ����� ��������� ������ � ������
    public enum ClipName {
        MenuCursor,
        MenuSelect,
        LevelStart,
        Shoot,
        Bonus,
        BonusTake,
        ExplodeEnemy,
        ExplodePlayer,
        HitArmor,
        HitConcrete,
        HitPowerful,
        OneUp,
        GameOver,
        Win,
        Engine,
        Move
    }

    public Dictionary<ClipName, AudioClip> aclips = new Dictionary<ClipName, AudioClip>();

    private GameObject startLevelMusic = null;
    private GameObject winMusic = null;

    private GameObject engineSound = null;
    private GameObject player1MoveSound = null;
    private GameObject player2MoveSound = null;
    private bool player1moved = false;
    private bool player2moved = false;

    private bool playersMoveSoundVolumeHigh = false;

    private void Start() {
        game = GameObject.FindObjectOfType(typeof(GameController)) as GameController;

        // ��������� �����
        aclips.Add(ClipName.MenuCursor, Resources.Load<AudioClip>("Audio/menu_cursor"));
        aclips.Add(ClipName.MenuSelect, Resources.Load<AudioClip>("Audio/menu_select"));
        aclips.Add(ClipName.LevelStart, Resources.Load<AudioClip>("Audio/startlevel"));
        aclips.Add(ClipName.Shoot, Resources.Load<AudioClip>("Audio/shoot"));
        aclips.Add(ClipName.Bonus, Resources.Load<AudioClip>("Audio/bonus"));
        aclips.Add(ClipName.BonusTake, Resources.Load<AudioClip>("Audio/bonustake"));
        aclips.Add(ClipName.ExplodeEnemy, Resources.Load<AudioClip>("Audio/explode_enemy"));
        aclips.Add(ClipName.ExplodePlayer, Resources.Load<AudioClip>("Audio/explode_player"));
        aclips.Add(ClipName.HitArmor, Resources.Load<AudioClip>("Audio/hit_armor"));
        aclips.Add(ClipName.HitConcrete, Resources.Load<AudioClip>("Audio/hit_concrete"));
        aclips.Add(ClipName.HitPowerful, Resources.Load<AudioClip>("Audio/hit_powerful"));
        aclips.Add(ClipName.OneUp, Resources.Load<AudioClip>("Audio/1up"));
        aclips.Add(ClipName.GameOver, Resources.Load<AudioClip>("Audio/gameover"));
        aclips.Add(ClipName.Win, Resources.Load<AudioClip>("Audio/win"));
        aclips.Add(ClipName.Engine, Resources.Load<AudioClip>("Audio/engine"));
        aclips.Add(ClipName.Move, Resources.Load<AudioClip>("Audio/move"));

        // �������� ���������, ������� ����� �������� �� ���� ������������ �������
        CreatePlayersMoveSoundSources();
    }

    private void FixedUpdate() {
        // ����� ������������� ����������� �������� ������, ����������� ���������
        if(startLevelMusic == null && !playersMoveSoundVolumeHigh) {
            player1MoveSound.GetComponent<AudioSource>().volume = 0.10f;
            player2MoveSound.GetComponent<AudioSource>().volume = 0.10f;
            playersMoveSoundVolumeHigh = true;
        }

        // ���� �� ������ ����������� �������� � ��� �� ������� ������� ���� ����������
        if (startLevelMusic == null && engineSound == null && game.isPlayEngine())
            PlayEngineSound();
        else if (engineSound != null && !game.isPlayEngine()) // ���� ���� ������� �� �������������� ���� ����������, �� ���������� ������
            Destroy(engineSound, 0f);

        EngineSoundControl(); // ��������� ��������� ����� �������� ����� ����������, ����� �� ������ ������������
    }

    public void PlaySound(ClipName cName) {
        // ����� ��� ��������������� �����

        AudioClip playClip = aclips[cName]; // �������������� ���� �� ������

        // ������ ������, �������� ����
        GameObject SoundGameObject = new GameObject("Sound");
        AudioSource SoundAudioSource = SoundGameObject.AddComponent<AudioSource>();
        SoundAudioSource.clip = playClip;
        SoundAudioSource.volume = startLevelMusic == null ? 0.3f : 0.1f;
        SoundAudioSource.Play();
        Destroy(SoundGameObject, playClip.length); // ���������� ������, ��� ������ ���������� ����

        // ���� ��������������� �������� ������, �� ���������� �����������
        if (cName == ClipName.LevelStart) {
            startLevelMusic = SoundGameObject; // ��������� ������ � ����������, ����� �������������� ������ ���� ��� ���
            player1MoveSound.GetComponent<AudioSource>().volume = 0.05f;
            player2MoveSound.GetComponent<AudioSource>().volume = 0.05f;
            playersMoveSoundVolumeHigh = false;
        }
    }

    private void PlayEngineSound() {
        // ���� ����������

        AudioClip playClip = aclips[ClipName.Engine];

        GameObject EngineSoundGameObject = new GameObject("EngineSound");
        AudioSource EngineSoundAudioSource = EngineSoundGameObject.AddComponent<AudioSource>();
        EngineSoundAudioSource.clip = playClip;
        EngineSoundAudioSource.volume = 0.10f;
        EngineSoundAudioSource.loop = true;
        EngineSoundAudioSource.Play();

        engineSound = EngineSoundGameObject;
    }

    private void CreatePlayersMoveSoundSources() {
        // ����� �������� ��������, ��������������� ���� �������� �������

        AudioClip playClip = aclips[ClipName.Move];

        GameObject MoveSoundGameObject = new GameObject("Player1MoveSound");
        AudioSource MoveSoundAudioSource = MoveSoundGameObject.AddComponent<AudioSource>();
        MoveSoundAudioSource.clip = playClip;
        MoveSoundAudioSource.volume = 0.5f;
        MoveSoundAudioSource.loop = true;

        player1MoveSound = MoveSoundGameObject;

        MoveSoundGameObject = new GameObject("Player2MoveSound");
        MoveSoundAudioSource = MoveSoundGameObject.AddComponent<AudioSource>();
        MoveSoundAudioSource.clip = playClip;
        MoveSoundAudioSource.volume = 0.5f;
        MoveSoundAudioSource.loop = true;

        player2MoveSound = MoveSoundGameObject;
    }

    public void PlayerMovePlaySound(int player) {
        if (player == 1 && !player1MoveSound.GetComponent<AudioSource>().isPlaying) { player1MoveSound.GetComponent<AudioSource>().Play(); player1moved = true; } 
        else
        if (player == 2 && !player2MoveSound.GetComponent<AudioSource>().isPlaying) { player2MoveSound.GetComponent<AudioSource>().Play(); player2moved = true; }
    }

    public void PlayerMoveStopSound(int player) {
        if (player == 1 && player1MoveSound.GetComponent<AudioSource>().isPlaying) { player1MoveSound.GetComponent<AudioSource>().Stop(); player1moved = false; }
        else
        if (player == 2 && player2MoveSound.GetComponent<AudioSource>().isPlaying) { player2MoveSound.GetComponent<AudioSource>().Stop(); player2moved = false; }
    }

    private void EngineSoundControl() {
        // ������������ ����� ������ ���� ���� ���������, ���� ���� ��������.

        if(engineSound) {
            if(engineSound.GetComponent<AudioSource>().isPlaying) {
                if (player1moved || player2moved) engineSound.GetComponent<AudioSource>().Pause();
            } else {
                if (!player1moved && !player2moved) engineSound.GetComponent<AudioSource>().Play();
            }
        }
    }
}