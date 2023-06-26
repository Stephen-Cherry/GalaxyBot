import { Client, Message, TextChannel } from "discord.js";
var cron = require("node-cron");

export class BuffService {
  static validHours: number[] = [0, 1, 2, 3, 4];
  static cooldown: number = 0;
  static buffUpdated: boolean = false;

  static CheckCooldown = (dateTimeStamp: number) => {
    return this.cooldown - dateTimeStamp <= 0;
  };

  static SetCooldown = (dateTimeStamp: number) => {
    this.cooldown = dateTimeStamp;
  };

  static CheckBuffsUpdated = () => {
    return this.buffUpdated;
  };

  static SetBuffsUpdated = (status: boolean) => {
    this.buffUpdated = status;
  };

  static ScheduleJob = (client: Client) => {
    cron.schedule(
      "0 5 * * *",
      async () => {
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
          await (buffChannel as TextChannel).send(
            `@here, I have not seen the Buff Cat lately.  Please honor me with its presence if the buffs have been updated.`
          );
        }
      },
      { timezone: "Etc/UTC" }
    );
  };

  static HandleMessage = async (message: Message) => {
    const currentDate = new Date();
    const currentHour: number = currentDate.getUTCHours();
    await message.reply("Praise be to the Buff Cat!");
    if (
      message.channelId === process.env.BUFF_CHANNEL_ID &&
      BuffService.validHours.includes(currentHour) &&
      BuffService.CheckCooldown(new Date().getTime())
    ) {
      currentDate.setUTCHours(currentHour + 6);
      BuffService.SetBuffsUpdated(true);
      BuffService.SetCooldown(currentDate.getTime());
    }
  };
}
