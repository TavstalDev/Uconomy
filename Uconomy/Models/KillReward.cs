using System;

namespace fr34kyn01535.Uconomy.Models
{
    [Serializable]
    public class KillReward
    {
        public bool Enable { get; set; }
        public string EventName { get; set; }
        public decimal Reward { get; set; }

        public KillReward(string eventName, decimal reward, bool enable) { EventName = eventName; Reward = reward; Enable = enable; }

        public KillReward() { }
    }
}
