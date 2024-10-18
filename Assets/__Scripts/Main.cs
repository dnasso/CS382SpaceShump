using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Main : MonoBehaviour
{
    static private Main S;  // A private singleton for Main

    [Header("Inscribed")]
    public GameObject[] prefabEnemies;  // An array of enemy prefabs
    public Text         uitLevel;
    public Text         uitScore;
    public float        enemySpawnPerSecond = 0.5f; // # Enemies spawned/second
    public float        enemyInsetDefault = 1.5f;   // Inset from the sides
    public float        gameRestartDelay = 2;

    private BoundsCheck bndCheck;
    private int         enemiesDead = 0;
    private int         level = 0;
    private int         levelMax = 5;
    private int         score = 0;
    
    // I made this into an array, and I hope to increment it as the level goes on.
    private float[]     enemySpawnPerSecondArray = {.25f, 0.5f, 1.0f, 2.0f, 4.0f} ; // # Enemies spawned/second

    void Awake() {
        S = this;
        // Set bndCheck to reference the BoundsCheck component on this
        // GameObject
        bndCheck = GetComponent<BoundsCheck>();
        enemySpawnPerSecond = enemySpawnPerSecondArray[level];

        // Invoke SpawnEnemy() once (in 2 seconds, based on default values)
        Invoke( nameof(SpawnEnemy), 1f/enemySpawnPerSecond );
    }

    public void SpawnEnemy() {
        // Pick a random Enemy prefab to instantiate
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>( prefabEnemies[ ndx ] );

        // Position the Enemy above the screen with a random x postion
        float enemyInset = enemyInsetDefault;
        if (go.GetComponent<BoundsCheck>() != null) {
            enemyInset = Mathf.Abs( go.GetComponent<BoundsCheck>().radius );
        }

        // Set the initial position for the spawned Enemy
        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyInset;
        float xMax = bndCheck.camWidth - enemyInset;
        pos.x = Random.Range( xMin, xMax );
        pos.y = bndCheck.camHeight + enemyInset;
        go.transform.position = pos;

        // Invoke SpawnEnemy() again
        Invoke( nameof(SpawnEnemy), 1f/enemySpawnPerSecond );
    }

    void DelayedRestart() {
        // Invoke the Restart() method in gameRestartDelay seconds
        Invoke( nameof(Restart), gameRestartDelay );
    }

    void Restart() {
        // Reload __Scene_0 to restart the game
        // "__Scene_0" below starts with 2 underscores and ends with a zero.
        SceneManager.LoadScene( "__Scene_0" );
    }

    void UpdateGUI() {
        uitLevel.text = "Level: "+(level+1)+" of "+levelMax;
        uitScore.text = "Score: "+score;
    }

    void UpdateSpawnRate() {
        enemySpawnPerSecond = enemySpawnPerSecondArray[level];
        Debug.Log(enemySpawnPerSecond);
    }

    void Update() {
        UpdateGUI();
        if ((level+1) == levelMax) {
            return;
        }
        if (enemiesDead == 10*(level+1) ) {
            level++;
            UpdateSpawnRate();
        }
    }

    static public void HERO_DIED() {
        S.DelayedRestart();
    }

    static public void ENEMY_DIED() {
        S.enemiesDead = S.enemiesDead + 1;
        S.score = S.score + 100;
    }
}
