using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Realms;
using UnityEngine.UI;
using MongoDB.Bson;

// realm controller opens the realm initially + inserts new player + new scores
public class RealmController : MonoBehaviour
{
    private Realm realm;
    public static PlayerModel player;
    public static ScoreModel score;
    public Text scoreCard;
    public static RealmController Instance;
    private static int runTime; // lower run time rewards speedrunners with bonus points
    private int bonusPoints = 0; // start with 0 bonus points and at the end of the game we add bonus points based on how long you played


    void Awake()
    {
        Instance = this;

        if (realm != null)
        {
            realm.Dispose();
        }
        //var config = new RealmConfiguration(default);
        //Realm.DeleteRealm(config);


        realm = Realm.GetInstance();
        player = realm.Find<PlayerModel>("OfflinePlayer1");


        if(player == null)
        {
            var p1 = new PlayerModel();
            p1.name = "OfflinePlayer1";

            var s1 = new ScoreModel(p1, 0, 0, 0);
            s1.ScoreOwner = p1;

            realm.Write(() =>
            {
                player = realm.Add(p1);
                score = realm.Add(s1);
                player.Scores.Add(score);
            });
        }
        else
        {
            var s1 = new ScoreModel(player, 0, 0, 0);
            realm.Write(() =>
            {
                score = realm.Add(s1);
                player.Scores.Add(score);
            });
        }

        ScoreCardManager(); // set the initial scores


        var myTimer = new System.Timers.Timer(10000); // every ten seconds
        // Hook up the Elapsed event for the timer.
        myTimer.Enabled = true;
        myTimer.Elapsed += (sender, e) => runTime += 10; // +10 seconds


    }
    public void collectToken() // performs an update on the Character Model's token count
    {
        realm.Write(() =>
        {
            score.TokensCollected += 1;
        });
        ScoreCardManager();
    }
    public void defeatEnemy() // performs an update on the Character Model's enemiesDefeated Count
    {
        realm.Write(() =>
        {
            score.EnemiesDefeated += 1;
        });
        ScoreCardManager();
    }

    public int calculatePoints()
    {
        var currentPoints = (score.EnemiesDefeated + 1) * (score.TokensCollected + 1) + bonusPoints;
        return currentPoints;
    }

    void ScoreCardManager()
    {
        scoreCard = GameObject.Find("ScoreCard").GetComponent<Text>();
        scoreCard.fontStyle = FontStyle.Bold;
        scoreCard.text = player.name + "\n" +
            "Enemies Defeated: " + score.EnemiesDefeated + "\n" +
            "Tokens Collected: " + score.TokensCollected + "\n" +
            "Current Points: " + calculatePoints();
    }
    public int playerWon()
    {
        if (runTime <= 30) // if the game is beat in in less than or equal to 30 seconds, +80 bonus points
        {
            bonusPoints = 80;
        }
        else if (runTime <= 60) // if the game is beat in in less than or equal to 1 min, +70 bonus points
        {
            bonusPoints = 70;
        }
        else if (runTime <= 90) // if the game is beat in less than or equal to 1 min 30 seconds, +60 bonus points
        {
            bonusPoints = 60;
        }
        else if (runTime <= 120) // if the game is beat in less than or equal to 2 mins, +50 bonus points
        {
            bonusPoints = 50;
        }

        // calculate final points + write to realm with points
        var finalPoints = calculatePoints();
        // update scorecard
        scoreCard = GameObject.Find("ScoreCard").GetComponent<Text>();
        scoreCard.fontStyle = FontStyle.Bold;
        scoreCard.text = player.name + "\n" +
            "Enemies Defeated: " + score.EnemiesDefeated + "\n" +
            "Tokens Collected: " + score.TokensCollected + "\n" +
            "Final Points: " + finalPoints + "( +" + bonusPoints + " bonus points)";


        realm.Write(() =>
        {
            score.Points = finalPoints;
        });


        return finalPoints;
    }

    void OnDisable()
    {
        realm.Dispose();
    }

    // :code-block-start: realm-controller
    // :state-start: final
    // >>>> HERE
    // :state-end: :state-uncomment-start: start
    //// TODO: implement controller
    // :state-uncomment-end:
    // :code-block-end:

}
