export const currentDateLinuxTime = (): number => {
  const currentDate = new Date();
  return dateToUnixTime(currentDate);
};

export const dateFromUnixTime = (unixTime: number): Date => new Date(unixTime * 1000);

export const dateToUnixTime = (date: Date): number => {
  return Math.floor((date.getTime() + date.getDate()) / 1000);
};

export const secondsToTimeFormat = (totalSeconds: number): string => {
  const minutes: number = Math.trunc(totalSeconds / 60);
  const seconds: number = totalSeconds - minutes * 60;
  const secondsPad = seconds.toString().padStart(2, '0');
  return `${minutes}:${secondsPad}`;
};
