using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace GameBuilderBot.Models
{
    /// <summary>
    /// Files that represent individual games (<seealso cref="GameDefinition"/>s)are deserialized into this object
    /// see the Examples directory for game definitions loaded form YAML.
    /// </summary>
    public class GameFile
    {
        /// <summary>
        /// <seealso cref="GameDefinition"/>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// <seealso cref="GameEvent"/>
        /// </summary>
        [YamlMember(Alias = "Events")]
        public GameEventIngest[] GameEvents { get; set; }

        /// <summary>
        /// <seealso cref="Field"/>
        /// </summary>
        public Dictionary<string, FieldIngest> Fields { get; set; }

        /// <summary>
        /// Default constructor required for deserialization
        /// </summary>
        public GameFile() { }
    }
}
