namespace Gerador.Models
{
	using Gerador.Models;
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Linq;
	using System.Web.Mvc;

	public partial class Analises
    {
		[Key]
        public int IDAnalise { get; set; }
        [Display(Name = "Habite-se")]
        public System.DateTime DataEntrega { get; set; }
        [Display(Name = "Valor de Financiamento")]
        public decimal ValorFinanciamento { get; set; }
        [Display(Name = "Valor Total")]
        public decimal ValorTotal { get; set; }
        public decimal SaldoDevedor { get; set; }
        [Display(Name = "Observação")]
        public string Observacao { get; set; }
        [Display(Name = "Tipo de análise")]
        public string TipoAnalise { get; set; }
		[ForeignKey("Clientes")]
        [Display(Name = "Cliente")]
		public int IDCliente { get; set; }
		[ForeignKey("Unidades")]
        [Display(Name = "Unidade")]
		public int IDUnidade { get; set; }
		
		public virtual Clientes Clientes { get; set; }
        public virtual Unidades Unidades { get; set; }
		[ForeignKey("User")]
        [Display(Name = "Responsável")]
		public string IDUsuario { get; set; }
		public virtual ApplicationUser User { get; set; }

        public static List<SelectListItem> Tipos()
        {
            IList<SelectListItem> tiposAnalise = new List<SelectListItem>();
            tiposAnalise.Add(new SelectListItem() { Text = "Padrão", Value = "Padrão" });
            tiposAnalise.Add(new SelectListItem() { Text = "Aluguel", Value = "Aluguel" });
            return tiposAnalise.ToList();
        }

    }
}
