import { Client, Events } from "discord.js";
import { IntentOptions } from "./config/IntentOptions";
import { onInteraction } from "./events/onInteraction";
import { onMessage } from "./events/onMessage";
import { onReady } from "./events/onReady";
import { validateEnv } from "./utils/ValidateEnv";
import "dotenv/config";

(async () => {
  if (!validateEnv) return;
  const client = new Client({ intents: IntentOptions });

  client.once(Events.ClientReady, async (client) => {
    await onReady(client);
  });

  client.on(Events.InteractionCreate, async (interaction) => {
    await onInteraction(interaction);
  });

  client.on(Events.MessageCreate, async (message) => {
    await onMessage(message);
  });

  await client.login(process.env.TOKEN);
})();
