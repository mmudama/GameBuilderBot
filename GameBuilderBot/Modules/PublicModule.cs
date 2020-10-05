using Discord.Commands;
using GameBuilderBot.Services;
using System.Threading.Tasks;

namespace GameBuilderBot.Modules
{
    /// <summary>
    /// This class handles commands received from a Discord channel
    /// </summary>
    public class PublicModule : ModuleBase<SocketCommandContext>
    {
        protected ResponseService _responseService;
        protected GameHandlingService _gameHandlingService;

        public PublicModule(ResponseService responseService, GameHandlingService gameHandlingService)
        {
            _responseService = responseService;
            _gameHandlingService = gameHandlingService;
        }

        /// <summary>
        /// General help for the user. Does not require a game to be in progress.
        /// </summary>
        /// <returns>An async Task to deliver a help message to the current Discord channel</returns>
        [Command("help")]
        public Task HelpAsync()
            => ReplyAsync(_responseService.HelpForUser());

        /// <summary>
        /// Allows the user to start a game
        /// </summary>
        /// <param name="inputs">If empty, responds with a list of games from which the user may choose.
        /// If there is a first argument, it is treated as an integer representing a game from the list.
        /// Subsequent arguments are ignored.</param>
        /// <returns>An async Task to deliver a response to the current Discord channel</returns>
        [Command("Start")]
        public Task StartGameAsync(params string[] inputs) => ReplyAsync(_responseService.StartGame(inputs, Context));

        /// <summary>
        /// Rolls virtual dice to determine outcomes. Possible parameter values and their behaviors
        /// are defined in GameDefinition and GameFile. This is the meat of the bot.
        /// </summary>
        /// <param name="inputs">A string representing a roll within the game OR "help" to get a list
        /// of available inputs. Only the first element of inputs is recognized</param>
        /// <returns>An async Task to deliver a response to the current Discord channel. The response
        /// will inform the caller of the outcomes according to the configuration of each Outcome.</returns>
        [Command("game")]
        [Alias("g", "gb", "gamebuilder")]
        public Task RollEventsAsync(params string[] inputs)
        => ReplyAsync(_responseService.RollEventsForUser(Context, inputs));

        /// <summary>
        /// Retrieves key/value pairs from the current game's state
        /// </summary>
        /// <param name="inputs">If inputs is empty or the first element is "all", gets all loaded 
        /// key/value pairs. Otherwise, if it has one or more elements, attempt to treat each element
        /// as a key and retrieve the value.</param>
        /// <returns>An async Task to deliver a response to the current Discord channel</returns>
        [Command("get")]
        [Alias("list")]
        public Task GetAsync(params string[] inputs) => ReplyAsync(_responseService.GetFieldValuesForUser(Context.Channel.Id, inputs));

        /// <summary>
        /// Adds a key/value pair to the game state, or updates the value if the key already exists
        /// </summary>
        /// <param name="inputs">The first element should be a key. The second element should be a value.</param>
        /// <returns>An async Task to deliver a response to the current Discord channel</returns>        
        [Command("set")]
        public Task SetAsync(params string[] inputs) => _responseService.SetFieldValueForUser(inputs, Context);

        /// <summary>
        /// Removes a key/value pair from the current game state
        /// </summary>
        /// <param name="inputs">One or more keys</param>
        /// <returns>An async Task to deliver a response to the current Discord channel</returns>
        [Command("Delete")]
        [Alias("del", "remove", "rm", "unset")]
        public Task DeleteAsync(params string[] inputs) => ReplyAsync(_responseService.DeleteFieldValueForUser(inputs, Context));

        /// <summary>
        /// Evaluates an expression as a numeric value.
        /// </summary>
        /// <param name="expression">Examples: "1d4" "#miles# / #mpg#"</param>
        /// <returns>/// <returns>An async Task to deliver a response to the current Discord channel</returns></returns>
        [Command("evaluate")]
        [Alias("eval")]
        public Task EvaluateAsync([Remainder] string expression) => ReplyAsync(_responseService.EvaluateExpressionForUser(expression, Context.Channel.Id));

        /// <summary>
        /// Adds a number to the current value for the key and sets the sum as the new value.
        /// If there is no current value, the behavior is identical
        /// to <seealso cref="SetAsync(string[])"/> above.
        /// </summary>
        /// <param name="inputs">The first element should be a key. The second element should be a value.</param>
        /// <returns>An async Task to deliver a response to the current Discord channel</returns>        
        [Command("add")]
        public Task AddAsync(params string[] inputs) => _responseService.AddFieldValueForUser(inputs, Context);

        /// <summary>
        /// Subtracts a number from the current value for the key and sets the difference as the new value.
        /// If there is no current value, the behavior is identical
        /// to <seealso cref="SetAsync(string[])"/> above.
        /// </summary>
        /// <param name="inputs">The first element should be a key. The second element should be a value.</param>
        /// <returns>An async Task to deliver a response to the current Discord channel</returns>        
        [Command("subtract")]
        public Task SubtractAsync(params string[] inputs) => _responseService.SubtractFieldValueForUser(inputs, Context);
    }
}
