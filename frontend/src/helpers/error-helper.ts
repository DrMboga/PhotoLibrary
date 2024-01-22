export const ensureError = (value: unknown): Error => {
  if (value instanceof Error) return value;
  if ((value as any)?.hasOwnProperty('message')) {
    return new Error((value as any)['message']);
  }

  let serialized = '[Unable to stringify the thrown value]';
  try {
    serialized = JSON.stringify(value);
  } catch {}

  return new Error(`This value was thrown as is, not through an Error: ${serialized}`);
};

export const errorFromObject = (message: string): string => {
  if (message.startsWith('{')) {
    const errorObject = JSON.parse(message);
    if (!errorObject) {
      return message;
    }
    // {"DuplicateUserName":["Username 'mike@fake.com' is already taken."]}
    // {"PasswordTooShort":["Passwords must be at least 6 characters."],"PasswordRequiresNonAlphanumeric":["Passwords must have at least one non alphanumeric character."],"PasswordRequiresLower":["Passwords must have at least one lowercase ('a'-'z')."],"PasswordRequiresUpper":["Passwords must have at least one uppercase ('A'-'Z')."]}
    const errorsArray: string[] = [];
    for (const errorValue of Object.values(errorObject)) {
      if (Array.isArray(errorValue)) {
        for (const errorArrayElement of errorValue) {
          errorsArray.push(JSON.stringify(errorArrayElement));
        }
      } else {
        errorsArray.push(JSON.stringify(errorValue));
      }
    }
    return errorsArray.join('\n');
  } else {
    return message;
  }
};
