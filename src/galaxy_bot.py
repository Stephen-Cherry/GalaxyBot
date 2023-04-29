import sys
import os
from os import path
import discord
from dotenv import load_dotenv

# Add the 'src' directory to the sys.path
sys.path.append(path.join(path.dirname(path.abspath(__file__)), ".."))


def main():
    load_dotenv()

    intents = discord.Intents.all()
    token = os.environ.get("TOKEN")

    if token is None:
        raise ValueError("Token can't be empty")
    bot = discord.Bot(intents=intents)

    @bot.event
    async def on_ready():
        print(f"We have logged in as {bot.user}")

    @bot.event
    async def on_message():
        pass

    load_cogs(bot)
    bot.run(token)


def load_cogs(bot, cogs_dir="cogs"):
    main_script_dir = os.path.dirname(os.path.abspath(__file__))
    cogs_abs_path = os.path.join(main_script_dir, cogs_dir)

    for file_name in os.listdir(cogs_abs_path):
        name, ext = os.path.splitext(file_name)
        if ext == ".py" and name != "__init__":
            bot.load_extension(f"{cogs_dir}.{name}")


if __name__ == "__main__":
    main()
