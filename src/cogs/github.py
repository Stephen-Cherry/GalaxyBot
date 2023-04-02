"""File holding the Cog for GitHub commands"""
import discord
from discord.ext import commands


class GithubCog(commands.Cog):
    """Github Commands Cog"""

    def __init__(self, bot: discord.Bot) -> None:
        self.bot = bot

    @commands.slash_command(
        name="github", description="Get the link to the GalaxyBot GitHub Repository"
    )
    async def github(self, ctx: discord.ApplicationContext) -> None:
        """Hello Command"""
        await ctx.respond("https://github.com/Narolith/GalaxyBotv2/", ephemeral=True)


def setup(bot: discord.Bot) -> None:
    """Adds cog to the bot"""
    bot.add_cog(GithubCog(bot))
