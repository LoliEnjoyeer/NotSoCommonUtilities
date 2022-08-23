using Synapse.Config;
using System.ComponentModel;

namespace CommonUtilities
{
    public class PluginConfig : AbstractConfigSection
    {
        [Description("Is plugin enabled?")]
        public bool isEnabled = true;

        [Description("Is Automatic nuke enabled?")]
        public bool isNuke = true;

        [Description("Time after which nuke is activated automatically in seconds")]
        public int NukeActivationDelay = 900;

        [Description("Cassie message before starting nuke detonation")]
        public string CassieNukeMessage = "cassie_sl pitch_.2 .g4 .g4 pitch_1 . Facility diagnostic anomaly detected . .g2 o 5 password accepted .g3 . automatic warhead detonation sequence authorized pitch_.9 .g3 . pitch_1 detonation tminus 5 minutes . all personnel evacuate pitch_.8 . .g1 . .g1 . .g1 pitch_1 bell_end";

        [Description("Delay between Cassie message and starting nuke detonation in seconds")]
        public int CassieDelay = 15;

        [Description("Are blackouts enabled?")]
        public bool isBlackout = true;

        [Description("First blackout delay in seconds")]
        public int blackoutDelay = 70;

        [Description("Cassie message before first blackout")]
        public string CassieBlackout = "pitch_.2 .g4 .g4 pitch_.9 warning pitch_1 unauthorized chaos insurgent USBdrive access jam_050_03 detected . jam_083_3 broadcasting pitch_.8 new pitch_1 jam_10_3 message .g1 . . .g1 . . .g1 . pitch_2 . . .g2 pitch_.2 .g4 .g5 pitch_.9 Warning pitch_1 .g2 facility generator system critical error . please stand by . all science personnel please pitch_.8 die pitch_.1 .g3 pitch_1 .g1 . . .g1 . . .g1 . bell_end";

        [Description("Min time between blackouts")]
        public int minBlackout = 30;

        [Description("Max time between blackouts")]
        public int maxBlackout = 40;

        [Description("Min time of blackouts")]
        public int minDurationBlackout = 20;

        [Description("Max time of blackouts")]
        public int maxDurationBlackout = 40;

        [Description("Flicker duration (preferably lower than 1 to imitate flicker effect)")]
        public float flickerDuration = 0.5f;

        [Description("Is blocking doors with coin enabled?")]
        public bool isCoin = true;

        [Description("Time which doors should be blocked for in seconds")]
        public int coinBlockTime = 8;
    }
}
