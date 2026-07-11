using SDG.Unturned;
using System;

namespace fr34kyn01535.Uconomy.Models
{
    /// <summary>
    /// Represents a monetary penalty configuration
    /// for a specific cause of player death.
    /// </summary>
    [Serializable]
    public class DeathPenalty
    {
        /// <summary>
        /// The type of death that triggers this penalty.
        /// </summary>
        public EDeathCause Cause { get; set; }

        /// <summary>
        /// Whether this penalty is currently enabled.
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// The amount deducted from the player's balance on death.
        /// </summary>
        public decimal Fine { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeathPenalty"/> class.
        /// </summary>
        /// <param name="cause">The type of death that triggers this penalty.</param>
        /// <param name="enable">Whether this penalty is currently enabled.</param>
        /// <param name="fine">The amount deducted from the player's balance on death.</param>
        public DeathPenalty(EDeathCause cause, bool enable, decimal fine) { Cause = cause; Enable = enable; Fine = fine; }

        /// <summary>
        /// Initializes a new empty instance of the <see cref="DeathPenalty"/> class.
        /// </summary>
        public DeathPenalty() { }
    }
}
