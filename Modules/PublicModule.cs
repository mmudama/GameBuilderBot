using Discord.Commands;
using DiscordOregonTrail.Models;
using System;
using System.Text;
using System.Threading.Tasks;

namespace DiscordOregonTrail.Modules
{

    // Modules must be public and inherit from an IModuleBase
    public class PublicModule : ModuleBase<SocketCommandContext>
    {
        protected Random r = new Random();
        Config _config { get; set; }

        public PublicModule(Config config)
        {
            _config = config;

        }


        [Command("ping")]
        [Alias("pong", "hello")]
        public Task PingAsync()
            => ReplyAsync("pong!");

        [Command("help")]
        public Task Help()
            => ReplyAsync(HelpMe());

        private string HelpMe()
        {
            StringBuilder helpMessage = new StringBuilder();

            helpMessage.AppendLine("> !trail\n**Available arguments:**");

            helpMessage.AppendJoin('\n', _config.choiceMap.Keys);

            return helpMessage.ToString();
            
        }

        [Command("trail")]
        public Task TrailAsync(params string[] objects)
        => ReplyAsync(RollEvents(objects));

        public string RollEvents(params string[] objects)
        {
            string retval = HelpMe();

            if (objects.Length == 0) return retval;

            string choice = objects[0];

            retval = GetResponse(choice);

            return retval;
        }

        [Command("state")]
        public Task StateAsync() => ReplyAsync(Dump());

        private string Dump()
        {
            StringBuilder retval = new StringBuilder("> State:\n");

            foreach (string choice in _config.choiceMap.Keys)
            {
                retval.AppendLine(choice);
                foreach (string c in _config.choiceMap[choice].outcomeMap.Keys)
                {
                    retval.AppendFormat("\t{0}:\t{1}\n", c, _config.choiceMap[choice].outcomeMap[c].Weight);
                }

            }


            return retval.ToString();
        }


        private string GetResponse(string choice)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("> Rolling for {0}", choice));

            if (_config.choiceMap.ContainsKey(choice.ToLower()))
            {
                Choice c = _config.choiceMap[choice.ToLower()];

                if (c.Distribution.Equals("Weighted"))
                {
                    int max = c.outcomeNameList.Count;
                    int roll = r.Next(0, max);

                    Outcome o = c.outcomeMap[c.outcomeNameList[roll]];

                    string outcome = o.Text;

                    if (o.Roll > 0)
                    {
                        int count = r.Next(1, o.Roll + 1);
                        outcome = String.Format(outcome, count);
                    }

                    sb.AppendLine(string.Format("Rolled 1d{0} and got {1}: {2}", max, roll+ 1, outcome));

                    if (o._choice != null)
                    {
                        sb.AppendLine(GetResponse(o._choice.Name));
                    }
                }
            }

            if (sb.Length == 0)
            {
                sb.AppendLine("Something went wrong");
            }
            return sb.ToString();

            
        }


        // [Remainder] takes the rest of the command's arguments as one argument, rather than splitting every space
        [Command("echo")]
        public Task EchoAsync([Remainder] string text)
            // Insert a ZWSP before the text to prevent triggering other bots!
            => ReplyAsync('\u200B' + text);

        // 'params' will parse space-separated elements into a list
        [Command("list")]
        public Task ListAsync(params string[] objects)
            => ReplyAsync("You listed: " + string.Join("; ", objects));

    }



}
