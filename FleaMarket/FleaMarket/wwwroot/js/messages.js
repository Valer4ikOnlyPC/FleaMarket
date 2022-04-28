$.get("/Dialog/CountNewDialogs", {})
    .done(function (countDialog) {
        if (Number(countDialog) == 0) {
            document.getElementById("MyChat").style.visibility = "hidden";
        }
        else {
            document.getElementById("MyChat").style.visibility = "visible";
            document.getElementById("MyChat").textContent = String(countDialog);
        }
    });
$.get("/Home/GetCountNewDeals", {})
    .done(function (msg) {
        if (msg == 0) {
            document.getElementById("CountNewDeals").style.visibility = 'hidden';
        }
        else {
            document.getElementById("CountNewDeals").style.visibility = 'visible';
            $('#CountNewDeals').html(msg);
        }
    });

function DeActive() {
    $.get("/Dialog/CountNewDialogs", {})
        .done(function (countDialog) {
            if (Number(countDialog) == 0) {
                document.getElementById("MyChat").style.visibility = "hidden";
            }
            else {
                document.getElementById("MyChat").style.visibility = "visible";
                document.getElementById("MyChat").textContent = String(countDialog);
            }
        });
    let elements = document.getElementsByClassName("openDialog");
    for (let elem of elements) {
        elem.classList.remove("active");
    }
}
function chat_create(userId) {
    $.get("/Dialog/CreateDialog", { userId: userId })
        .done(function (dialogId) {
            $.get("/Dialog/AllDialog", {})
                .done(function (msg) {
                    $('#AllChat').html(msg);
                    open_chat(dialogId);
                });
        });
}
function chat_active() {
    $.get("/Dialog/AllDialog", {})
        .done(function (msg) {
            $('#AllChat').html(msg);
        });
    $('#SelectedChat').html('<div class="h-100 position-relative pb-5"><p class="position-absolute pb-5 top-50 start-50 translate-middle text-center text-secondary">Выберите чат</p></div>');
}
function open_chat(dialogId) {
    var elem1 = document.getElementById("_" + dialogId);
    elem1.style.visibility = "hidden";

    $.get("/Dialog/ReadMessage", { dialogId: dialogId });
    $.get("/Dialog/ViewDialog", { dialogId: dialogId })
        .done(function (msg) {
            $('#SelectedChat').html(msg);
            let elements = document.getElementsByClassName("openDialog");
            for (let elem of elements) {
                elem.classList.remove("active");
            }
            document.getElementById(dialogId).classList.add("active");
            document.getElementById('d_' + dialogId).classList.add("active");
            lastMessageScroll('smooth');
        });
}
function blocked_chat(dialogId) {
    $.get("/Dialog/BlockedDialog", { dialogId: dialogId })
        .done(function () {
            chat_active();
        });
}

function add_message(e, dialogId) {
    e.preventDefault();
    var data = new FormData($("#messageForm")[0]);
    var text = data.get("text");
    if (text == "" | text == null) return;

    $.ajax({
        type: "POST",
        url: "/Dialog/AddMessage",
        data: data,
        contentType: false,
        processData: false,
        beforeSend: function () {
            $('#SelectedChat').show();
        }
    })
        .done(function (msg) {
            $('#SelectedChat').html(msg);
            lastMessageScroll('smooth');
        });
}
function lastMessageScroll(b) {
    var e = document.querySelector('.wrapper_Scrollbottom');
    if (!e) return;

    e.scrollIntoView({
        block: 'end',
    });
}