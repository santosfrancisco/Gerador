

namespace Gerador.Models
{
	using IdentitySample.Models;
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	public partial class Analises
    {
		[Key]
        public int IDAnalise { get; set; }
        public System.DateTime DataEntrega { get; set; }
        public decimal ValorFinanciamento { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal SaldoDevedor { get; set; }
        public string Observacao { get; set; }
        public string TipoAnalise { get; set; }
		[ForeignKey("Clientes")]
		public int IDCliente { get; set; }
		[ForeignKey("Unidades")]
		public int IDUnidade { get; set; }
    
        public virtual Clientes Clientes { get; set; }
        public virtual Unidades Unidades { get; set; }
		[ForeignKey("User")]
		public string IDUsuario { get; set; }
		public virtual ApplicationUser User { get; set; }
	}
}
