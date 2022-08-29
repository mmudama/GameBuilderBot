# GameBuilderBot
Discord bot to facilitate playing games that involve lots of dice rolls.

# To Run
<work in progress>

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
TODO: more examples

Example file: https://github.com/bounceswoosh/GameBuilderBot/blob/master/GameBuilderBot/Examples/ExampleGame.yaml


