#!/usr/bin/env node

const path = require('path');
const fs = require('fs');
const database = require('./database');

// Read config file
const cwd = process.cwd();
const configFilePath = path.join(cwd, 'sec-i18n.config.json');
const configFileContents = fs.readFileSync(configFilePath, 'utf8');
const config = JSON.parse(configFileContents);

// Prepare output file path
const outputDir = path.join(cwd, config.outputDir);

const directoryExists = path => {
  try {
    return fs.statSync(path).isDirectory();
  } catch (err) {
    return false;
  }
};

if (!directoryExists(outputDir)) {
  fs.mkdirSync(outputDir);
}

// Fetch expressions from database
database.open(config.database)
  .then(() => {
    return Promise.all(
      config.expressions.map(exp => database.fetch(exp))
    );
  })
  .then(recordset => {
    const languages = Object.keys(config.languages);
    const inserts = [];

    for (const language of languages) {
      const expressions = {};

      for (let i = 0; i < recordset.length; i++) {
        if (recordset[i].found) {
          expressions[recordset[i].expression] = recordset[i].row[language];
        } else {
          if (language === 'pt') {
            inserts.push(database.insert(recordset[i].expression));
          }

          expressions[recordset[i].expression] = recordset[i].expression;
        }
      }

      const outputFilePath = path.join(outputDir, `${language}.json`);
      const outputFileData = {
        dateTimeFormat: config.languages[language].dateTimeFormat,
        dateFormat: config.languages[language].dateFormat,
        timeFormat: config.languages[language].timeFormat,
        expressions
      };

      fs.writeFileSync(outputFilePath, JSON.stringify(outputFileData, null, 2), 'utf8');
    }

    return Promise.all(inserts);
  })
  .catch(err => {
    console.error(err.message);
    process.exit(1);
  })
  .then(() => database.close());
