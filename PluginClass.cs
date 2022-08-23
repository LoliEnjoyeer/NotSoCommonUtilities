using Synapse.Api.Plugin;

namespace CommonUtilities
{
    [PluginInformation(
        Author = "LoliEnjoyeer",
        Description = "Adds features to spice up the gameplay",
        LoadPriority = 0,
        Name = "Common Utilities",
        SynapseMajor = 2,
        SynapseMinor = 10,
        SynapsePatch = 0,
        Version = "0.0.1"
        )]
    public class PluginClass : AbstractPlugin
    {
        [Config(section = "CommonUtilities")]
        public static PluginConfig Config;

        public override void Load()
        {
            new EventHandlers();
        }
    }
}
