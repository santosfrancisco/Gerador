

namespace Gerador.Models
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	public partial class Unidades
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Unidades()
        {
            this.Analises = new HashSet<Analises>();
        }
		[Key]
        public int IDUnidade { get; set; }
        public string Numero { get; set; }
		[ForeignKey("Empreendimentos")]
        public int IDEmpreendimento { get; set; }
        public int UnidadeStatus { get; set; }
        public string Tipo { get; set; }
        public string UnidadeObservacao { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Analises> Analises { get; set; }
        public virtual Empreendimentos Empreendimentos { get; set; }
    }
}
