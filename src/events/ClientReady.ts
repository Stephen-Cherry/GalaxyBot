import { REST } from "@discordjs/rest";
import { Routes } from "discord-api-types/v9";
import { Client } from "discord.js";
import { CommandList } from "../commands/_CommandList";
import { BuffService } from "../services/BuffService";

export const ClientReady = async (client: Client) => {
  const rest = new REST({ version: "9" }).setToken(process.env.TOKEN as string);

  const commandData = CommandList.map((command) => command.data.toJSON());

  await rest.put(Routes.applicationCommands(client.user?.id || "missing id"), {
    body: commandData,
  });

  BuffService.ScheduleJob(client);

  console.log("Discord ready!");
};
