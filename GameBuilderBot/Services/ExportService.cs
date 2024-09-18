using System;
using Discord.Commands;
using GameBuilderBot.Models;

namespace GameBuilderBot.Services
{
    public class ExportService
    {
        private readonly IServiceProvider _service;

        public ExportService(IServiceProvider services)
        {
            _service = services;
        }

        public void ExportGameState(GameState state, ICommandContext discordContext)
        {
            new GameStateExporterJsonFile(_service).SaveGameState(state, discordContext);
        }
    }
}
