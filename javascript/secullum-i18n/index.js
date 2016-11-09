let expressions;
let dateFormat;

const regexPlaceholders = /\{(\d+)\}/g;

const init = data => {
  expressions = data.expressions;
  dateFormat = data.dateFormat;
};

const translate = (expression, ...args) => {
  let translatedExpression = expressions[expression];

  if (!translatedExpression) {
    translatedExpression = expression;
  }

  return translatedExpression.replace(regexPlaceholders, (match, number) => {
    const argIndex = parseInt(number, 10);
    return args[argIndex];
  });
};

const getDateFormat = () => {
  return dateFormat;
};

module.exports = {
  init,
  translate,
  getDateFormat
};
