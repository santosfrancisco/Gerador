
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
		[Display(Name = "Profiss�o")]
        public string Profissao { get; set; }
		[Display(Name = "Data de Nascimento")]
        public Nullable<System.DateTime> DataNascimento { get; set; }
        public string Renda { get; set; }
		[Display(Name = "Estado Civil")]
        public string EstadoCivil { get; set; }
		[Display(Name = "Regime de casamento")]
        public string RegimeCasamento { get; set; }
		[Display(Name = "C�njuge CPF")]
        public string Conjuge_Cpf { get; set; }
		[Display(Name = "C�njuge Nome")]
        public string Conjuge_Nome { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Analises> Analises { get; set; }
		//public virtual Usuarios Usuarios { get; set; }
		[ForeignKey("User")]
		[Display(Name = "Respons�vel")]
		public string IDUsuario { get; set; }
		public virtual ApplicationUser User { get; set; }

		// lista de tipos de pessoa
		public static List<SelectListItem> ListaTipo()
		{
			IList<SelectListItem> tipo = new List<SelectListItem>();
			tipo.Add(new SelectListItem() { Text = "F�sica", Value = "0" });
			tipo.Add(new SelectListItem() { Text = "Jur�dica", Value = "1" });

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
			estadosCivis.Add(new SelectListItem() { Text = "Vi�vo(a)", Value = "Vi�vo(a)" });
			estadosCivis.Add(new SelectListItem() { Text = "Uni�o Est�vel", Value = "Uni�o Est�vel" });
			return estadosCivis.ToList();
		}

		// lista de regimes de casamento
		public static List<SelectListItem> ListaRegimeCasamento()
		{
			IList<SelectListItem> regimesCasamento = new List<SelectListItem>();
			regimesCasamento.Add(new SelectListItem() { Text = "Comunh�o universal de bens", Value = "Comunh�o universal de bens" });
			regimesCasamento.Add(new SelectListItem() { Text = "Comunh�o parcial de bens", Value = "Comunh�o parcial de bens" });
			regimesCasamento.Add(new SelectListItem() { Text = "Separa��o total de bens", Value = "Separa��o total de bens" });
			regimesCasamento.Add(new SelectListItem() { Text = "Participa��o final nos aquestos", Value = "Participa��o final nos aquestos" });
			regimesCasamento.Add(new SelectListItem() { Text = "Separa��o obrigat�ria de bens", Value = "Separa��o obrigat�ria de bens" });
			return regimesCasamento.ToList();
		}
	}
}
