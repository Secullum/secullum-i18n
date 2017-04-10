let language;
let dateTimeFormat;
let dateFormat;
let timeFormat;
let expressions = {};

const regexPlaceholders = /\{(\d+)\}/g;

export const init = data => {
  language = data.language;
  dateTimeFormat = data.dateTimeFormat;
  dateFormat = data.dateFormat;
  timeFormat = data.timeFormat;
  expressions = data.expressions;
};

export const translate = (expression, ...args) => {
  let translatedExpression = expressions[expression];

  if (!translatedExpression) {
    translatedExpression = expression;
  }

  return translatedExpression.replace(regexPlaceholders, (match, number) => {
    const argIndex = parseInt(number, 10);
    return args[argIndex];
  });
};

export const getLanguage = () => {
  return language;
};

export const getDateTimeFormat = () => {
  return dateTimeFormat;
};

export const getDateFormat = () => {
  return dateFormat;
};

export const getTimeFormat = () => {
  return timeFormat;
};
