const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/chat")
    .build();
hubConnection.on('SendMessage', function (dialogId) {
    var elem = document.getElementById("offcanvasRight");
    if (!elem.classList.contains('show')) {
        $.get("/Dialog/CountNewDialogs", {})
            .done(function (countDialog) {
                if (countDialog == 0) {
                    document.getElementById("MyChat").style.visibility = "hidden";
                }
                else {
                    document.getElementById("MyChat").style.visibility = "visible";
                    document.getElementById("MyChat").textContent = String(countDialog);
                }
            });
    }

    var elem = document.getElementById(dialogId);
    if (elem.classList.contains('active')) {
        $.get("/Dialog/ReadMessage", { dialogId: dialogId });
        $.get("/Dialog/ViewDialog", { dialogId: dialogId })
            .done(function (msg) {
                $('#SelectedChat').html(msg);
                lastMessageScroll('smooth');
            });
    }
    else {
        var elem1 = document.getElementById("_" + dialogId);
        elem1.style.visibility = "visible";
    }
});
hubConnection.on('ReadMessage', function (dialogId) {
    var elem = document.getElementById(dialogId);
    if (elem.classList.contains('active')) {
        $.get("/Dialog/ViewDialog", { dialogId: dialogId })
            .done(function (msg) {
                $('#SelectedChat').html(msg);
                lastMessageScroll('smooth');
            });
    }
});
hubConnection.start();