var tg = document.getElementById('tg');
var isClicked = false;

tg.addEventListener('click', function () {
    if (isClicked) {
        tg.style.left = '-1px';
        isClicked = false;
    } else {
        tg.style.left = '249px';
        isClicked = true;
    }
});