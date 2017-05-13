function getDate() {
    var date = new Date();

    document.getElementById('timedisplay').innerHTML = "Сегодня: " + date.toLocaleString();
}

setInterval(getDate, 0);