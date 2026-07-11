using System;
using SDG.Unturned;

namespace fr34kyn01535.Uconomy.Models
{
    /// <summary>
    /// Represents a monetary reward configuration
    /// for a specific kill event type.
    /// </summary>
    [Serializable]
    public class KillReward
    {
        /// <summary>
        /// Whether this reward is currently enabled.
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// The kill event that triggers this reward.
        /// </summary>
        public EPlayerStat Event { get; set; }

        /// <summary>
        /// The amount added to the killer's balance on a successful kill.
        /// </summary>
        public decimal Reward { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="KillReward"/> class.
        /// </summary>
        /// <param name="event">The kill event that triggers this reward.</param>
        /// <param name="reward">The amount added to the killer's balance on a successful kill.</param>
        /// <param name="enable">Whether this reward is currently enabled.</param>
        public KillReward(EPlayerStat @event, decimal reward, bool enable) { Event = @event; Reward = reward; Enable = enable; }

        /// <summary>Initializes a new empty instance of the <see cref="KillReward"/> class.</summary>
        public KillReward() { }
    }
}
