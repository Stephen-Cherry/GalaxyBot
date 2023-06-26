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
}
