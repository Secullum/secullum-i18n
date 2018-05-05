const regexPlaceholders = /\{(\d+)\}/g;

export class Translator {
  constructor(data) {
    this.language = data.language;
    this.dateTimeFormat = data.dateTimeFormat;
    this.dateFormat = data.dateFormat;
    this.timeFormat = data.timeFormat;
    this.dayMonthFormat = data.dayMonthFormat;
    this.expressions = data.expressions;
  }

  translate(expression, ...args) {
    let translatedExpression = this.expressions[expression];

    if (!translatedExpression) {
      translatedExpression = expression;
    }

    if (!translatedExpression) {
      return '';
    }

    return translatedExpression.replace(regexPlaceholders, (match, number) => {
      const argIndex = parseInt(number, 10);
      return args[argIndex];
    });
  }
}

let translator;

export const init = data => {
  translator = new Translator(data);
};

export const translate = (expression, ...args) => {
  return translator.translate(expression, ...args);
};

export const translate = (expression, firstCharUpperCase, ...args) => {
  const translatedExpression = translator.translate(expression, ...args);

  if (translatedExpression.lenght === 0) {
    return translatedExpression;
  }

  return translatedExpression.substr(0, 1).toUpperCase() + translatedExpression.substr(1).toLowerCase();
};

export const getLanguage = () => {
  return translator.language;
};

export const getDateTimeFormat = () => {
  return translator.dateTimeFormat;
};

export const getDateFormat = () => {
  return translator.dateFormat;
};

export const getTimeFormat = () => {
  return translator.timeFormat;
};

export const getDayMonthFormat = () => {
  return translator.dayMonthFormat;
};
