import { REST } from "@discordjs/rest";
import { Routes } from "discord-api-types/v9";
import { Client, TextChannel } from "discord.js";
import { CommandList } from "../commands/_CommandList";
import cron from "node-cron";
import { BuffService } from "../services/BuffService";

export const onReady = async (client: Client) => {
  const rest = new REST({ version: "9" }).setToken(process.env.TOKEN as string);

  const commandData = CommandList.map((command) => command.data.toJSON());

  await rest.put(Routes.applicationCommands(client.user?.id || "missing id"), {
    body: commandData,
  });

  cron.schedule(
    "0 5 * * *",
    () => {
      if (BuffService.CheckBuffsUpdated()) {
        BuffService.SetBuffsUpdated(false);
      } else {
        const buffChannel = client.channels.cache.find((channel) => {
          return channel.id === process.env.BUFF_CHANNEL_ID;
        });

        if (buffChannel === undefined) {
          console.warn("Buff channel was not found during cron job");
          return;
        }
        (buffChannel as TextChannel).send(
          `@here, I have not seen the Buff Cat lately.  Please honor me with its presence if the buffs have been updated.`
        );
      }
    },
    { timezone: "Etc/UTC" }
  );
  console.log("Discord ready!");
};
