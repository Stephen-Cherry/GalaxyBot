import { Client, Events } from "discord.js";
import { IntentOptions } from "./config/IntentOptions";
import { InteractionCreate } from "./events/InteractionCreate";
import { MessageCreate } from "./events/MessageCreate";
import { ClientReady } from "./events/ClientReady";
import { validateEnv } from "./utils/validateEnv";
import "dotenv/config";

(async () => {
  if (!validateEnv) return;
  const client = new Client({ intents: IntentOptions });

  client.once(Events.ClientReady, async (client) => {
    await ClientReady(client);
  });

  client.on(Events.InteractionCreate, async (interaction) => {
    await InteractionCreate(interaction);
  });

  client.on(Events.MessageCreate, async (message) => {
    await MessageCreate(message);
  });

  await client.login(process.env.TOKEN);
})();
