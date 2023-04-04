"""A Discord bot module to send a random dog picture to a specific channel
every day using the https://random.dog/woof.json API."""

from datetime import datetime
import os
import discord
from discord.ext import commands, tasks
import requests
from src.constants.constants import DAILY_DOGGO_TIME

from src.models.doggo_response import DoggoResponse


class DailyDoggoService(commands.Cog):
    """A cog that sends a random dog picture to a specified channel every
    day."""

    def __init__(self, bot: discord.Bot):
        """Initialize the DailyDoggo cog.

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
        self.daily_doggo_task.start()

    @commands.Cog.listener()
    async def on_ready(self):
        """When the bot is ready, start the daily_doggo_task."""
        self.bot.loop.create_task(self.daily_doggo_task())

    @tasks.loop(time=DAILY_DOGGO_TIME)
    async def daily_doggo_task(self):
        """A task that runs every 60 seconds to check if it's time to send a
        random dog picture to the specified channel.

        If it's time, fetches the picture and sends it.
        """
        if (
            DAILY_DOGGO_TIME.hour == datetime.utcnow().hour
            and DAILY_DOGGO_TIME.minute == datetime.utcnow().minute
        ):
            channel = self.bot.get_channel(self.general_channel_id)
            if not isinstance(channel, discord.TextChannel):
                raise ValueError("General channel ID is not a text channel")
            data: DoggoResponse = requests.get(
                "https://random.dog/woof.json", timeout=15
            ).json()
            if data is None:
                print("Failed to get doggo picture")
                return
            await channel.send(
                "Hello Galaxy!  I hope you have a wonderful day.  "
                "Here's your daily doggo picture to start the day right.  "
                "Enjoy!  :smiley:"
            )
            await channel.send(f"{data['url']}")


def setup(bot: commands.Bot):
    """Add the DailyDoggo cog to the bot.

    :param bot: A commands.Bot instance.
    """
    bot.add_cog(DailyDoggoService(bot))
