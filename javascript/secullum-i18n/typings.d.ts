declare module 'secullum-i18n' {
  export interface SecullumI18nData {
    language: string;
    dateTimeFormat: string;
    dateFormat: string;
    timeFormat: string;
    dayMonthFormat: string;
    expressions: { [key: string]: string };
  }

  export class Translator {
    language: string;
    dateTimeFormat: string;
    dateFormat: string;
    timeFormat: string;
    dayMonthFormat: string;

    constructor(data: SecullumI18nData);
    translate(expression: string, ...args: string[]);
    translateFirstUpper(expression: string, firstCharUpperCase: boolean, ...args: string[]);
  }

  export const init: (data: SecullumI18nData) => void;
  export const translate: (expression: string, ...args: string[]) => string;
  export const getLanguage: () => string;
  export const getDateTimeFormat: () => string;
  export const getDateFormat: () => string;
  export const getTimeFormat: () => string;
  export const getDayMonthFormat: () => string;
}
