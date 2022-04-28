var dialogId = document.getElementById('_dialogId').value;
var scrol = document.getElementById('textElem');
scrol.addEventListener('scroll', function () {
    let obj = document.getElementById('chatTop');
    let { top, bottom } = obj.getBoundingClientRect();
    let height = document.documentElement.clientHeight;
    if (top > 62) {
        var numberPage = Number(obj.textContent) + 1;
        $.get("/Dialog/ViewDialog", { dialogId: dialogId, pageNumber: numberPage })
            .done(function (msg) {
                $('#SelectedChat').html(msg);
                scrol.scrollTop = height * 2;
            });
    }
});