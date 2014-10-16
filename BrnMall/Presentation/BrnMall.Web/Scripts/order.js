//获得配送地址列表
function getShipAddressList() {
    Ajax.get("/UCenter/AjaxShipAddressList", false, getShipAddressListResponse);
}

//处理获得配送地址列表的反馈信息
function getShipAddressListResponse(data) {
    var result = eval("(" + data + ")");
    if (result.state == "success") {
        var shipAddressList = "";
        for (var i = 0; i < result.content.count; i++) {
            shipAddressList += "<tr><td width='70px' bgcolor='#ffffff'><input name='shipAddressItem' type='radio' value='" + result.content.list[i].said + "' onclick=" + '"' + "selectShipAddress(" + result.content.list[i].said + ")" + '"' + " /></td><td bgcolor='#ffffff'>" + result.content.list[i].user + "&nbsp;&nbsp;&nbsp;" + result.content.list[i].address + "</td></tr>";
        }
        document.getElementById("shipAddressShowBlock").style.display = "none";
        document.getElementById("shipAddressListBlock").style.display = "";
        document.getElementById("shipAddressListBlock").innerHTML = shipAddressList;
        document.getElementById("addShipAddressBlock").style.display = "";
    }
    else {
        alert(result.content);
    }
}

//选择配送地址
function selectShipAddress(saId) {
    var payName = document.getElementById("payName").value;
    window.location.href = "/Order/ConfirmOrder?saId=" + saId + "&payName=" + payName;
}

//添加配送地址
function addShipAddress() {
    var addShipAddressForm = document.forms["addShipAddressForm"];

    var alias = addShipAddressForm.elements["alias"].value;
    var consignee = addShipAddressForm.elements["consignee"].value;
    var mobile = addShipAddressForm.elements["mobile"].value;
    var phone = addShipAddressForm.elements["phone"].value;
    var email = addShipAddressForm.elements["email"].value;
    var zipcode = addShipAddressForm.elements["zipcode"].value;
    var regionId = getSelectedOption(addShipAddressForm.elements["regionId"]).value;
    var address = addShipAddressForm.elements["address"].value;
    var isDefault = addShipAddressForm.elements["isDefault"].checked ? 1 : 0;

    if (!verifyAddShipAddress(alias, consignee, mobile, regionId, address)) {
        return;
    }

    Ajax.post("/UCenter/AddShipAddress",
            { 'alias': alias, 'consignee': consignee, 'mobile': mobile, 'phone': phone, 'email': email, 'zipcode': zipcode, 'regionId': regionId, 'address': address, 'isDefault': isDefault },
            false,
            addShipAddressResponse)
}

//验证添加的收货地址
function verifyAddShipAddress(alias, consignee, mobile, regionId, address) {
    if (alias == "") {
        alert("请填写昵称");
        return false;
    }
    if (consignee == "") {
        alert("请填写收货人");
        return false;
    }
    if (mobile == "" && phone == "") {
        alert("手机号和固定电话必须填写一项");
        return false;
    }
    if (parseInt(regionId) < 1) {
        alert("请选择区域");
        return false;
    }
    if (address == "") {
        alert("请填写详细地址");
        return false;
    }
    return true;
}

//处理添加配送地址的反馈信息
function addShipAddressResponse(data) {
    var result = eval("(" + data + ")");
    if (result.state == "success") {
        var payName = document.getElementById("payName").value;
        window.location.href = "/Order/ConfirmOrder?saId=" + result.content + "&payName=" + payName;
    }
    else {
        var msg = "";
        for (var i = 0; i < result.content.length; i++) {
            msg += result.content[i].msg + "\n";
        }
        alert(msg)
    }
}

//获得支付插件列表
function getPayPluginList() {
    Ajax.get("/Order/PayPluginList", false, getPayPluginListResponse);
}

