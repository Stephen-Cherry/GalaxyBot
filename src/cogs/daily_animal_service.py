"""A Discord bot module to send a random dog picture and cat picture to a 
specific channel every day using the https://random.dog/woof.json  and
https://api.thecatapi.com/v1/images/search API."""

from datetime import datetime
import os
import discord
from discord.ext import commands, tasks
import requests
from src.constants.constants import DAILY_ANIMAL_TIME

from src.models.animal_responses import Doggo, Cat


class DailyAnimalService(commands.Cog):
    """A cog that sends a random dog picture to a specified channel every
    day."""

    def __init__(self, bot: discord.Bot):
        """Initialize the DailyAnimal cog.

        :param bot: A discord.Bot instance.
        """
        self.bot = bot
        is_production = os.environ.get("IS_PRODUCTION") == "true"
        general_channel_id = os.environ.get(
            "PRODUCTION_GENERAL_CHANNEL"
            if is_production
            else "DEVELOPMENT_GENERAL_CHANNEL"
        )
        if general_channel_id is None:
            raise ValueError("General channel ID is not set")
        self.general_channel_id = int(general_channel_id)
        # pylint: disable=no-member
        self.daily_animal_task.start()

    @commands.Cog.listener()
    async def on_ready(self):
        """When the bot is ready, start the daily_animal_task."""
        self.bot.loop.create_task(self.daily_animal_task())

    @tasks.loop(time=DAILY_ANIMAL_TIME)
    async def daily_animal_task(self):
        """A task that runs every 60 seconds to check if it's time to send a
        random dog and cat picture to the specified channel.

        If it's time, fetches the picture and sends it.
        """
        if (
            DAILY_ANIMAL_TIME.hour == datetime.utcnow().hour
            and DAILY_ANIMAL_TIME.minute == datetime.utcnow().minute
        ):
            channel = self.bot.get_channel(self.general_channel_id)
            if not isinstance(channel, discord.TextChannel):
                raise ValueError("General channel ID is not a text channel")
            dog_data: Doggo = requests.get(
                "https://random.dog/woof.json", timeout=15
            ).json()
            cat_data: list[Cat] = requests.get(
                "https://api.thecatapi.com/v1/images/search", timeout=15
            ).json()
            if dog_data is None or cat_data is None:
                print("Failed to get animal pictures")
                return
            await channel.send(
                "Hello Galaxy!  I hope you have a wonderful day.  "
                "Here's your daily animal pictures to start the day right.  "
                "Enjoy!  :smiley:"
            )
            await channel.send(f"{dog_data['url']}")
            await channel.send(f"{cat_data[0]['url']}")


def setup(bot: commands.Bot):
    """Add the DailyAnimal cog to the bot.

    :param bot: A commands.Bot instance.
    """
    bot.add_cog(DailyAnimalService(bot))
