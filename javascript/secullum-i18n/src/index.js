let expressions;
let dateFormat;

const regexPlaceholders = /\{(\d+)\}/g;

export const init = data => {
  expressions = data.expressions;
  dateFormat = data.dateFormat;
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

export const getDateFormat = () => {
  return dateFormat;
};
