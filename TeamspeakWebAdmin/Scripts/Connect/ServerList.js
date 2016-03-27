﻿$.post("/A/ServerList/", { Guid: guid }, UpdateServerList);

var SuccessWindowOpen = 2000;

var ServerList;
var ServerIndex;
var ChannelList;
var ClientList;
var ClientIndex;

var sending = false;

var cl = document.getElementById("ChannelList");

function toDateTime(secs) {
    var t = new Date(1970, 0, 1);
    t.setSeconds(secs);
    return t;
}

function CollapseUserControls(target) {
    $(".userControls").collapse('hide');
    $("#" + target).collapse('show');
}

function Send(request, re, data) {
    if (!sending) {
        sending = true;
        if (data == undefined)
            data = {};
        data.Guid = guid
        $.post("/A/" + request + "/", data, function (e) {
            sending = false;
            re(e);
        }).fail(function (e) {
            console.log("Error");
            console.log(e);
        });
    }
}

function UpdateServerList(data) {

    ServerList = data;

    var list = $("#ServerList");
    list.empty();

    for (var i = 0; i < data.length; i++) {
        var root = document.createElement("a");
        root.className = "list-group-item";
        root.setAttribute("href", "#");
        root.id = i;
        if(data[i].Status == "online")
            root.innerHTML = '<span class="glyphicon glyphicon-ok" aria-hidden="true"></span>';
        else {
            root.innerHTML = '<span class="glyphicon glyphicon-ok" aria-hidden="true"></span>';
        }

        root.innerHTML += " " + data[i].Name + " " + data[i].Clients + "/" + data[i].MaxClients;

        root.addEventListener("click", ChangeServerSelection);

        list.append(root);
    }
}

function ChangeServerSelection(e) {
    var index = e.srcElement.id
    if (index == ServerIndex)
        return;
    ServerIndex = index;
    DeselectUser();
    document.getElementById("ServerName").innerText = ServerList[ServerIndex].Name;
    Send("SelectServer", function (e) {
        UpdateChannelList();
    }, { Id: ServerList[ServerIndex].Id });
}

function UpdateChannelList() {

    while (cl.firstChild != undefined)
        cl.removeChild(cl.firstChild);

    Send("ChannelList", function (e) {
        ChannelList = e;

        Send("ClientList", function (d) {
            ClientList = d;

            for (var i = 0; i < ChannelList.length; i++) {

                $("#moveToIndex").append($("<option></option>").attr("value", i).text(ChannelList[i].Name));

                if (ChannelList[i].ParentId == 0)
                    cl.appendChild(CreateChannelTree(i));
            }
            $("#serverWindow").collapse('show');
        });
    });
}

function CreateChannelTree(index) {
    var root = document.createElement("li");
    root.className = "media";
    var mediaEl = document.createElement("div");
    mediaEl.className = "media-left";
    var icon = document.createElement("span");
    icon.classList.add("glyphicon");
    icon.classList.add("glyphicon-menu-down");
    icon.classList.add("media-object");
    icon.setAttribute("aria-hidden", "true");
    mediaEl.appendChild(icon);
    
    var body = document.createElement("div");
    body.id = "Channel-" + index;
    body.className = "media-body";
    var txt = document.createElement("h4");
    txt.innerText = ChannelList[index].Name;
    txt.className = "media-heading";
    body.appendChild(txt);

    var kidlistwrap = document.createElement("li");
    kidlistwrap.className = "media";

    var kidlist = document.createElement("ul");
    kidlist.className = "media-list";
    
    for (var i = 0; i < ClientList.length; i++) {
        if (ClientList[i].ChannelId == ChannelList[index].ChannelId)
            kidlist.appendChild(CreateUser(i));
    }

    for (var i = 0; i < ChannelList.length; i++) {
        if (i == index)
            continue;
        else if (ChannelList[i].ParentId == ChannelList[index].ChannelId) {
            kidlist.appendChild(CreateChannelTree(i));
        }
    }

    if (kidlist.childNodes.length > 0) {
        kidlistwrap.appendChild(kidlist);
        body.appendChild(kidlistwrap);
    }

    root.appendChild(mediaEl);
    root.appendChild(body);

    return root;
}

