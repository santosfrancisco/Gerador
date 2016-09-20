

namespace Gerador.Models
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;

	public partial class Empreendimentos
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Empreendimentos()
        {
            this.Unidades = new HashSet<Unidades>();
        }
		[Key]
		[Display(Name = "Empreendimento")]
		public int IDEmpreendimento { get; set; }
		[Required(ErrorMessage = "Obrigatório informar nome do empreendimento")]
		[StringLength(80, ErrorMessage = "Nome do empreendimento deve possuir no máximo 80 caracteres")]
		public string Nome { get; set; }
		[Display(Name = "Habite-se")]
		[Required(ErrorMessage = "Data de habite-se é obrigatória")]
		[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
		public System.DateTime DataEntrega { get; set; }
		public string Produto { get; set; }
		public string Campanha { get; set; }
		[Display(Name = "Empresa")]
		public int IDEmpresa { get; set; }

		public virtual Empresas Empresas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Unidades> Unidades { get; set; }
    }
}
