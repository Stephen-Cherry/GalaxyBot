import { Message } from "discord.js";
import { BuffService } from "../services/BuffService";

export const MessageCreate = async (message: Message) => {
  if (message.content.includes(":BuffCat:")) {
    await BuffService.HandleMessage(message);
  }
};
