import discord
from discord.ext import commands


class ServerCog(commands.Cog):

    def __init__(self, bot: discord.Bot):
        self.bot = bot

    @commands.slash_command(
        name="server", description="Get information about the current server"
    )
    async def server(self, ctx: discord.ApplicationContext) -> None:
        """Server Command."""

        if ctx.guild is None:
            raise ValueError("Server can't be None")
        if ctx.guild.owner is None:
            raise ValueError("Owner can't be None")

        online_members = sum(
            1
            for member in ctx.guild.members
            if member.status != discord.Status.offline
        )

        icon_url = ctx.guild.icon.url if ctx.guild.icon else ""
        embed = discord.Embed()
        embed.set_author(name=ctx.guild.name, icon_url=icon_url)
        embed.set_thumbnail(url=icon_url)
        embed.add_field(name="Owner", value=ctx.guild.owner.name, inline=True)
        embed.add_field(
            name="Created",
            value=ctx.guild.created_at.strftime("%b %d, %Y"),
            inline=True,
        )
        embed.add_field(
            name="Online Members", value=str(online_members), inline=True
        )
        embed.add_field(
            name="Total Members",
            value=str(ctx.guild.member_count),
            inline=True,
        )
        embed.color = 0x0000FF  # blue color

        await ctx.respond(embed=embed, ephemeral=True)


def setup(bot: discord.Bot) -> None:
    bot.add_cog(ServerCog(bot))
