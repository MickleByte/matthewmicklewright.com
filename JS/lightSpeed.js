var canvElem = document.getElementById("canvas");
var offsetX = canvElem.getBoundingClientRect().left;
var offsetY = canvElem.getBoundingClientRect().top;
var ctx = canvElem.getContext("2d");
ctx.canvas.width  = window.innerWidth;
ctx.canvas.height = window.innerHeight;

var particles = [];
var x = ctx.canvas.width / 2, y = ctx.canvas.height / 2;
var pixelWidth = 3;
var clearCounter = 0;
var UpperInterval = 10, LowerInterval = -UpperInterval;
var timePeriod = 8;

window.onresize = function(){ location.reload(); }

clear();
setInterval(drawCube, timePeriod);


function drawCube(){
    if (clearCounter == 10){
        clear();
        clearCounter = 0;
    }
    else clearCounter++;
    

    particles.push([x, y, randomIntFromInterval(LowerInterval, UpperInterval) , randomIntFromInterval(LowerInterval, UpperInterval)]);

    ctx.fillStyle = "#FFFFFF";

    for (var i = 0; i < particles.length; i++){
        ctx.fillRect(particles[i][0], particles[i][1], pixelWidth, pixelWidth);
        particles[i][0] += particles[i][2];
        particles[i][1] += particles[i][3]
    }
    
}

function clear(){
    ctx.fillStyle = "#000000";
    ctx.fillRect(0,0, ctx.canvas.width, ctx.canvas.width);
}

function randomIntFromInterval(min, max) { // min and max included
    return Math.floor(Math.random() * (max - min + 1) + min);
  }