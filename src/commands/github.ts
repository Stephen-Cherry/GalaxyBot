import { SlashCommandBuilder } from "discord.js";
import { Command } from "../interfaces/Command";

export const github: Command = {
  data: new SlashCommandBuilder()
    .setName("github")
    .setDescription("Get a link to the GalaxyBot's source code."),
  run: async (interaction) => {
    await interaction.reply({
      content: "https://github.com/Narolith/GalaxyBotV2",
      ephemeral: true,
    });
  },
};
