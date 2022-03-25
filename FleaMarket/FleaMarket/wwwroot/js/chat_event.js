const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/chat")
    .build();
var NoRead = 0;
hubConnection.on('SendMessage', function (dialogId) {
    var elem = document.getElementById(dialogId);
    if (elem.classList.contains('active')) {
        $.get("/Dialog/ReadMessage", { dialogId: dialogId });
        $.get("/Dialog/ViewDialog", { dialogId: dialogId })
            .done(function (msg) {
                $('#SelectedChat').html(msg);
                lastMessageScroll('smooth');
                NoRead -= 1;
                BadgesVisually();
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

function BadgesVisually() {
    var badges = document.getElementById('badges');
    if (NoRead > 0) {
        badges.style.visibility = "visible";
    }
    else {
        badges.style.visibility = "hidden";
    }
}