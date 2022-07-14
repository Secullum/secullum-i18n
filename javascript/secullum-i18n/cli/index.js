#!/usr/bin/env node

const path = require("path");
const fs = require("fs");
const fetch = require("node-fetch");

// Read config file
const cwd = process.cwd();
const configFilePath = path.join(cwd, "sec-i18n.config.json");
const configFileContents = fs.readFileSync(configFilePath, "utf8");
const config = JSON.parse(configFileContents);

// Prepare output file path
const outputDir = path.join(cwd, config.outputDir);

const directoryExists = (path) => {
  try {
    return fs.statSync(path).isDirectory();
  } catch (err) {
    return false;
  }
};

if (!directoryExists(outputDir)) {
  fs.mkdirSync(outputDir);
}

// Fetch expressions from WebService
let webServiceQuery = async () => {
  const expressions = { expressions: config.expressions };

  const response = await fetch(config.webservice.url + "Expressions", {
    method: "POST",
    body: JSON.stringify(expressions),
    headers: {
      "Content-Type": "application/json",
    },
  });

  if (response.status === 400) {
    const responseData = await response.json();
    throw new Error(responseData.errorMessage);
  } else if (response.status !== 200) {
    throw new Error(response.status + " - " + response.statusText);
  }

  const responseData = await response.json();

  for (const language in responseData) {
    const outputFilePath = path.join(outputDir, `${language}.json`);

    const outputFileData = {
      language,
      dateTimeFormat: config.languages[language].dateTimeFormat,
      dateFormat: config.languages[language].dateFormat,
      timeFormat: config.languages[language].timeFormat,
      dayMonthFormat: config.languages[language].dayMonthFormat,
      expressions: responseData[language],
    };

    fs.writeFileSync(
      outputFilePath,
      JSON.stringify(outputFileData, null, 2),
      "utf8"
    );
  }
};

webServiceQuery().catch((err) => {
  console.error(err.message);
  process.exit(1);
});
