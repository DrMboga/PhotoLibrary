export const ensureError = (value: unknown): Error => {
  if (value instanceof Error) return value;

  let serialized = '[Unable to stringify the thrown value]';
  try {
    serialized = JSON.stringify(value);
  } catch {}

  return new Error(`This value was thrown as is, not through an Error: ${serialized}`);
};
