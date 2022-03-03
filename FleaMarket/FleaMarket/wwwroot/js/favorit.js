function heart_visible(ProductId) {
    var elem = document.getElementById("heart_" + ProductId);
    check_favorit(ProductId);
    elem.style.visibility = 'visible'
}
function heart_anvisible(ProductId) {
    var elem = document.getElementById("heart_" + ProductId);
    elem.style.visibility = 'hidden'
}
function check_favorit(ProductId) {
    $.get("/Product/CheckFavorite", { productId: ProductId })
        .done(function (result) {
            if (result) {
                var elem = document.getElementById("heart_" + ProductId);
                elem.classList.remove("text-secondary");
                elem.classList.add("text-danger");
            }
            else {
                var elem = document.getElementById("heart_" + ProductId);
                elem.classList.remove("text-danger");
                elem.classList.add("text-secondary");
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
    $.get("/Product/FavoriteToList", {})
        .done(function (msg) {
            $('#favoriteToList').html(msg);
        });
}