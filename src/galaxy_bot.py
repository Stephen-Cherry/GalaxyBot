"""
GalaxyBot - A Discord bot with various functionalities.

This module sets up and starts the bot, loading cogs dynamically from the "cogs" directory.
It supports two environments: production and development, 
which can be switched using command-line arguments.

Main Components:
- main: The main function that initializes and starts the bot.
- load_cogs: Loads all cogs from the specified directory.
- on_ready: An event handler for when the bot is ready and connected.
"""

import os
import sys
import discord
from dotenv import load_dotenv


def main():
    """The main function to start the bot."""
    load_dotenv()

    intents = discord.Intents.all()

    os.environ["is_production"] = (
        "true"
        if any(arg.lower() in ["--production", "-p"] for arg in sys.argv)
        else "false"
    )
    is_production = os.environ["is_production"] == "true"
    token = os.environ.get("PRODUCTION_TOKEN" if is_production else "DEVELOPMENT_TOKEN")

    if token is None:
        raise ValueError("Token can't be empty")

    bot = discord.Bot(intents=intents)

    @bot.event
    async def on_ready():
        """Bot OnReady Event."""
        print(f"We have logged in as {bot.user}")

    load_cogs(bot)
    bot.run(token)


def load_cogs(bot, cogs_dir="cogs"):
    """Load all cogs from the specified directory."""
    main_script_dir = os.path.dirname(os.path.abspath(__file__))
    cogs_abs_path = os.path.join(main_script_dir, cogs_dir)

    for file_name in os.listdir(cogs_abs_path):
        name, ext = os.path.splitext(file_name)
        if ext == ".py" and name != "__init__":
            bot.load_extension(f"{cogs_dir}.{name}")


if __name__ == "__main__":
    main()
