function getDate() {
    var date = new Date();

    $('#timedisplay').html("Сегодня: " + date.toLocaleString());
}

setInterval(getDate, 0);