from interactions import Extension, SlashContext, slash_command


class Source(Extension):
    @slash_command(name="source", description="Get a link to the bot source code.")
    async def help(self, ctx: SlashContext):
        await ctx.send("https://github.com/Stephen-Cherry/GalaxyBotv2/", ephemeral=True)
