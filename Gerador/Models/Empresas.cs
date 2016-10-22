
namespace Gerador.Models
{
	using Gerador.Models;
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;

	public partial class Empresas
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Empresas()
        {
            this.Empreendimentos = new HashSet<Empreendimentos>();
            this.Usuarios = new HashSet<ApplicationUser>();
        }
		[Key]
		[Display(Name = "Empresa")]
		public int IDEmpresa { get; set; }
		[Required]
		[StringLength(80, ErrorMessage = "O {0} deve ter pelo menos {2} caracteres.", MinimumLength = 3)]
		[Display(Name = "Nome")]
		public string Nome { get; set; }
		[Required]
		[StringLength(80, ErrorMessage = "O {0} deve ter pelo menos {2} caracteres.", MinimumLength = 3)]
		[Display(Name = "Responsável")]
		public string Responsavel { get; set; }
		[Required]
		[EmailAddress]
		[StringLength(80, ErrorMessage = "O {0} deve ter pelo menos {2} caracteres.", MinimumLength = 3)]
		[Display(Name = "E-mail")]
		public string Responsavel_Email { get; set; }
		[StringLength(20, ErrorMessage = "O {0} deve ter pelo menos {2} caracteres.")]
		[Display(Name = "Telefone")]
		public string Responsavel_Telefone { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Empreendimentos> Empreendimentos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ApplicationUser> Usuarios { get; set; }
    }
}
