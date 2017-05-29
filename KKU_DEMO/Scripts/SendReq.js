function SendReq(url,param,method) {
    $.ajax({
        'url': url + param,
        'type': method
       
        
    });

}
