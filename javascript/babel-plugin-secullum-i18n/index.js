let expressions = null;

module.exports = function () {
  return {
    visitor: {
      CallExpression(path, state) {
        if (!expressions) {
          expressions = require(state.opts.expressionsPath).expressions;
        }

        const node = path.node;

        if (
          node.callee.name === state.opts.translateFunctionName &&
          node.arguments[0].type === "StringLiteral" &&
          !expressions.includes(node.arguments[0].value)
        ) {
          throw path.buildCodeFrameError(
            `Expression not found: ${node.arguments[0].value}`
          );
        }
      },
    },
  };
};
