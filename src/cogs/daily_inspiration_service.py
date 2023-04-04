"""
A Discord bot module to send a random weight loss inspiration to a specific channel every day using
a local json file.
"""

import asyncio
from datetime import datetime, timedelta
import json
import os
import random
from typing import List
import discord
from discord.ext import commands, tasks
import pytz

from src.datetime_helper import calculate_daily_quote_time
from src.models.quote import Quote


class DailyInspirationService(commands.Cog):
    """
    A cog that sends a random weight loss inspiration to a specified channel every day.
    """

    def __init__(self, bot: discord.Bot):
        """
        Initialize the DailyInspiration cog.

        :param bot: A discord.Bot instance.
        """
        self.bot = bot
        is_production = os.environ.get("IS_PRODUCTION") == "true"
        weight_loss_channel_id = os.environ.get(
            "PRODUCTION_WEIGHT_LOSS_CHANNEL"
            if is_production
            else "DEVELOPMENT_WEIGHT_LOSS_CHANNEL"
        )
        if weight_loss_channel_id is None:
            raise ValueError("Weight loss channel ID is not set")
        self.weight_loss_channel_id = int(weight_loss_channel_id)
        self.quote_due_time = calculate_daily_quote_time()
        # pylint: disable=no-member
        self.daily_quote_task.start()

    @commands.Cog.listener()
    async def on_ready(self):
        """
        When the bot is ready, start the daily_quote_task.
        """
        self.bot.loop.create_task(self.daily_quote_task())

    @tasks.loop(seconds=60)
    async def daily_quote_task(self):
        """
        A task that runs every 60 seconds to check if it's time to send a random weight loss
        quote to the specified channel. If it's time, fetches the quote and sends it.
        """
        if datetime.now(pytz.UTC) >= self.quote_due_time:
            channel = self.bot.get_channel(self.weight_loss_channel_id)
            if not isinstance(channel, discord.TextChannel):
                raise ValueError("Weight loss channel ID is not a text channel")
            with open("src/data/weight_loss_quotes.json", "r", encoding="utf8") as file:
                data: List[Quote] = json.load(file)
            if data is None:
                print("Failed to get weight loss quotes")
                return
            random_idx = random.randint(0, len(data) - 1)
            embed = discord.Embed(
                title="Daily Weight Loss Inspiration",
                description="I hope this helps you stay motivated!  :muscle:",
                fields=[
                    discord.EmbedField("Quote", data[random_idx]["quote"]),
                    discord.EmbedField("Author", data[random_idx]["author"]),
                ],
                color=0x00FF00,
            )
            await channel.send(embed=embed)
            self.quote_due_time += timedelta(days=1)

    @daily_quote_task.before_loop
    async def before_daily_quote_task(self):
        """
        Calculate the time to wait before the first execution of the daily_quote_task loop
        and sleep for that duration.
        """
        time_to_wait = (self.quote_due_time - datetime.now(pytz.UTC)).total_seconds()
        if time_to_wait > 0:
            await asyncio.sleep(time_to_wait)


def setup(bot: commands.Bot):
    """
    Add the DailyInspirationService cog to the bot.

    :param bot: A commands.Bot instance.
    """
    bot.add_cog(DailyInspirationService(bot))
