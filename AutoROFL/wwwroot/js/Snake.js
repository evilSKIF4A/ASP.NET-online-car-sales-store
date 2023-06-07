
const canvas = document.getElementById("Snake");
const context = canvas.getContext("2d");

const box = 16;
var speed = 0;

const snake = {
    x: 160,
    y: 160,
    dx: box,
    dy: 0,
    tail: [],
    startTail: 4
};

const eat = {
    x: getRandomInt(0, 25) * box,
    y: getRandomInt(0, 25) * box
}

function getRandomInt(min, max) {
    return Math.floor(Math.random() * (max - min)) + min;
}

function game() {
    requestAnimationFrame(game);
    if (++speed < 6) {
        return;
    }
    speed = 0;
    context.clearRect(0, 0, canvas.width, canvas.height);
    snake.x += snake.dx;
    snake.y += snake.dy;
    if (snake.x < 0) {
        snake.x = canvas.width - box;
    }
    else if (snake.x >= canvas.width) {
        snake.x = 0;
    }

    if (snake.y < 0) {
        snake.y = canvas.height - box;
    }
    else if (snake.y >= canvas.height) {
        snake.y = 0;
    }

    snake.tail.unshift({ x: snake.x, y: snake.y });
    if (snake.tail.length > snake.startTail) {
        snake.tail.pop();
    }

    context.fillStyle = 'red';
    context.fillRect(eat.x, eat.y, box - 1, box - 1);

    context.fillStyle = 'green';
    snake.tail.forEach(function (tail, index) {
        context.fillRect(tail.x, tail.y, box - 1, box - 1);
        if (tail.x === eat.x && tail.y === eat.y) {
            snake.startTail++;
            eat.x = getRandomInt(0, 25) * box;
            eat.y = getRandomInt(0, 25) * box;
        }
        for (var i = index + 1; i < snake.tail.length; i++) {
            if (tail.x === snake.tail[i].x && tail.y === snake.tail[i].y) {
                snake.x = 160;
                snake.y = 160;
                snake.tail = [];
                snake.startTail = 4;
                snake.dx = box;
                snake.dy = 0;
                eat.x = getRandomInt(0, 25) * box;
                eat.y = getRandomInt(0, 25) * box;
            }
        }
    });
}

document.addEventListener('keydown', function (e) {
    // стрелка влево
    if (e.which === 37 && snake.dx === 0) {
        snake.dx = -box;
        snake.dy = 0;
    }
    // Стрелка вверх
    else if (e.which === 38 && snake.dy === 0) {
        snake.dy = -box;
        snake.dx = 0;
    }
    // Стрелка вправо
    else if (e.which === 39 && snake.dx === 0) {
        snake.dx = box;
        snake.dy = 0;
    }
    // Стрелка вниз
    else if (e.which === 40 && snake.dy === 0) {
        snake.dy = box;
        snake.dx = 0;
    }
});

requestAnimationFrame(game);
