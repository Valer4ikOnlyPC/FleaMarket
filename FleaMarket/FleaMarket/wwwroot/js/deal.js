function deal_extension(DealId) {
    document.getElementById("alert").style.visibility = 'visible';
    var data = { dealId: DealId }
    $.get("/Deal/ViewDetails", { DealId: DealId })
        .done(function (msg) {
            $('#alert').html(msg);
        });
}
function Close_deal() {
    document.getElementById("alert").style.visibility = 'hidden';
}
function rating_extension() {
    var data = new FormData($('#form1')[0]);
    $.ajax({
        type: "POST",
        url: "ViewDetails",
        data: data,
        contentType: false,
        processData: false,
        beforeSend: function () {
            $('#form1').show();
        }
    });
    Close_deal();
}