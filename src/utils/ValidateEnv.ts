export const validateEnv = () => {
  if (process.env.TOKEN == null) {
    console.warn("Bot Token is not valid");
    return false;
  }
  if (process.env.BUFF_CHANNEL_ID == null) {
    console.warn("Buff Channel ID is not valid");
    return false;
  }
  return true;
};
