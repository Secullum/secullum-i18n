# secullum-i18n

Packages for i18n used internally by Secullum

## Usage

Create a file named sec-i18n.config.json:

```json
{
  "outputDir": "<directory to output to>",
  "database": {
    "server": "server",
    "database": "database",
    "user": "user",
    "password": "password"
  },
  "languages": {
    "pt": { "dateFormat": "dd/MM/yyyy" },
    "en": { "dateFormat": "MM/dd/yyyy" },
    "es": { "dateFormat": "dd/MM/yyyy" }
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
