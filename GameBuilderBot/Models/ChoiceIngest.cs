namespace GameBuilderBot.Models
{
    /// <summary>
    /// Contains the values present in Choice sections of GameFile.
    /// 
    /// </summary>
    /// <seealso cref="OutcomeIngest"/> <seealso cref="GameFile"/><seealso cref="Choice"/>

    public class ChoiceIngest
    {
        /// <summary>
        /// <seealso cref="Choice"/>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// <seealso cref="Choice"/>
        /// </summary>
        public string Distribution { get; set; }

        /// <summary>
        /// <seealso cref="Choice"/>
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// <seealso cref="Choice"/>
        /// </summary>
        public bool IsPrimary { get; set; }

        /// <summary>
        /// <seealso cref="Choice"/>
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// <seealso cref="Choice"/>
        /// </summary>
        public OutcomeIngest[] Outcomes { get; set; }
    }
}
