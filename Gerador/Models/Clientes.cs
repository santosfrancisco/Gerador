
namespace Gerador.Models
{
	using IdentitySample.Models;
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	public partial class Clientes
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Clientes()
        {
            this.Analises = new HashSet<Analises>();
        }
		[Key]
        public int IDCliente { get; set; }
        public int TipoPessoa { get; set; }
        public string Nome { get; set; }
        public string CpfCnpj { get; set; }
        public string Sexo { get; set; }
        public string Profissao { get; set; }
        public Nullable<System.DateTime> DataNascimento { get; set; }
        public string Renda { get; set; }
        public string EstadoCivil { get; set; }
        public string RegimeCasamento { get; set; }
        public string Conjuge_Cpf { get; set; }
        public string Conjuge_Nome { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Analises> Analises { get; set; }
		//public virtual Usuarios Usuarios { get; set; }
		[ForeignKey("User")]
		public string IDUsuario { get; set; }
		public virtual ApplicationUser User { get; set; }
	}
}
