
namespace Gerador.Models
{
	using Gerador.Models;
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Linq;
	using System.Web.Mvc;

	public partial class Clientes
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Clientes()
        {
            this.Analises = new HashSet<Analises>();
        }
		[Key]
        public int IDCliente { get; set; }
		[Display(Name = "Tipo")]
        public int TipoPessoa { get; set; }
        public string Nome { get; set; }
		[Display(Name = "CPF/CNPJ")]
        public string CpfCnpj { get; set; }
        public string Sexo { get; set; }
		[Display(Name = "Profissão")]
        public string Profissao { get; set; }
		[Display(Name = "Data de Nascimento")]
        public Nullable<System.DateTime> DataNascimento { get; set; }
        public string Renda { get; set; }
		[Display(Name = "Estado Civil")]
        public string EstadoCivil { get; set; }
		[Display(Name = "Regime de casamento")]
        public string RegimeCasamento { get; set; }
		[Display(Name = "Cônjuge CPF")]
        public string Conjuge_Cpf { get; set; }
		[Display(Name = "Cônjuge Nome")]
        public string Conjuge_Nome { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Analises> Analises { get; set; }
		//public virtual Usuarios Usuarios { get; set; }
		[ForeignKey("User")]
		[Display(Name = "Responsável")]
		public string IDUsuario { get; set; }
		public virtual ApplicationUser User { get; set; }

		// lista de tipos de pessoa
		public static List<SelectListItem> ListaTipo()
		{
			IList<SelectListItem> tipo = new List<SelectListItem>();
			tipo.Add(new SelectListItem() { Text = "Física", Value = "0" });
			tipo.Add(new SelectListItem() { Text = "Jurídica", Value = "1" });

			return tipo.ToList();
		}
		// lista de sexos
		public static List<SelectListItem> ListaSexo()
		{
			IList<SelectListItem> sexos = new List<SelectListItem>();
			sexos.Add(new SelectListItem() { Text = "Masculino", Value = "Masculino" });
			sexos.Add(new SelectListItem() { Text = "Feminino", Value = "Feminino" });
			return sexos.ToList();
		}
		// lista de estados civis
		public static List<SelectListItem> ListaEstadoCivil()
		{
			IList<SelectListItem> estadosCivis = new List<SelectListItem>();
			estadosCivis.Add(new SelectListItem() { Text = "Solteiro(a)", Value = "Solteiro(a)" });
			estadosCivis.Add(new SelectListItem() { Text = "Casado(a)", Value = "Casado(a)" });
			estadosCivis.Add(new SelectListItem() { Text = "Divorciado(a)", Value = "Divorciado(a)" });
			estadosCivis.Add(new SelectListItem() { Text = "Separado(a) judicialmente", Value = "Separado(a) judicialmente" });
			estadosCivis.Add(new SelectListItem() { Text = "Viúvo(a)", Value = "Viúvo(a)" });
			estadosCivis.Add(new SelectListItem() { Text = "União Estável", Value = "União Estável" });
			return estadosCivis.ToList();
		}

		// lista de regimes de casamento
		public static List<SelectListItem> ListaRegimeCasamento()
		{
			IList<SelectListItem> regimesCasamento = new List<SelectListItem>();
			regimesCasamento.Add(new SelectListItem() { Text = "Comunhão universal de bens", Value = "Comunhão universal de bens" });
			regimesCasamento.Add(new SelectListItem() { Text = "Comunhão parcial de bens", Value = "Comunhão parcial de bens" });
			regimesCasamento.Add(new SelectListItem() { Text = "Separação total de bens", Value = "Separação total de bens" });
			regimesCasamento.Add(new SelectListItem() { Text = "Participação final nos aquestos", Value = "Participação final nos aquestos" });
			regimesCasamento.Add(new SelectListItem() { Text = "Separação obrigatória de bens", Value = "Separação obrigatória de bens" });
			return regimesCasamento.ToList();
		}
	}
}
