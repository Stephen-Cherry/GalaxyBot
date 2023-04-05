"""Buff Reminder Service for Discord.

This module provides a Discord bot cog, BuffReminderService, that listens for 
messages containing ':BuffCat:' and sends a response if the cooldown period has 
passed.

The module includes:
- BuffReminderService: A cog that listens for specific messages and sends a 
response.
- setup: A function to add the cog to the bot.
"""

from datetime import datetime, timedelta
import os
import discord

from discord.ext import commands, tasks
from src.constants.constants import DAILY_BUFF_TIME


class BuffReminderService(commands.Cog):
    """A cog that sends a response to messages containing ':BuffCat:' if the
    cooldown period has passed."""

    def __init__(self, bot: discord.Bot):
        self.bot = bot
        self.cooldown: datetime = datetime.min
        self.buffs_renewed_today: bool = False
        self._buff_channel_id: int = get_buff_channel_id()
        # pylint: disable=no-member
        self.buff_reminder_task.start()

    @commands.Cog.listener()
    async def on_message(self, message: discord.Message):
        """Listener for incoming messages containing ':BuffCat:'.

        This method listens for messages containing ':BuffCat:' and sends a
        response if the cooldown period has passed. It updates the cooldown
        after sending a response.

        Args:
            message (discord.Message): The message object containing the
            message data.
        """
        if (
            ":BuffCat:" in message.content
            and self.cooldown < datetime.utcnow()
        ):
            await message.channel.send("Praise be to the Buff Cat!")
            self.cooldown = datetime.utcnow() + timedelta(hours=12)
            # pylint: disable=no-member
            if self.buff_reminder_task.is_running():
                self.buff_reminder_task.cancel()
            self.buff_reminder_task.restart()
            self.buffs_renewed_today = True

    @tasks.loop(time=DAILY_BUFF_TIME)
    async def buff_reminder_task(self):
        """Periodically check if the buff due time has passed and send a
        reminder message.

        This method is a pycord tasks loop that runs every 60 seconds.
        It checks if the current UTC time has passed the buff due time.
        If so, it retrieves the buff channel using the stored channel ID
        and sends a reminder message to the channel, indicating that the
        buff is due. After sending the message, the task loop is
        stopped. If the buff channel is not a TextChannel, it raises a
        ValueError.
        """
        if (
            datetime.utcnow().hour == DAILY_BUFF_TIME.hour
            and datetime.utcnow().minute == DAILY_BUFF_TIME.minute
        ):
            if self.buffs_renewed_today:
                self.buffs_renewed_today = False
            else:
                channel = self.bot.get_channel(self._buff_channel_id)
                if isinstance(channel, discord.TextChannel):
                    await channel.send(
                        "@here, I have not seen the Buff Cat lately.  "
                        "Please honor me with its presence if the buffs have"
                        "been updated."
                    )
                    # pylint: disable=no-member
                    self.buff_reminder_task.stop()
                else:
                    raise ValueError(
                        "Buff channel did not return as a text channel"
                    )


def setup(bot: discord.Bot):
    """Adds cog to the bot."""
    bot.add_cog(BuffReminderService(bot))


def get_buff_channel_id() -> int:
    """Get the Buff Channel ID from environment variables.

    This function retrieves the Buff Channel ID from environment variables
    based on whether the bot is running in production or development mode.
    It raises a ValueError if the channel ID is not found.

    Returns:
        int: The Buff Channel ID as an integer.

    Raises:
        ValueError: If the Buff Channel ID is not found in environment
        variables.
    """
    is_production = os.environ.get("IS_PRODUCTION") == "true"
    if is_production is None:
        raise ValueError(
            "Production boolean not found in Environment Variables"
        )
    channel_id = os.environ.get(
        "PRODUCTION_BUFF_CHANNEL"
        if is_production
        else "DEVELOPMENT_BUFF_CHANNEL"
    )
    if channel_id:
        return int(channel_id)
    raise ValueError("Could not find Buff Channel ID in Environment Variables")
