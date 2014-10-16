//删除购物车中商品
function delCartProduct(pid) {
    Ajax.get("/Cart/DelPruduct?pid=" + pid, false, function (data) {
        try {
            alert(val("(" + data + ")").content);
        }
        catch (ex) {
            document.getElementById("cartBody").innerHTML = data;
        }
    })
}

//删除购物车中套装
function delCartSuit(pmId) {
    Ajax.get("/Cart/DelSuit?pmId=" + pmId, false, function (data) {
        try {
            alert(eval("(" + data + ")").content);
        }
        catch (ex) {
            document.getElementById("cartBody").innerHTML = data;
        }
    })
}

//删除购物车中满赠
function delCartFullSend(pmId) {
    Ajax.get("/Cart/DelFullSend?pmId=" + pmId, false, function (data) {
        try {
            alert(eval("(" + data + ")").content);
        }
        catch (ex) {
            document.getElementById("cartBody").innerHTML = data;
        }
    })
}

//清空购物车
function clearCart() {
    Ajax.get("/Cart/Clear", false, function (data) {
        var result = eval("(" + data + ")");
        if (result.state == "nologin") {
            alert("请先登录");
        }
        else if (result.state == "success") {
            document.getElementById("cartBody").innerHTML = "";
        }
        else {
            alert("清空购物车失败");
        }
    })
}

//改变商品数量
function changePruductCount(pid, buyCount) {
    if (isNaN(buyCount)) {
        alert('请输入数字');
        return false;
    }
    Ajax.get("/Cart/ChangePruductCount?pid=" + pid + "&buyCount=" + buyCount, false, function (data) {
        try {
            alert(eval("(" + data + ")").content);
        }
        catch (ex) {
            document.getElementById("cartBody").innerHTML = data;

        }
    })
}

//改变套装数量
function changeSuitCount(pmId, buyCount) {
    if (isNaN(buyCount)) {
        alert('请输入数字');
        return false;
    }
    Ajax.get("/Cart/ChangeSuitCount?pmId=" + pmId + "&buyCount=" + buyCount, false, function (data) {
        try {
            alert(eval("(" + data + ")").content);
        }
        catch (ex) {
            document.getElementById("cartBody").innerHTML = data;
        }
    })
}

//获取满赠商品
function getFullSend(pmId) {
    Ajax.get("/Cart/GetFullSend?pmId=" + pmId, false, function (data) {
        getFullSendResponse(data, pmId);
    })
}

//处理获取满赠商品的反馈信息
function getFullSendResponse(data, pmId) {
    var result = eval("(" + data + ")");
    if (result.content != undefined) {
        alert(result.content);
    }
    else {
        if (result.length < 1) {
            alert("满赠商品不存在");
            return;
        }
        var listHtml = "";
        for (var i = 0; i < result.length; i++) {
            listHtml += "<div class=\"fullSendProduct\">" + result[i].Name + "</div><div class=\"addFullSendBut\" onclick=\"addFullSend(" + pmId + "," + result[i].Pid + ")\">选择</div><div style=\" clear:both;\"></div>";
        }
        document.getElementById("fullSendProductList" + pmId).innerHTML = listHtml;
        document.getElementById("fullSendBlock" + pmId).style.display = "block";
    }
}

//关闭满赠层
function closeFullSendBlock(pmId) {
    document.getElementById("fullSendProductList" + pmId).innerHTML = "";
    document.getElementById("fullSendBlock" + pmId).style.display = "none";
}

//添加满赠商品
function addFullSend(pmId, pid) {
    Ajax.get("/Cart/AddFullSend?pmId=" + pmId + "&pid=" + pid, false, function (data) {
        try {
            alert(eval("(" + data + ")").content);
        }
        catch (ex) {
            document.getElementById("cartBody").innerHTML = data;
        }
    })
}