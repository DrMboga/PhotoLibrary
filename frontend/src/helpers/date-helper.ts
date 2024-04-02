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

export const dateToDateInputFormat = (date: Date): string =>
  `${date.getFullYear()}-${(date.getMonth() + 1).toString().padStart(2, '0')}-${date
    .getDate()
    .toString()
    .padStart(2, '0')}`;

export const parseDateInput = (dateAsString: string): Date => {
  const dateReg = /^\d{4}-\d{2}-\d{2}$/;
  const m = dateAsString.match(dateReg);
  if (m) {
    const dateParts = dateAsString.split('-');
    return new Date(+dateParts[0], +dateParts[1] - 1, +dateParts[2]);
  }
  return new Date();
};
