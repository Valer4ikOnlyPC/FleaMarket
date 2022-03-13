var textarea = document.querySelector('textarea');
textarea.style.height = textarea.scrollHeight + "px";

function deal_extension(productId) {
    document.getElementById("deal").style.visibility = 'visible';
    var data = { ProductId: productId }
    $.get("/Deal/DealOffer", { ProductId: productId })
        .done(function (msg) {
            $('#deal').html(msg);
        });
}
function Close_deal() {
    document.getElementById("deal").style.visibility = 'hidden';
}
function create_deal(productMaster, productRecipient, userRecipient) {
    var data = { productMaster: productMaster, productRecipient: productRecipient, userRecipient: userRecipient }
    $.get("/Deal/CreateDeal", { productMaster: productMaster, productRecipient: productRecipient, userRecipient: userRecipient })
        .done(function (msg) {
            if (msg != "") {
                alert(msg);
            }
            Close_deal();
        });
}
function edit_extension(productId) {
    document.getElementById("deal").style.visibility = 'visible';
    var data = { ProductId: productId }
    $.get("/Product/EditProduct", { ProductId: productId })
        .done(function (msg) {
            $('#deal').html(msg);
        });
}