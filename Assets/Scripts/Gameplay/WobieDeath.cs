using Platformer.Core;
using Platformer.Mechanics;

namespace Platformer.Gameplay
{
    /// <summary>
    /// Fired when the health component on an enemy has a hitpoint value of  0.
    /// </summary>
    /// <typeparam name="WobieDeath"></typeparam>
    public class WobieDeath : Simulation.Event<WobieDeath>
    {
        public WobieController enemy;

        public override void Execute()
        {
            enemy._collider.enabled = false;
            if (enemy._audio && enemy.ouch)
                enemy._audio.PlayOneShot(enemy.ouch);
            enemy.gameObject.SetActive(false);
        }
    }
}