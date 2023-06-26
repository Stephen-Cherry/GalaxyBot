import { Message } from "discord.js";
import { BuffService } from "../services/BuffService";

export const onMessage = async (message: Message) => {
  const currentDate = new Date();
  const currentHour: number = currentDate.getUTCHours();
  if (message.content.includes(":BuffCat:")) {
    message.reply("Praise be to the Buff Cat!");
    if (
      message.channelId === process.env.BUFF_CHANNEL_ID &&
      BuffService.validHours.includes(currentHour) &&
      BuffService.CheckCooldown(new Date().getTime())
    ) {
      currentDate.setUTCHours(currentHour + 6);
      BuffService.SetBuffsUpdated(true);
      BuffService.SetCooldown(currentDate.getTime());
    }
  }
};
