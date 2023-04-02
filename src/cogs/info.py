"""File holding the Cog for GitHub commands"""
import discord
from discord.ext import commands


class InfoCog(commands.Cog):
    """Github Commands Cog"""

    def __init__(self, bot: discord.Bot) -> None:
        self.bot = bot

    @commands.slash_command(
        name="info", description="Get information about yourself or another user"
    )
    async def info(self, ctx: discord.ApplicationContext, user: discord.Member) -> None:
        """Info Command"""
        user = user or ctx.author
        avatar_url = user.avatar.url if user.avatar else ""
        embed = discord.Embed()
        embed.set_author(name=user.name, icon_url=avatar_url)
        embed.set_thumbnail(url=avatar_url)
        embed.add_field(name="Username", value=user.name, inline=True)
        embed.add_field(name="Discriminator", value=user.discriminator, inline=True)
        embed.add_field(name="ID", value=str(user.id), inline=True)
        embed.add_field(
            name="Created at", value=user.created_at.strftime("%b %d, %Y"), inline=True
        )
        embed.add_field(
            name="Joined at",
            value=user.joined_at.strftime("%b %d, %Y") if user.joined_at else "N/A",
            inline=True,
        )
        embed.add_field(
            name="Status",
            value=str(user.status),
            inline=True,
        )
        embed.add_field(name="Bot", value=str(user.bot), inline=True)
        embed.add_field(name="Mention", value=user.mention, inline=True)
        embed.color = 0x0000FF  # blue color

        await ctx.respond(embed=embed, ephemeral=True)


def setup(bot: discord.Bot) -> None:
    """Adds cog to the bot"""
    bot.add_cog(InfoCog(bot))
