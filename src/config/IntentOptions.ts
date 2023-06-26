import { GatewayIntentBits, IntentsBitField } from "discord.js";

export const IntentOptions: GatewayIntentBits[] = [
  IntentsBitField.Flags.Guilds,
  IntentsBitField.Flags.GuildMessages,
  IntentsBitField.Flags.MessageContent,
  IntentsBitField.Flags.GuildPresences,
];
