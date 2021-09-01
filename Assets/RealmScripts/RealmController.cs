using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Realms;
using UnityEngine.UI;
using MongoDB.Bson;
using System.Threading.Tasks;
using Realms.Sync;
using System.Linq;

// realm controller opens the realm initially + inserts new player + new scores
public class RealmController : MonoBehaviour
{
    private static Realm realm;
    private static App realmApp;
    public static ScoreModel score;
    public static Text scoreCard;
    public static RealmController Instance;
    private static int runTime; // lower run time rewards speedrunners with bonus points
    private static int bonusPoints = 0; // start with 0 bonus points and at the end of the game we add bonus points based on how long you played

    public static Realms.Sync.User user;
    public static PlayerModel realmPlayer;

    // sync variables:
    public static string email;


    public Realms.Sync.User getUser()
    {
        return user;
    }

    // when app.EmailPasswordAuth.RegisterUserAsync is called, then a new player should be created
    public static async Task<PlayerModel> registerPlayer(string requestEmail, string requestPassword)
    {
     
        realmApp = App.Create("unity-realm-tutorial-vxtjl");
        await realmApp.EmailPasswordAuth.RegisterUserAsync(requestEmail, requestPassword);
        var newUser = await realmApp.LogInAsync(Credentials.EmailPassword(requestEmail, requestPassword));
        var syncConfiguration = new SyncConfiguration("UnityTutorialPartition", newUser);
        var realm = await Realm.GetInstanceAsync(syncConfiguration);
        
        
        var p1 = new PlayerModel();
        p1.Id = newUser.Id;
        p1.Name = requestEmail;
        p1.Partition = "UnityTutorialPartition";

        var s1 = new ScoreModel();
        s1.ScoreOwner = p1;
        s1.Points = 0;
        s1.EnemiesDefeated = 0;
        s1.TokensCollected = 0;


        realm.Write(() =>
        {
            realmPlayer = realm.Add(p1);
            score = realm.Add(s1);
            realmPlayer.Scores.Add(score);
        });
        return realmPlayer;
        //return new PlayerModel();
    }
    public static async Task<Realms.Sync.User> logIn(string requestEmail, string requestPassword)
    {
        realmApp = App.Create("unity-realm-tutorial-vxtjl");
        var loggedInUser = await realmApp.LogInAsync(Credentials.EmailPassword(requestEmail, requestPassword));
        return loggedInUser;
    }

    public static async Task<Realm> CreateRealmAsync(Realms.Sync.User loggedInUser) // Realms.Sync.User
    {
        var syncConfiguration = new SyncConfiguration("UnityTutorialPartition", loggedInUser);
        return await Realm.GetInstanceAsync(syncConfiguration);
    }

    public static async void onPressLogin()
    {
        var tempEmail = "petunia.pometoun4@example.com";
        var tempPass = "petunia";
        user = await logIn(tempEmail, tempPass);
        if (user != null)
        {
            realm = await CreateRealmAsync(user);
            realmPlayer = realm.Find<PlayerModel>(user.Id);

            if (realmPlayer != null)
            {
                var s1 = new ScoreModel();
                s1.ScoreOwner = realmPlayer;
                s1.Points = 0;
                s1.EnemiesDefeated = 0;
                s1.TokensCollected = 0;
                s1.ScoreOwner = realmPlayer;

                realm.Write(() =>
                {
                    score = realm.Add(s1);
                    realmPlayer.Scores.Add(score);
                });

                ScoreCardManager(); // set the initial scores


                // record each 10 seconds (runTime will be used to calculate bonus points once the player wins the game)
                var myTimer = new System.Timers.Timer(10000);
                myTimer.Enabled = true;
                myTimer.Elapsed += (sender, e) => runTime += 10;

                LeaderboardManager.Instance.setLoggedInUser(user);




            }
        }
    }



    // doesn't need to be async for non sync
    void Awake()
    {
        Instance = this;

        //var createAndGetPlayer = await registerPlayer(tempEmail, tempPass);
        //Debug.Log("do player exists?");
        //Debug.Log(createAndGetPlayer);


        //user = await logIn(tempEmail, tempPass);

        //Debug.Log("do player exists?");
        //Debug.Log(user);

        onPressLogin();

        //if(user != null)
        //{
        //    Debug.Log("login success!");
        //    realm = await CreateRealmAsync(user);
        //    realmPlayer = realm.Find<PlayerModel>(user.Id);

        //    if (realmPlayer != null)
        //    {
        //        var s1 = new ScoreModel();
        //        s1.ScoreOwner = realmPlayer;
        //        s1.Points = 0;
        //        s1.EnemiesDefeated = 0;
        //        s1.TokensCollected = 0;
        //        s1.ScoreOwner = realmPlayer;

        //        realm.Write(() =>
        //        {
        //            score = realm.Add(s1);
        //            realmPlayer.Scores.Add(score);
        //        });

        //        ScoreCardManager(); // set the initial scores


        //        // record each 10 seconds (runTime will be used to calculate bonus points once the player wins the game)
        //        var myTimer = new System.Timers.Timer(10000);
        //        myTimer.Enabled = true;
        //        myTimer.Elapsed += (sender, e) => runTime += 10; 
        //    }
        //}
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

    public static int calculatePoints()
    {
        var currentPoints = (score.EnemiesDefeated + 1) * (score.TokensCollected + 1) + bonusPoints;
        return currentPoints;
    }

    public static void ScoreCardManager()
    {
        scoreCard = GameObject.Find("ScoreCard").GetComponent<Text>();
        scoreCard.fontStyle = FontStyle.Bold;
        scoreCard.text = realmPlayer.Name + "\n" +
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
        scoreCard.text = realmPlayer.Name + "\n" +
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
        //realm.Dispose();
    }

    // :code-block-start: realm-controller
    // :state-start: final
    // >>>> HERE
    // :state-end: :state-uncomment-start: start
    //// TODO: implement controller
    // :state-uncomment-end:
    // :code-block-end:

}