function CreateUser(index) {

    if (index > ClientList.length)
        return null;

    var root = document.createElement("li");
    root.className = "media";
    var mediaEl = document.createElement("div");
    mediaEl.className = "media-left";
    var icon = document.createElement("span");
    icon.classList.add("glyphicon");
    icon.classList.add("glyphicon-user");
    icon.classList.add("media-object");
    icon.setAttribute("aria-hidden", "true");
    mediaEl.appendChild(icon);

    var body = document.createElement("a");
    body.setAttribute("href", "#userControlPanel");
    body.className = "media-body";
    var txt = document.createElement("h4");
    txt.id = "Client-" + index;
    txt.addEventListener("click", SelectUser);
    txt.innerText = ClientList[index].Name;
    txt.className = "media-heading";
    body.appendChild(txt);

    root.appendChild(mediaEl);
    root.appendChild(body);

    return root;
}

function SelectUser(e) {
    ClientIndex = e.srcElement.id.split("-")[1];
    document.getElementById("ClientName").innerText = ClientList[ClientIndex].Name;
    Send("ClientInfo", function (e) {
        document.getElementById("NameLabel").innerText = e.Name;
        document.getElementById("UniqueIdLabel").innerText = e.UniqueId;
        document.getElementById("VersionLabel").innerText = e.Version;
        document.getElementById("IPLabel").innerText = e.IP;
        document.getElementById("ConnectionsLabel").innerText = e.TotalConnections;
        document.getElementById("FirstConnectedLabel").innerText = toDateTime(e.ClientCreated).toLocaleString();
        document.getElementById("LastConnectedLabel").innerText = toDateTime(e.ClientLastConnected).toLocaleString();
        document.getElementById("IdleTimeLabel").innerText = e.IdleTime;

        var temp = document.getElementById("ServerGroupsLabel");
        temp.innerHTML = "";
        for (var i = 0; i < e.ServerGroupIds.length; i++) {
            temp.innerHTML += e.ServerGroupIds[i];
            if (i + 1 < e.ServerGroupIds.length)
                temp.innerHTML += "<br/>";
        }



        $("#userControlPanel").collapse('show');
    }, { ClientId: ClientList[ClientIndex].ClientId });
}

function DeselectUser() {
    ClientIndex = -1;
    $("#userControlPanel").collapse('hide');
}

function ShowSuccessMessage(cmd) {
    $("#"+cmd+"Success").collapse('show');
    setTimeout(function () {
        $("#" + cmd + "Success").collapse('hide');
    }, SuccessWindowOpen)
}

function Poke() {
    Send("Poke", function (e) {
        if (e == "Success") {
            $("#pokeSuccess").collapse('show');
            setTimeout(function () {
                $("#pokeSuccess").collapse('hide');
            },SuccessWindowOpen)
        }
    }, { Text: document.getElementById("pokeValue").value, ClientId: ClientList[ClientIndex].ClientId });
    document.getElementById("pokeValue").value = "";
}

function Kick() {
    Send("Kick", function (e) {
        if (e == "Success") {
            ShowSuccessMessage("kick");
        }
        UpdateChannelList();
    }, { Text: document.getElementById("kickValue").value, ClientId: ClientList[ClientIndex].ClientId, Reasonid: $("input[name=optradio]:checked").val() });
    document.getElementById("kickValue").value = "";
}

function Ban() {
    var time;
    if($("input[name=optradio]:checked").val() == "-1"){
        time = -1;
    } else {
        time = document.getElementById("banTime").value;
    }
    Send("Ban", function (e) {
        if (e == "Success") {
            ShowSuccessMessage("ban");
        }
        UpdateChannelList();
    }, { Text: document.getElementById("banReason").value, ClientId: ClientList[ClientIndex].ClientId, Time: time });
}

function Move() {
    var cid = ChannelList[document.getElementById("moveToIndex").value].ChannelId;
    var clid = ClientList[ClientIndex].ClientId;

    Send("Move", function (e) {
        if (e == "Success") {
            ShowSuccessMessage("move");
        }
        UpdateChannelList();
    }, { ClientId: clid, ChannelId: cid });
}