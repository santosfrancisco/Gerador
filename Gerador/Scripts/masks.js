
jQuery(function ($) {
	$(".data").mask("99/99/9999");
	$(".telefone").mask("(99)999999999");
	$(".moeda").mask("###0,00", {reverse: true});
});
