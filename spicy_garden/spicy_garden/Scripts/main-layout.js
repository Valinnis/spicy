$(document).ready(function () {
	$('#user-func').width($('.width-container').width() - 325);
	$(window).resize(function () {
		$('#user-func').width($('.width-container').width() - 325);
	});
})