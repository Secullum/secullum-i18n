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
    .then(recordset => recordset[0]);
};

const close = () => {
  return sql.close();
};

module.exports = {
  open,
  fetch,
  close
};
