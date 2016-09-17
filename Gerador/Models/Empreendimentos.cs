

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
        public int IDEmpreendimento { get; set; }
        public string Nome { get; set; }
        public System.DateTime DataEntrega { get; set; }
        public string Produto { get; set; }
        public string Campanha { get; set; }
        public int IDEmpresa { get; set; }
    
        public virtual Empresas Empresas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Unidades> Unidades { get; set; }
    }
}
