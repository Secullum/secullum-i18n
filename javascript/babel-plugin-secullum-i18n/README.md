# babel-plugin-secullum-i18n

![npm](https://img.shields.io/npm/v/babel-plugin-secullum-i18n.svg)
![npm](https://img.shields.io/npm/l/babel-plugin-secullum-i18n.svg)

## Install

```sh
yarn add babel-plugin-secullum-i18n --dev
```

## Usage

### Via [`webpack config`](https://webpack.js.org/loaders/babel-loader/)

```js
module: {
  rules: [
    {
      test: /\.js$/,
      exclude: /node_modules/,
      use: {
        loader: "babel-loader",
        options: {
          plugins: [
            [
              "babel-plugin-secullum-i18n",
              {
                expressionsPath: "./sec-i18n.config.json",
                translateFunction: "traduzir",
              },
            ],
          ],
        },
      },
    },
  ];
}
```

### Via `babel.config.js`

```js
module.exports = (api) => ({
  plugins: [
    [
      "babel-plugin-secullum-i18n",
      {
        expressionsPath: "./sec-i18n.config.json",
        translateFunction: "translate",
      },
    ],
  ],
});
```

### Via `config-overrides.js`

```js
module.exports = function override(config, env) {
  config.module.rules[2].oneOf.forEach((rule) => {
    if (rule.test instanceof RegExp && rule.test.test(".js")) {
      rule.options.plugins.push([
        "babel-plugin-secullum-i18n",
        {
          expressionsPath: "./sec-i18n.config.json",
          translateFunction: "translate",
        },
      ]);
    }
  });

  return config;
};
```

## Options

- `expressionsPath`: string
  - Path to expressions file.
- `translateFunction`: string
  - Name of translate function used on project.
