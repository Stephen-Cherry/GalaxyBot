import logging
import os
from typing import List

from dotenv import load_dotenv
from interactions import Client, Intents


def main():
    load_dotenv()
    invalid_secrets: List[str] = validate_secrets()
    if invalid_secrets:
        raise ValueError(invalid_secrets)

    logging.basicConfig()
    cls_log = logging.getLogger("GalaxyLogger")
    cls_log.setLevel(logging.INFO)

    bot = Client(
        intents=Intents.GUILD_MESSAGES | Intents.GUILDS | Intents.MESSAGE_CONTENT,
        logger=cls_log,
    )

    for file in os.listdir("src/cogs"):
        if file.endswith(".py"):
            bot.load_extension(f"cogs.{file[:-3]}")
    bot.start(os.environ.get("GALAXY_TOKEN"))


def validate_secrets() -> List[str]:
    invalid_secrets: List[str] = []
    token: str | None = os.environ.get("GALAXY_TOKEN")
    if not token:
        invalid_secrets.append("GALAXY_TOKEN")


if __name__ == "__main__":
    main()
