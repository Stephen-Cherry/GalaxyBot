import { EmbedBuilder, GuildMember, SlashCommandBuilder } from "discord.js";
import { Command } from "../interfaces/Command";

export const info: Command = {
  data: new SlashCommandBuilder()
    .setName("info")
    .setDescription(
      "Look up information on a user account. Defaults to current user"
    )
    .addUserOption((option) =>
      option
        .setName("user")
        .setDescription("User to look up information on")
        .setRequired(false)
    )
    .setDefaultMemberPermissions("0"),
  run: async (interaction) => {
    await interaction.deferReply({ ephemeral: true });
    const member: GuildMember =
      (interaction.options.getMember("user") as GuildMember) ??
      interaction.member;
    if (member == null) {
      console.warn(
        "There was an issue finding the member during an info command"
      );
      return;
    }
    const replyEmbed = new EmbedBuilder()
      .setThumbnail(member.displayAvatarURL())
      .addFields([
        { name: "Username", value: member.user.username, inline: true },
        {
          name: "Discriminator",
          value: member.user.discriminator,
          inline: true,
        },
        { name: "ID", value: member.id, inline: true },
        {
          name: "Created At",
          value: member.user.createdAt.toLocaleDateString(),
          inline: true,
        },
        {
          name: "Joined At",
          value: member.joinedAt?.toLocaleDateString() ?? "unknown",
          inline: true,
        },
        {
          name: "Status",
          value: member.presence?.status.toString() ?? "unknown",
          inline: true,
        },
        { name: "Is Bot", value: member.user.bot ? "Yes" : "No", inline: true },
        { name: "Mention", value: `<@${member.user.id}>`, inline: true },
      ])
      .setColor(0x0000ff);

    interaction.editReply({ embeds: [replyEmbed] });
  },
};
