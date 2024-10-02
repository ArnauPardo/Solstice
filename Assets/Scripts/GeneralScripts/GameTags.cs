using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTags : MonoBehaviour
{     
    //Player Animations
    public static string playerMoveAnim = "moveAnim";
    public static string playerJumpAnim = "jumpAnim";
    public static string playerJumpAttackAnim = "jumpAttackAnim"; 
    public static string playerAttackAnim = "attackAnim";
    public static string playerTakeDamageAnim = "takeDamageAnim";
    public static string playerDeath = "deathAnim";

    //Simple Enemy Animations
    public static string seMovement = "isMoving";
    public static string seTakeDmg = "takeDmg";
    public static string seDeath = "death";

    //Boss Animations
    public static string isActive = "isActive";
    public static string isEnraged = "isEnraged";
    public static string distanciaJugador = "distanciaJugador";
    public static string habilityCast = "HabilityCast";
    public static string takeDamage = "TakeDamage";

    //PlayerPrefs en partida
    public static string currentHealth = "CurrentHealth";
    public static string currentMoney = "CurrentMoney";
    public static string lastScene = "LastScene";

    //PlayerPrefs de guardado
    public static string savedHealth = "SavedHealth";
    public static string savedMoney = "SavedMoney";
    public static string savedScene = "SavedScene";
    public static string playerXPosition = "XPosition";
    public static string playerYPosition = "YPosition";
    public static string playerZPosition = "ZPosition";

    //PlayerPrefs de opciones
    public static string resolucionIndex = "ResolucionIndex";
    public static string calidadIndex = "CalidadIndex";
    public static string pantallaCompleta = "PantallaCompleta";

    //PlayerPrefs de guardado general de la partida
    public static string maxHearths = "MaxHearths";
    public static string firstBossDefeated = "FirstBossWon";

    //Tags
    public static string ground = "Ground";
    public static string player = "Player";
    public const string simpleEnemy = "SimpleEnemy";
    public const string ghostEnemy = "GhostEnemy";
    public const string boss = "Boss";
    public const string weaknessPoint = "WeaknessPoint";
    public static string lifeItem = "Life";
    public static string puntuacionText = "Puntuacion";
    public static string coinFloat = "Coin";
    public static string transicion = "Transicion";
    public static string fallingIntoVoid = "FallingIntoVoid";
    public static string checkpoint = "Checkpoint";
    public static string textoPartidaGuardada = "textoPartidaGuardada";
    public static string audio = "Audio";

    //Scenes
    public static string changingScene = "ChangingScene";
    public static string menu = "Menu";
    public static string intro = "Intro";
    public static string scene01 = "B1_Scene_01";
    public static string scene03 = "B1_Scene_03";
    public static string scene04 = "B1_Scene_04";
    public static string scene05 = "B1_Scene_05_01";
    public static string scene06 = "B1_Scene_06";
    public static string gameOverScene = "GameOver";
    public static string credits = "Ending";

    //Inputs
    public static string horizontalMove = "Horizontal";
}
