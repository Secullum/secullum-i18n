using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Secullum.Internationalization.WebService.Model
{
    /// <summary>
    /// Modelo para cadastro de expressões no banco de dados.
    /// </summary>
    /// <remarks>Escrita por Elinton em 04/07/2022</remarks>
    [Table("expressoes")]
    public class Expression
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("portugues")]
        public string Portuguese { get; set; }

        [Column("ingles")]
        public string English { get; set; }

        [Column("espanhol")]
        public string Spanish { get; set; }

        [Column("criacao_data")]
        public DateTime DateCreated { get; set; }

        [Column("alteracao_data")]
        public DateTime? DateChanged { get; set; }
    }
}
