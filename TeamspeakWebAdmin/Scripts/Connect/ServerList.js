var SuccessWindowOpen = 2000;

var ServerList;
var ServerIndex = -1;
var ServerGroupList;
var ChannelGroupList;
var ChannelList;
var ChannelInfo;
var ChannelIndex = -1;
var ClientList;
var ClientIndex = -1;
var temp;

var Codec = ["Speex Narrowband", "Speex Wideband", "Speex Ultrawideband", "CELT Mono"];

var sending = false;

var cl = document.getElementById("ChannelList");

function ServerGroupNameFromId(id) {
    if (ServerGroupList != undefined) {
        for (var i = 0; i < ServerGroupList.length; i++) {
            if (ServerGroupList[i].ServerGroupId == id)
                return ServerGroupList[i].Name;
        }
    }
    return id;
}

function ChannelGroupNameFromId(id) {
    if (ChannelGroupList != undefined) {
        for (var i = 0; i < ChannelGroupList.length; i++) {
            if (ChannelGroupList[i].ChannelGroupId == id)
                return ChannelGroupList[i].Name;
        }
    }
    return id;
}

function toDateTime(secs) {
    var t = new Date(1970, 0, 1);
    t.setSeconds(secs);
    return t;
}

function CollapseUserControls(target) {
    $(".userControls").collapse('hide');
    $("#" + target).collapse('show');
}

function CollapseChannelControls(target){
    $(".channelControl").collapse('hide');
    $("#" + target).collapse('show');
}

function Send(request, re, data, fail) {
    if (!sending) {
        sending = true;
        if (data == undefined)
            data = {};
        data.Guid = guid
        $.post("/A/" + request + "/", data, function (e) {
            sending = false;
            temp = e;
            if (e.Id == e.Message) {
                re(e);
            } else {
                console.log(e);
                fail(e);
            }
        }).fail(function (e) {
            sending = false;
            console.log(e);
            fail(e);
        });
    }
}

function UpdateServerList() {

    Send("ServerList", function (data) {
        ServerList = data;

        var list = $("#ServerList");
        list.empty();

        for (var i = 0; i < data.length; i++) {
            var root = document.createElement("a");
            root.className = "list-group-item";
            root.setAttribute("href", "#");
            root.id = i;
            if (data[i].Status == "online")
                root.innerHTML = '<span class="glyphicon glyphicon-ok" aria-hidden="true"></span>';
            else {
                root.innerHTML = '<span class="glyphicon glyphicon-ok" aria-hidden="true"></span>';
            }

            root.innerHTML += " " + data[i].Name + " " + data[i].Clients + "/" + data[i].MaxClients;

            root.addEventListener("click", ChangeServerSelection);

            list.append(root);
        }
    }, null, function (e) {
        $("#ServerListFailPanel").collapse();
    });
}

function ServerFormClick(e) {
    if (/^\d+$/.test($("#ServerIdInput").val())) {
        Send("SelectServer", function (e) {
            Send("ServerGroupList", function (e) {
                ServerGroupList = e;
                Send("ChannelGroupList", function (e) {
                    ChannelGroupList = e;
                    UpdateChannelList();
                }, null, function (e) {
                    $("#ServerFormClickFail").text(e.Message).collapse('show');
                    setTimeout(function () {
                        $("#ServerFormClickFail").collapse('hide');
                    }, SuccessWindowOpen)
                });
            }, null, function (e) {
                $("#ServerFormClickFail").text(e.Message).collapse('show');
                setTimeout(function () {
                    $("#ServerFormClickFail").collapse('hide');
                }, SuccessWindowOpen)
            });
        }, { Id: $("#ServerIdInput").val() }, function (e) {
            $("#ServerFormClickFail").text(e.Message).collapse('show');
            setTimeout(function () {
                $("#ServerFormClickFail").collapse('hide');
            }, SuccessWindowOpen)
        });
    } else {
        $("#ServerIdInput").val("");
    }
}

