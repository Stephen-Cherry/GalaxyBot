from interactions import (
    Extension,
    GuildText,
    Modal,
    ModalContext,
    OptionType,
    ParagraphText,
    Permissions,
    SlashContext,
    slash_command,
    slash_default_member_permission,
    slash_option,
)


class Message(Extension):
    @slash_command(name="message", description="Send a message as GalaxyBot.")
    @slash_default_member_permission(Permissions.ADMINISTRATOR)
    @slash_option(
        name="channel",
        description="Channel to send message to.",
        required=True,
        opt_type=OptionType.CHANNEL,
    )
    async def help(self, ctx: SlashContext, channel: GuildText):
        bot_message_modal = Modal(
            ParagraphText(label="Message", custom_id="bot_message_text"),
            title="Bot Message",
        )
        await ctx.send_modal(modal=bot_message_modal)
        modal_ctx: ModalContext = await ctx.bot.wait_for_modal(bot_message_modal)
        message: str | None = modal_ctx.responses.get("bot_message_text")
        await modal_ctx.send(f"Submitted to {channel.mention}.", ephemeral=True)
        if message:
            await channel.send(message)
