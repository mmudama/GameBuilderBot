namespace GameBuilderBot.Models
{
    /// <summary>
    /// Contains the values present in GameEvent sections of GameFile.
    /// 
    /// </summary>
    /// <seealso cref="OutcomeIngest"/> <seealso cref="GameFile"/><seealso cref="GameEvent"/>

    public class GameEventIngest
    {
        /// <summary>
        /// <seealso cref="GameEvent"/>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// <seealso cref="GameEvent"/>
        /// </summary>
        public string Distribution { get; set; }

        /// <summary>
        /// <seealso cref="GameEvent"/>
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// <seealso cref="GameEvent"/>
        /// </summary>
        public bool IsPrimary { get; set; }

        /// <summary>
        /// <seealso cref="GameEvent"/>
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// <seealso cref="GameEvent"/>
        /// </summary>
        public OutcomeIngest[] Outcomes { get; set; }
    }
}
