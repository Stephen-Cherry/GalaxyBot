export const validateEnv = () => {
  if (
    !process.env.TOKEN ||
    typeof process.env.TOKEN !== "string" ||
    process.env.TOKEN.trim().length === 0
  ) {
    throw new Error("Invalid Bot Token");
  }
  if (
    !process.env.BUFF_CHANNEL_ID ||
    typeof process.env.BUFF_CHANNEL_ID !== "string" ||
    !/^\d+$/.test(process.env.BUFF_CHANNEL_ID)
  ) {
    throw new Error("Invalid Buff Channel ID");
  }
  return true;
};
