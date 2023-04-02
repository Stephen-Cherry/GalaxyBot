# GalaxyBotv2

GalaxyBotv2 is a Discord bot with various functionalities. It's designed to be versatile and dynamically loads "cogs" (modules) to extend its capabilities. The bot can operate in two environments: production and development, which can be switched using environmental variables.

## Features

- Dynamic loading of cogs from the "cogs" directory
- Support for production and development environments
- Easy configuration using environment variables

## Setup

1. Clone the repository to your local machine.
2. Install the required dependencies by running `pip install -r requirements.txt`.
3. Create a `.env` file in the root directory of the project with the following variables:
- IS_PRODUCTION=true_or_false
- PRODUCTION_TOKEN=your_production_bot_token
- DEVELOPMENT_TOKEN=your_development_bot_token
4. Replace `true_or_false` with `true` for the production environment or `false` for the development environment. Replace `your_production_bot_token` and `your_development_bot_token` with your respective Discord bot tokens.
5. Run the bot by executing `python galaxy_bot.py`.

## Adding Cogs

To add new cogs, create a new Python file in the "cogs" directory. The file should define a class that inherits from `discord.ext.commands.Cog`. The bot will automatically load the cog when it starts.

## Contributing

If you'd like to contribute to the project or report any issues, please open an issue or submit a pull request on the GitHub repository.

## License

This project is released under the [MIT License](LICENSE).
