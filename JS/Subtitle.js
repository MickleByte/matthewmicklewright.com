var Subtitles = [
	'I have too much time on my hands....',
	'Micklepussy.net was already taken....',
	'Hello There',
	'Hi Mum',
	'bet you were hoping for a 404 error',
	'Did you remeber to lock your front door?'
	];
				
document.getElementById("SubTitle").innerHTML = Subtitles[Math.round(Math.random()*Subtitles.length)];
console.log("Success");