//处理获得支付插件列表的反馈信息
function getPayPluginListResponse(data) {
    var result = eval("(" + data + ")");
    if (result.state == "nologin") {
        alert("请先登录");
    }
    else if (result.state == "success") {
        var payPluginList = "<tr><th bgcolor='#ffffff'></th><th bgcolor='#ffffff'>名称</th><th bgcolor='#ffffff'>描述</th></tr>";
        for (var i = 0; i < result.content.length; i++) {
            payPluginList += "<tr><td width='70px' bgcolor='#ffffff'><input name='payPluginItem' type='radio' value='" + result.content[i].systemname + "' onclick=" + '"' + "selectPayPlugin('" + result.content[i].systemname + "'" + ")" + '"' + " /></td><td bgcolor='#ffffff'>" + result.content[i].friendname + "</td><td bgcolor='#ffffff'>" + result.content[i].description + "</td></tr>";
        }
        document.getElementById("payPluginShowBlock").style.display = "none";
        document.getElementById("payPluginListBlock").style.display = "";
        document.getElementById("payPluginListBlock").innerHTML = payPluginList;
    }
}

//选择支付方式
function selectPayPlugin(paySystemName) {
    var saId = document.getElementById("saId").value;
    window.location.href = "/Order/ConfirmOrder?saId=" + saId + "&payName=" + paySystemName;
}

//验证支付积分
function verifyPayCredit(hasPayCreditCount, maxUsePayCreditCount) {
    var obj = document.getElementById("payCreditCount");
    var usePayCreditCount = obj.value;
    if (isNaN(usePayCreditCount)) {
        obj.value = 0;
        alert("请输入数字");
    }
    else if (usePayCreditCount > hasPayCreditCount) {
        obj.value = hasPayCreditCount;
        alert("积分不足");
    }
    else if (usePayCreditCount > maxUsePayCreditCount) {
        obj.value = maxUsePayCreditCount;
        alert("最多只能使用" + maxUsePayCreditCount + "个");
    }
}

//获得有效的优惠劵列表
function getValidCouoponList() {
    Ajax.get("/Order/GetValidCouponList", false, getValidCouoponListResponse);
}

//处理获得有效的优惠劵列表的反馈信息
function getValidCouoponListResponse(data) {
    var result = eval("(" + data + ")");
    if (result.state == "success") {
        var optionList = "<option selected='selected' value='0'>选择优惠劵</option>";
        for (var i = 0; i < result.length; i++) {
            optionList += "<option value='" + result[i].couponid + "'>" + result[i].name + "</option>";
        }
        document.getElementById("couponId").innerHTML = optionList;
    }
    else {
        alert(result.content);
    }
}

//验证优惠劵编号
function verifyCouponSN(couponSN) {
    if (couponSN == undefined || couponSN == null || couponSN.length == 0) {
        alert("请输入优惠劵编号");
    }
    else if (couponSN.length != 16) {
        alert("优惠劵编号不正确");
    }
    else {
        Ajax.get("/Order/VerifyCouponSN?couponSN=" + couponSN, false, verifyCouponSNResponse);
    }
}

//处理验证优惠劵编号的反馈信息
function verifyCouponSNResponse(data) {
    var result = eval("(" + data + ")");
    alert(result.content);
}

//提交订单
function submitOrder() {
    var saId = document.getElementById("saId").value;
    var payName = document.getElementById("payName").value;
    var payCreditCount = document.getElementById("payCreditCount") ? document.getElementById("payCreditCount").value : 0;
    var couponId = document.getElementById("couponId") ? getSelectedOption(document.getElementById("couponId")).value : 0;
    var couponSN = document.getElementById("couponSN") ? document.getElementById("couponSN").value : "";
    var buyerRemark = document.getElementById("buyerRemark") ? document.getElementById("buyerRemark").value : "";
    var verifyCode = document.getElementById("verifyCode") ? document.getElementById("verifyCode").value : "";

    if (!verifySubmitOrder(saId, payName, buyerRemark)) {
        return;
    }

    Ajax.post("/Order/SubmitOrder",
            { 'verifyCode': verifyCode, 'buyerRemark': buyerRemark, 'saId': saId, 'payName': payName, 'payCreditCount': payCreditCount, 'couponId': couponId, 'couponSN': couponSN },
            false,
            submitOrderResponse)
}

//验证提交订单
function verifySubmitOrder(saId, payName, buyerRemark) {
    if (saId < 1) {
        alert("收货地址不能为空");
        return false;
    }
    if (payName.length < 1) {
        alert("配送方式不能为空");
        return false;
    }
    if (buyerRemark.length > 125) {
        alert("最多只能输入125个字");
        return false;
    }
    return true;
}

//处理提交订单的反馈信息
function submitOrderResponse(data) {
    var result = eval("(" + data + ")");
    if (result.state != "success") {
        alert(result.content);
    }
    else {
        window.location.href = result.content;
    }
}