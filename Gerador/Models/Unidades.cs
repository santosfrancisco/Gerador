

namespace Gerador.Models
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Linq;
	using System.Web.Mvc;

	public partial class Unidades
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public Unidades()
		{
			this.Analises = new HashSet<Analises>();
		}
		[ForeignKey("Empreendimentos")]
		public int IDEmpreendimento { get; set; }
		[Key]
        [Display(Name = "Unidade")]
		public int IDUnidade { get; set; }
        [Display(Name ="Número")]
		public string Numero { get; set; }
        [Display(Name = "Status")]
		public Status UnidadeStatus { get; set; }
		public Tipos Tipo { get; set; }
        [Display(Name = "Observação")]
		public string UnidadeObservacao { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<Analises> Analises { get; set; }
		public virtual Empreendimentos Empreendimentos { get; set; }

		//public static List<SelectListItem> TipoUnidade()
		//{
		//	IList<SelectListItem> tipo = new List<SelectListItem>();
		//	tipo.Add(new SelectListItem() { Text = "Residencial", Value = "0" });
		//	tipo.Add(new SelectListItem() { Text = "Comercial", Value = "1" });
		//	return tipo.ToList();
		//}
		//public static List<SelectListItem> StatusUnidade()
		//{
		//	IList<SelectListItem> status = new List<SelectListItem>();
		//	status.Add(new SelectListItem() { Text = "Disponível", Value = "0" });
		//	status.Add(new SelectListItem() { Text = "Vendida", Value = "1" });
		//	return status.ToList();
		//}
		public enum Status
		{
			Livre, Análise, Concluída, Suspensa
		}
		public enum Tipos
		{
			Residencial, Comercial
		}
	}
}
