# secullum-i18n

Packages for i18n used internally by Secullum

## Usage

Create a file named sec-i18n.config.json:

```json
{
  "outputDir": "<directory to output to>",
  "webservice": {
    "url": "http://localhost:57105/"
  },
  "languages": {
    "pt": {
      "dateTimeFormat": "dd/MM/yyyy HH:mm:ss",
      "dateFormat": "dd/MM/yyyy",
      "timeFormat": "HH:mm:ss"
    },
    "en": {
      "dateTimeFormat": "MM/dd/yyyy hh:mm:ss tt",
      "dateFormat": "MM/dd/yyyy",
      "timeFormat": "hh:mm:ss tt"
    },
    "es": {
      "dateTimeFormat": "dd/MM/yyyy HH:mm:ss",
      "dateFormat": "dd/MM/yyyy",
      "timeFormat": "HH:mm:ss"
    }
  },
  "expressions": [
    "Funcionários",
    "Domingo",
    "Hoje",
    "Sábado",
    "..."
  ]
}
```
## Examples

See the examples folder, to view the configuration and return files.
