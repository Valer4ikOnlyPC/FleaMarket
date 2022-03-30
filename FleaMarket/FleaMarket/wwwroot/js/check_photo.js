function photo_check_extension() {
    var data = new FormData($("#formAdd")[0]);
    $.ajax({
        type: "POST",
        url: "/Product/PhotoCheck",
        data: data,
        contentType: false,
        processData: false,
        beforeSend: function () {
            $('#results').show();
        }
    })
        .done(function (msg) {
            $('#results').html(msg);
        });
}



function category_extension(categoryId) {
    $.post("/Product/CategoryByParent", { CategoryId: categoryId })
        .done(function (msg) {
            if (msg != "") {
                $('#' + categoryId).html(msg);
            }
            else {
                $('#' + categoryId).addClass('non_visibility');
            }
            
        });
    $('#CategoryId').val(categoryId);
}
function category_select(categoryId) {
    $('#CategoryId').val(categoryId);
}


function edit_product() {
    var data = new FormData($('#form1')[0]);
    $.ajax({
        type: "POST",
        url: "UpdatePhoto",
        data: data,
        contentType: false,
        processData: false,
        beforeSend: function () {
            $('#deal').show();
        }
    })
        .done(function (msg) {
            $('#deal').html(msg);
        });
}

function delete_photo(productId, photoId) {
    $.get("/Product/DeletePhoto", { ProductId: productId, PhotoId: photoId })
        .done(function (msg) {
            $('#deal').html(msg);
        });
}