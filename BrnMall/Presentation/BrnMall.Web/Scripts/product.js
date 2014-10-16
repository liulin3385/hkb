//增加商品数量
function addProuctCount() {
    var buyCountInput = document.getElementById("buyCount");
    var buyCount = buyCountInput.value;
    if (isNaN(buyCount)) {
        alert('请输入数字');
        return false;
    }
    var count = parseInt(buyCount);
    buyCountInput.value = count + 1;
}

//减少商品数量
function cutProductCount() {
    var buyCountInput = document.getElementById("buyCount");
    var buyCount = buyCountInput.value;
    if (isNaN(buyCount)) {
        alert('请输入数字');
        return false;
    }
    var count = parseInt(buyCount);
    if (count > 1) {
        buyCountInput.value = count - 1;
    }
}

//咨询商品
function consultProduct(uid) {
    var consultProductFrom = document.forms["consultProductFrom"];

    var pid = consultProductFrom.elements["pid"].value;
    var consultTypeId = getSelectedRadio(consultProductFrom.elements["consultTypeId"]).value;
    var consultMessage = consultProductFrom.elements["consultMessage"].value;
    var verifyCode = consultProductFrom.elements["verifyCode"] ? consultProductFrom.elements["verifyCode"].value : undefined;

    if (!verifyConsultProduct(uid, pid, consultTypeId, consultMessage, verifyCode)) {
        return;
    }
    Ajax.post("/Catalog/ConsultProduct", { 'pid': pid, 'consultTypeId': consultTypeId, 'consultMessage': consultMessage, 'verifyCode': verifyCode }, false, consultProductResponse)
}

//验证咨询商品
function verifyConsultProduct(uid, pid, consultTypeId, consultMessage, verifyCode) {
    if (uid < 1) {
        alert("请登录");
        return false;
    }
    if (pid < 1) {
        alert("请选择商品");
        return false;
    }
    if (consultTypeId < 1) {
        alert("请选择咨询类型");
        return false;
    }
    if (consultMessage.lenth < 1) {
        alert("请填写咨询内容");
        return false;
    }
    if (consultMessage.length > 100) {
        alert("咨询内容内容太长");
        return false;
    }
    if (verifyCode != undefined && verifyCode.length == 0) {
        alert("请输入验证码");
        return false;
    }
    return true;
}

//处理咨询商品的反馈信息
function consultProductResponse(data) {
    var result = eval("(" + data + ")");
    alert(result.content);
}

//获得商品评价列表
function getProductReviewList(pid, page) {
    Ajax.get("/Catalog/AjaxProductReviewList?pid=" + pid + "&page=" + page, false, getProductReviewListResponse)
}

//处理获得商品评价的反馈信息
function getProductReviewListResponse(data) {
    document.getElementById("productReviewList").innerHTML = data;
}

//获得商品咨询列表
function getProductConsultList(pid, consultTypeId, consultMessage, page) {
    Ajax.get("/Catalog/AjaxProductConsultList?pid=" + pid + "&consultTypeId=" + consultTypeId + "&consultMessage=" + encodeURIComponent(consultMessage) + "&page=" + page, false, getProductConsultListResponse)
}

//处理获得商品咨询的反馈信息
function getProductConsultListResponse(data) {
    document.getElementById("productConsultList").innerHTML = data;
}