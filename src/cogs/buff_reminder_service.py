from datetime import datetime, timedelta
import os
import discord

from discord.ext import commands, tasks
from src.constants.constants import DAILY_BUFF_TIME

renewal_hours = [number for number in range(0,11)]
renewal_hours.extend([22,23])

class BuffReminderService(commands.Cog):
    def __init__(self, bot: discord.Bot):
        self.bot = bot
        self.cooldown: datetime = datetime.min
        self.buffs_renewed_today: bool = False
        self._buff_channel_id: int = get_buff_channel_id()
        # pylint: disable=no-member
        self.buff_reminder_task.start()

    @commands.Cog.listener()
    async def on_message(self, message: discord.Message) -> None:
        if (
            ":BuffCat:" in message.content
            and self.cooldown < datetime.utcnow()
        ):
            current_hour = datetime.utcnow().hour
            if current_hour in renewal_hours:
                self.buffs_renewed_today = True
            await message.channel.send("Praise be to the Buff Cat!")
            self.cooldown = datetime.utcnow() + timedelta(hours=12)

    @tasks.loop(time=DAILY_BUFF_TIME)
    async def buff_reminder_task(self) -> None:
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
                        "Please honor me with its presence if the buffs have "
                        "been updated."
                    )
                else:
                    raise ValueError(
                        "Buff channel did not return as a text channel"
                    )


def setup(bot: discord.Bot) -> None:
    bot.add_cog(BuffReminderService(bot))


def get_buff_channel_id() -> int:
    channel_id = os.environ.get("BUFF_CHANNEL")
    if channel_id:
        return int(channel_id)
    raise ValueError("Could not find Buff Channel ID in Environment Variables")
