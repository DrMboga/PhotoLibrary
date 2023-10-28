export const currentDateLinuxTime = (): number => {
  const currentDate = new Date();
  return Math.floor(currentDate.getTime() + currentDate.getDate() / 1000);
};
