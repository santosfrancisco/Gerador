using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gerador.Models
{
	public class Empresas
	{

		[Key]
		public int IDEmpresa { get; set; }

		[Required(AllowEmptyStrings = false, ErrorMessage = "Fornceça um nome para a Empresa")]
		[MaxLength(128, ErrorMessage = "Tamanho máximo {0} excedido")]
		[Display(Name = "Nome da Empresa")]
		public string Nome { get; set; }
		public string Responsavel { get; set; }
		public string Responsavel_Email { get; set; }
		public string Responsavel_Telefone { get; set; }
		
	}
}