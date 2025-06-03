# GameBuilderBot
This Discord bot is intended to blend a GM-run table top RPG with an "Oregon Trail"-style set of possibilities. 
I wrote it because we had a limited number of GMs, and they wanted to be able to play their characters more often. 
This bot allows the GM to be a story teller while externalizing the encounters so that the GMs can play their characters. 
In fact, it doesn't require any one person to define possible encounters - all players could add their own content. 
However, the bot is limited; you'll still need someone to run any non-trivial campaign.

# Capabilities
* Assign integer or roll-based expressions to variables
* Define events that have weighted outcomes (40% chance of a paper cut; 50% chance of a puncture wound; 10% chance of fatality)
* Define events in which all outcomes occur (They tied your shoelaces together AND stole your satchel AND ran around the corner)
* Define roll-based values within the events (There are {1d12} gnolls in your way, guarding {1d3} hostages)
* Define an event that calls other events (that can then call other events, etc)


# To Run

## Environment Variable
* `GBB_CONFIG_FILE` - specifies the full path of your config file.

## Config File Format
TODO provide example

The config file is Yaml format and requires:
* `DiscordBotToken` - see below. This is how your bot gets access to Discord
* `GameDefinitionDirectory` - This is where the possible moves for your games are defined.
* `GameStateDirectory` - This is where the bot stores the current state of your game, allowing you to continue playing it later
* `ValidCommandPrefixes` (list) - The bot will only recognize commands with these characters as a prefix. Common choices are `!` and `;`.

## Process to get your bot token:
https://www.writebots.com/discord-bot-token/

## Game Definition Format
TODO: better example that demonstrates all capabilities

Example file: https://github.com/mmudama/GameBuilderBot/blob/master/GameBuilderBot/Examples/ExampleGame.yaml


