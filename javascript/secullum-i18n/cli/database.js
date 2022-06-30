const sql = require('mssql');

const open = config => {
  return sql.connect(config);
};

const fetch = portugues => {
  const query =
    'select portugues as pt, ingles as en, espanhol as es ' +
    'from expressoes where portugues = @portugues';

  return new sql.Request()
    .input('portugues', portugues)
    .query(query)
    .then(result => {
      const found = result.recordset.length > 0;

      return {
        found,
        row: found ? result.recordset[0] : null,
        expression: portugues
      };
    });
};

const insert = portugues => {
  const query = 'insert into expressoes (portugues) values (@portugues)';

  return new sql.Request()
    .input('portugues', portugues)
    .query(query);
};

const close = () => {
  return sql.close();
};

module.exports = {
  open,
  fetch,
  insert,
  close
};
