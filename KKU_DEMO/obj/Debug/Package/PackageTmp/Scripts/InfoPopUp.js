function info(id,url) {
    $.ajax({
        'url': url+id,
        'type': 'GET',
        success: function (html) {
            $("#Details").html(html);

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