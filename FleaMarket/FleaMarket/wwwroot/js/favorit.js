function heart_visible(ProductId) {
    var elem = document.getElementById("heart_" + ProductId);
    var elem_min = document.getElementById("heart-min_" + ProductId);
    check_favorit(ProductId);
    elem.style.visibility = 'visible'
    elem_min.style.visibility = 'visible'
}
function heart_anvisible(ProductId) {
    var elem = document.getElementById("heart_" + ProductId);
    var elem_min = document.getElementById("heart-min_" + ProductId);
    elem.style.visibility = 'hidden'
    elem_min.style.visibility = 'hidden'
}
function check_favorit(ProductId) {
    $.get("/Product/CheckFavorite", { productId: ProductId })
        .done(function (result) {
            if (result) {
                var elem = document.getElementById("heart_" + ProductId);
                elem.classList.remove("text-secondary");
                elem.classList.add("text-danger");
                var elem_min = document.getElementById("heart-min_" + ProductId);
                elem_min.classList.remove("text-secondary");
                elem_min.classList.add("text-danger");
            }
            else {
                var elem = document.getElementById("heart_" + ProductId);
                elem.classList.remove("text-danger");
                elem.classList.add("text-secondary");
                var elem_min = document.getElementById("heart-min_" + ProductId);
                elem_min.classList.remove("text-danger");
                elem_min.classList.add("text-secondary");
            }
        });
}
function favorit_add(e, ProductId) {
    e.preventDefault();
    $.get("/Product/AddFavorite", { productId: ProductId })
        .done(function () {
            var elem = document.getElementById("heart_" + ProductId);
            if (elem.classList.contains("text-secondary")) {
                elem.classList.remove("text-secondary");
                elem.classList.add("text-danger");
            }
            else {
                elem.classList.remove("text-danger");
                elem.classList.add("text-secondary");
            }
            var elem_min = document.getElementById("heart-min_" + ProductId);
            if (elem_min.classList.contains("text-secondary")) {
                elem_min.classList.remove("text-secondary");
                elem_min.classList.add("text-danger");
            }
            else {
                elem_min.classList.remove("text-danger");
                elem_min.classList.add("text-secondary");
            }
        });
}
function favorit_list_add(e, ProductId) {
    e.preventDefault();
    $.get("/Product/AddFavorite", { productId: ProductId })
        .done(function () {
            var elem = document.getElementById("heartList_" + ProductId);
            if (elem.classList.contains("text-secondary")) {
                elem.classList.remove("text-secondary");
                elem.classList.add("text-danger");
            }
            else {
                elem.classList.remove("text-danger");
                elem.classList.add("text-secondary");
            }
        });
}
function favorite_extension() {
    $('#favoriteToList').append('<div class="h-100 position-relative"><div class="position-absolute start-50 translate-middle text-center text-secondary my-3"><div class="spinner-border text-danger" role="status"><span class="visually-hidden"> Loading...</span ></div></div></div>');
    $.get("/Product/FavoriteToList", {})
        .done(function (msg) {
            $('#favoriteToList').html(msg);
        });
}


function Product_extension() {
    document.getElementById("addProduct").style.visibility = 'visible';
    $.get("/Product/AddProduct", {})
        .done(function (msg) {
            $('#addProduct').html(msg);
        });
}
function Close_product() {
    document.getElementById("addProduct").style.visibility = 'hidden';
}