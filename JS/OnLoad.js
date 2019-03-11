window.onscroll = function() {scrollFunc()};		


function openTab(evt, tabName) {
	var i, tabcontent, tablinks;
	tabcontent = document.getElementsByClassName("tabcontent");
	for (i = 0; i < tabcontent.length; i++) {
		tabcontent[i].style.display = "none";
	}
	tablinks = document.getElementsByClassName("tablinks");
	for (i = 0; i < tablinks.length; i++) {
		tablinks[i].className = tablinks[i].className.replace(" active", "");
	}
	document.getElementById(tabName).style.display = "block";
	evt.currentTarget.className += " active";
}

function scrollFunc() {
	var navbar = document.getElementById("navbar");
	var sticky = navbar.offsetTop;
	if (window.pageYOffset >= sticky) {
		navbar.classList.add("sticky")
	} 
	 else {
		navbar.classList.remove("sticky");
	}
}