using Platformer.Core;
using Platformer.Mechanics;
using Platformer.Model;
using UnityEditor;
using UnityEngine;

namespace Platformer.Gameplay
{

    /// <summary>
    /// This event is triggered when the player character enters a trigger with a VictoryZone component.
    /// </summary>
    /// <typeparam name="PlayerEnteredVictoryZone"></typeparam>
    public class PlayerEnteredVictoryZone : Simulation.Event<PlayerEnteredVictoryZone>
    {
        public VictoryZone victoryZone;

        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        public override void Execute()
        {
            model.player.animator.SetTrigger("victory");
            model.player.controlEnabled = false;
            var finalScore = RealmController.Instance.playerWon();

            var didClickRestart = EditorUtility.DisplayDialog("You won!", "Score: " + finalScore, "restart game", "cancel");


            if (didClickRestart == true)
            {
                Simulation.Schedule<PlayerSpawn>(2);
            }
        }
    }
}