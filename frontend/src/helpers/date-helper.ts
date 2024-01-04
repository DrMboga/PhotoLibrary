export const currentDateLinuxTime = (): number => {
  const currentDate = new Date();
  return Math.floor((currentDate.getTime() + currentDate.getDate()) / 1000);
};

export const dateFromUnixTime = (unixTime: number): Date => new Date(unixTime * 1000);

export const secondsToTimeFormat = (totalSeconds: number): string => {
  const minutes: number = Math.trunc(totalSeconds / 60);
  let seconds: number = totalSeconds - minutes * 60;
  return `${minutes}:${seconds}`;
};
