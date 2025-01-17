﻿"use strict";
//
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/index").build();

document.getElementById("registerButton").disabled = true;
document.getElementById("sendButton").disabled = true;

connection.start().then(function () {
    document.getElementById("registerButton").disabled = false;
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("ReceiveMessage", function (received) {
    var li = document.createElement("li");
    document.getElementById("messages").appendChild(li);

    li.textContent =
        `${received.from}: @${received.to} ${received.body}`;
});

document.getElementById("registerButton").addEventListener("click", function (event) {
    var registermodel = {
        username: document.getElementById("from").value,
        groups: document.getElementById("groups").value
    };
    connection.invoke("Register", registermodel).catch(function (err) {
        return console.error(err.toString());
    });

    event.preventDefault();
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var messageToSend = {
        to: document.getElementById("to").value,
        toType: document.getElementById("toType").value,
        from: document.getElementById("from").value,
        body: document.getElementById("body").value
    };

    connection.invoke("SendMessage", messageToSend).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});
//


