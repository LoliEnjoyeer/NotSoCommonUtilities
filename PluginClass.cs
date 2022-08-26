using Synapse.Api.Plugin;

namespace CommonUtilities
{
    [PluginInformation(
        Author = "LoliEnjoyeer",
        Description = "Adds features to spice up the gameplay",
        LoadPriority = 0,
        Name = "Not So Common Utilities",
        SynapseMajor = 2,
        SynapseMinor = 10,
        SynapsePatch = 0,
        Version = "1.0.0"
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