function ChangeServerSelection(e) {
    var index = e.srcElement.id;
    if (index == ServerIndex)
        return;
    ServerIndex = index;
    DeselectUser();
    document.getElementById("ServerName").innerText = ServerList[ServerIndex].Name;
    Send("SelectServer", function (e) {
        Send("ServerGroupList", function (e) {
            ServerGroupList = e;
            Send("ChannelGroupList", function (e) {
                ChannelGroupList = e;
                UpdateChannelList();
            });
        });
    }, { Id: ServerList[ServerIndex].Id }, function (e) {
        console.log(e);
    });
}

function UpdateChannelList() {

    DeselectUser();
    DeselectChannel();

    while (cl.firstChild != undefined)
        cl.removeChild(cl.firstChild);

    $("#ChannelList").append($("<p></p>").text("Loading..."));

    Send("ChannelList", function (e) {
        ChannelList = e;

        Send("ClientList", function (d) {
            ClientList = d;

            while (cl.firstChild != undefined)
                cl.removeChild(cl.firstChild);

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
    body.className = "media-body";
    var txt = document.createElement("h4");
    txt.id = "Channel-" + index;
    txt.onclick = SelectChannel;
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
    DeselectChannel();
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
            temp.innerHTML += ServerGroupNameFromId(e.ServerGroupIds[i]);
            if (i + 1 < e.ServerGroupIds.length)
                temp.innerHTML += "<br/>";
        }

        document.getElementById("ChannelGroupLabel").innerText = ChannelGroupNameFromId(e.ChannelGroupId);



        $("#userControlPanel").collapse('show');
    }, { ClientId: ClientList[ClientIndex].ClientId });
}

function DeselectUser() {
    ClientIndex = -1;
    $("#userControlPanel").collapse('hide');
}

function SelectChannel(e) {
    DeselectUser();
    ChannelIndex = e.srcElement.id.split("-")[1];
    document.getElementById("ChannelName").innerText = ChannelList[ChannelIndex].Name;
    Send("ChannelInfo", function (e) {
        ChannelInfo = e;
        $("#ChannelNameLabel").text(e.Name); $("#channelNameInput").val(e.Name);
        if (e.FlagPassword == 1)
            $("#ChannelPasswordLabel").text("True");
        else
            $("#ChannelPasswordLabel").text("False");
        $("#ChannelTopicLabel").text(e.Topic); $("#channelTopicInput").val(e.Topic);
        $("#ChannelDescription").text(e.Description); $("#channelDescriptionInput").val(e.Description);

        $("#channelControlPanel").collapse('show');
    }, { ChannelId: ChannelList[ChannelIndex].ChannelId });
}

function DeselectChannel() {
    ChannelIndex = -1;
    $("#channelControlPanel").collapse("hide");
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

function ChannelUpdateName() {
    var data = { ChannelId: ChannelList[ChannelIndex].ChannelId };
    if ($("#channelNameInput").val() != ChannelInfo.Name)
        data.Name = $("#channelNameInput").val();
    if ($("#channelTopicInput").val() != ChannelInfo.Topic)
        data.Topic = $("#channelTopicInput").val();
    if ($("#channelDescriptionInput").val != ChannelInfo.Description)
        data.Description = $("#channelDescriptionInput").val();
    if (Object.keys(data).length == 1)
        return;

    Send("ChannelEdit", function (e) {
        UpdateChannelList();
    }, data, function (e) {
        $("#UpdateChannelNameError").text(e.Message);
        $("#UpdateChannelNameError").collapse('show');
        setTimeout(function () {
            $("#UpdateChannelNameError").collapse('hide');
        }, SuccessWindowOpen);
    });
}

function ChannelSetPassword() {
    Send("ChannelSetPassword", function (e) {
        $("#channelPasswordInput").val("");
        ShowSuccessMessage("UpdateChannelPassword");
    }, {
        Password: $("#channelPasswordInput").val(),
        ChannelId: ChannelList[ChannelIndex].ChannelId
    }, function (e) {
        $("#UpdateChannelPasswordError").text(e.Message);
        $("#UpdateChannelPasswordError").collapse('show');
        setTimeout(function () {
            $("#UpdateChannelPasswordError").collapse('hide');
        }, SuccessWindowOpen);
    });
}

UpdateServerList();