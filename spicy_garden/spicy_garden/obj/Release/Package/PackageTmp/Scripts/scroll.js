$(document).ready(function () {
	var main = $('.main').position();
	var x = $('.main');
	if ($(x).width() <= 920) {
		var newItemWidth = (x.width() - 40) / 2;
		if (newItemWidth >= 380) {
			$('.menuitem').width(newItemWidth - 0.5);
		}
	}
	else {
		var newItemWidth = (x.width() - 40) / 3;
		$('.menuitem').width(newItemWidth - 0.5);
	}
	$('#user-func').width(x.width() - 326);
	if (main.left <= 195) {
		$('.cat-container').css({
			'left': 0,
			'width': Math.min(main.left, 175)
		});
	}
	else {
		$('.cat-container').css({
			'width': 175 + 'px',
			'left': parseFloat(main.left) - $('.cat-container').width() - 20,
		});
	}
	$(window).scroll(function () {
		$('.cat-container').css({
			'top': $(this).scrollTop() + 175 + 'px'
		});
	});
	$(window).resize(function () {
		var main = $('.main');
		var x = main.position();
		if (x.left <= 195) {
			$('.cat-container').css({
				'left': 0,
				'width': Math.min(x.left, 175)
			});
		}
		else {
			$('.cat-container').css({
				'width': 175 + 'px',
				'left': parseFloat(x.left) - $('.cat-container').width() - 20,
			});
		}
		if ($(main).width() <= 920) {
			var newItemWidth = (main.width() - 40) / 2;
			if (newItemWidth >= 380) {
				$('.menuitem').width(newItemWidth-20.5);
			}
		}
		else {
			var newItemWidth = (main.width() - 40) / 3;
			$('.menuitem').width(newItemWidth-20.5);
		}
		$('#user-func').width(main.width() - 326);
	});
})