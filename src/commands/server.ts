import { EmbedBuilder, SlashCommandBuilder } from "discord.js";
import { Command } from "../interfaces/Command";

export const server: Command = {
  data: new SlashCommandBuilder()
    .setName("server")
    .setDescription("Look up information on the current server."),
  run: async (interaction) => {
    await interaction.deferReply({ ephemeral: true });
    const server = interaction.guild;
    if (server == null) {
      console.warn("Server was null on a server command!");
      interaction.reply("Something went wrong.");
      return;
    }
    const onlineMembers = server.members.cache.filter(
      (member) => member.presence?.status == "online" && !member.user.bot
    ).size;
    const totalMembers = server.members.cache.filter(
      (member) => !member.user.bot
    ).size;
    const replyEmbed = new EmbedBuilder()
      .setThumbnail(server.iconURL())
      .addFields([
        {
          name: "Owner",
          value: (await server.fetchOwner()).user.username,
          inline: true,
        },
        {
          name: "Created On",
          value: server.createdAt.toLocaleDateString(),
          inline: true,
        },
        {
          name: "Online Members",
          value: onlineMembers.toString(),
          inline: true,
        },
        {
          name: "Total Members",
          value: totalMembers.toString(),
          inline: true,
        },
      ])
      .setColor(0x0000ff);

    interaction.editReply({ embeds: [replyEmbed] });
  },
};
