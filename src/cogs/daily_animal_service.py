from datetime import datetime
import os
import discord
from discord.ext import commands, tasks
import requests
from src.constants.constants import DAILY_ANIMAL_TIME

from src.models.animal_responses import Doggo, Cat


class DailyAnimalService(commands.Cog):
    def __init__(self, bot: discord.Bot):
        self.bot = bot
        general_channel_id = os.environ.get("PICS_N_VIDS_CHANNEL")
        if general_channel_id is None:
            raise ValueError("General channel ID is not set")
        self.general_channel_id = int(general_channel_id)
        # pylint: disable=no-member
        self.daily_animal_task.start()

    @commands.Cog.listener()
    async def on_ready(self) -> None:
        self.bot.loop.create_task(self.daily_animal_task())

    @tasks.loop(time=DAILY_ANIMAL_TIME)
    async def daily_animal_task(self) -> None:
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


def setup(bot: commands.Bot) -> None:
    bot.add_cog(DailyAnimalService(bot))
