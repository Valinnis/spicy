function load() {
	$('.back').each(function () {
		updatePrice($(this));
		disableInput($(this));
	})
}
function disableInput(div) {
	var Id = div.find('input[type="hidden"]');
	var half = div.find('input[type="checkbox"]');
	var option = div.find('.option');
	var sauce = div.find('.sauce');
	var spicelevel = div.find('.spicelevel');
	var quantity = div.find('.quantity-input');
	if (div.find('.add-btn').html() == 'Remove') {
		Id.prop('disabled', true);
		half.prop('disabled', true);
		option.prop('disabled', true);
		sauce.prop('disabled', true);
		spicelevel.prop('disabled', true);
		quantity.prop('disabled', true);
	}
}
function updatePrice(div) {
	$.ajax({
		url: "Order/GetItemPrice",
		type: "GET",
		data: {
			Id: div.find('input[type="hidden"]').val(),
			quantity: div.find('.quantity-input').val(),
			optionId: div.find('.option').val(),
			halfOrder: div.find('input[type="checkbox"]').is(':checked')
		},
		success: function (data) {
			div.find('.price-tag').html('$' + data);
		},
		error: function (xhr, ajaxOptions, thrownError) {
			alert(xhr.responseText);
		}
	});
}
$(document).ready(function () {
	load();
	var total = 0.00;
	var cards = $('.menuitem');
	$('.menuitem').on("click", function (event) {
		if ($(this).hasClass("flipped")){
			$(this).removeClass("flipped");
		}
		else{
			$(this).addClass("flipped");
		}
	});

	function removeFromCart(Id) {
		$.ajax({
			url: "Order/RemoveFromCart",
			type: "POST",
			data: {
				itemId: Id.val()
			},
			success: function (data) {	
				if ($('.' + Id.val() + ' .price').html() != null) {
					total -= parseFloat($('.' + Id.val() + ' .price').html());
					total = parseFloat(total.toFixed(2));
					$('.' + Id.val()).remove();
					$('.total-price').html('$' + (total*1.05).toFixed(2));
				}
				// To Do: complete this code
			},
			error: function (xhr, ajaxOptions, thrownError) {
				alert(xhr.responseText);
			}
		});
	}
	function addToCart(Id, half, option, sauce, spicelevel, quantity) {
		$.ajax({
			url: "Order/AddToCart",
			type: "POST",
			data: {
				itemId: Id.val(),
				halfOrder: half.is(':checked'),
				optionId: option.val(),
				sauce: sauce.val(),
				spiceLevel: spicelevel.val(),
				quantity: quantity.val()
			},
			success: function (data) {
				$('#cart').append(data);
				total += parseFloat($('.' + Id.val() + ' .price').html());
				total = parseFloat(total.toFixed(2));
				$('.total-price').html('$' + (total*1.05).toFixed(2));
				// To Do: complete this code
			},
			error: function (xhr, ajaxOptions, thrownError) {
				alert(xhr.responseText);
			}
		});
	}

	$('.add-btn').on("click", function (event) {
		try {
			var Id = $(this).parent().find('input[type="hidden"]');
			var half = $(this).parent().find('input[type="checkbox"]');
			var option = $(this).parent().find('.option');
			var sauce = $(this).parent().find('.sauce');
			var spicelevel = $(this).parent().find('.spicelevel');
			var quantity = $(this).parent().find('.quantity-input');

			if ($(this).html() == 'Add to Cart') {
				if (!isNaN(quantity.val())) {
					addToCart(Id, half, option, sauce, spicelevel, quantity);
					Id.prop('disabled', true);
					half.prop('disabled', true);
					option.prop('disabled', true);
					sauce.prop('disabled', true);
					spicelevel.prop('disabled', true);
					quantity.prop('disabled', true);
					$(this).html('Remove');
				}
				else {
					quantity.val(0);
					return false;
				}
			}
			else {
				removeFromCart(Id);
				Id.prop('disabled', false);
				half.prop('disabled', false);
				option.prop('disabled', false);
				sauce.prop('disabled', false);
				spicelevel.prop('disabled', false);
				quantity.prop('disabled', false);
				$(this).html('Add to Cart');
			}
		}
		catch (e) {
			return false;
		}
	})
	$('.half-check').on("click", function () {
		updatePrice($(this).parent().parent());
	})
	$('.option').on("change", function () {
		updatePrice($(this).parent().parent());
	})
	$('.quantity-input').on('input', function () {
		if (!isNaN($(this).val()) && $(this).val() != ""){
			updatePrice($(this).parent());
		}
	})
	$('.dropdown_menu').click(function (event) {
		event.stopPropagation();
	});
	$('input').click(function (event) {
		event.stopPropagation();
	});
	$('button').click(function (event) {
		event.stopPropagation();
	})
})