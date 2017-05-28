function info(id, url) {
    onSBegin();
    $.ajax({
        'url': url+id,
        'type': 'GET',
        success: function (html) {
            $("#Details").html(html);
            onSComplete();
        }
    });

}
function edit(id, url) {
    $.ajax({
        'url': url + id,
        'type': 'GET',
        success: function (html) {
            $("#myModal").html(html);

        }
    });

}


    function onSBegin() {
        $("#Details").html("");
        $("#DetailsSpiner").show();
    }

    function onSComplete() {
    $("#DetailsSpiner").hide();
}
