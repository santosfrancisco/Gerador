// Parte de cadastro de clientes
$( document ).ready(function() {
	jQuery(function ($) {
		$("#DataNascimento").mask("99/99/9999");
		$("#Conjuge_Cpf").mask("999.999.999-99");
		$("#Renda").mask("#.##0,00", {reverse: true});
		if ($("#TipoPessoa").val() === "0") {
			$("#Registro").text("CPF");
			$("#CpfCnpj").mask("999.999.999-99");
			$("#ClienteSexo").show();
			$("#ClienteProfissao").show();
			$("#ClienteDataNascimento").show();
			$("#ClienteEstadoCivil").show();
			if ($("#EstadoCivil").val() === "Casado(a)" || $("#EstadoCivil").val() === "União Estável") {
				$("#ClienteRegimeCasamento").show();
				$("#ClienteConjuge_Cpf").show();
				$("#ClienteConjuge_Nome").show();
			} else {
				$("#ClienteRegimeCasamento").hide();
				$("#ClienteConjuge_Cpf").hide();
				$("#ClienteConjuge_Nome").hide();
			}
		} else {
			$("#Registro").text("CNPJ");
			$("#CpfCnpj").mask("99.999.999/9999-99");
			$("#Registro").text("CNPJ");
			$("#ClienteSexo").hide();
			$("#ClienteProfissao").hide();
			$("#ClienteDataNascimento").hide();
			$("#ClienteEstadoCivil").hide();
			$("#ClienteRegimeCasamento").hide();
			$("#ClienteConjuge_Cpf").hide();
			$("#ClienteConjuge_Nome").hide();
		}
	});
	$('#TipoPessoa').change(function () {
		if ($("#TipoPessoa").val() === "0") {
			$("#Registro").text("CPF");
			$("#CpfCnpj").mask("999.999.999-99");
			$("#ClienteSexo").show();
			$("#ClienteProfissao").show();
			$("#ClienteDataNascimento").show();
			$("#ClienteEstadoCivil").show();
		} else {
			$("#CpfCnpj").mask("99.999.999/9999-99");
			$("#Registro").text("CNPJ");
			$("#ClienteSexo").hide();
			$("#ClienteProfissao").hide();
			$("#ClienteDataNascimento").hide();
			$("#ClienteEstadoCivil").hide();
		}
	});
	$('#EstadoCivil').change(function () {
		if ($("#EstadoCivil").val() === "Casado(a)" || $("#EstadoCivil").val() === "União Estável") {
			$("#ClienteRegimeCasamento").show();
			$("#ClienteConjuge_Cpf").show();
			$("#ClienteConjuge_Nome").show();
		} else {
			$("#ClienteRegimeCasamento").hide();
			$("#ClienteConjuge_Cpf").hide();
			$("#ClienteConjuge_Nome").hide();
		}
	});
});