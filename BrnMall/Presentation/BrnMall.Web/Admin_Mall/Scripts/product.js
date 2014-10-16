var productPageType = 0; //0为添加商品,1为编辑商品,2为商品列表,3添加sku
var tempinputattrvalue = ""; //临时值
var productAttributeList = null; //商品属性列表
var pid = 0; //商品id
var skuAttrList = null; //sku属性列表

//小数取整
function modFoat(v) {
    var max = parseInt(v) + 1;
    if (max - v < 1) {
        return max;
    }
    return v;
}

//选择sku项
function setSKU(obj) {

    //初始化选中的属性值数组
    skuAttrList = new Array();

    var attrTrList = $("#skuTable tr[name=attrTr]");
    for (var i = 0; i < attrTrList.length; i++) {
        var items = new Array();

        //复选框sku
        var checkedAttrValueList = $(attrTrList[i]).find("input[name=attrValue]:checked");
        for (var j = 0; j < checkedAttrValueList.length; j++) {
            var currentObj = $(checkedAttrValueList[j]);
            var item = {
                'attrId': currentObj.attr("attrId"),
                'attrValueId': currentObj.val(),
                'attrValue': currentObj.attr("attrValue"),
                'inputValue': ''
            };
            items.push(item);
        }

        //手动输入sku
        var skuInputValueObj = $(attrTrList[i]).find("input[name=skuInputValue]");
        var skuInputValue = skuInputValueObj.val();
        if (skuInputValue != "") {
            var item = {
                'attrId': skuInputValueObj.attr("attrId"),
                'attrValueId': skuInputValueObj.attr("attrValueId"),
                'attrValue': skuInputValue,
                'inputValue': skuInputValue
            };
            items.push(item);
        }

        if (items.length > 0)
            skuAttrList.push(items);
    }

    //计算sku的总数
    var loopCount = 1;
    for (var i = 0; i < skuAttrList.length; i++) {
        loopCount = loopCount * skuAttrList[i].length;
    }
    if (loopCount <= 1)
        return;

    //构建sku
    var skuItmesStr = "";
    for (var i = 1; i <= loopCount; i++) {
        skuItmesStr += "<tr name='skuItemTr'><td width='76px' align='right'>SKU项：</td><td><table style='border-bottom:1px solid #d1d1d1;'><tr>";
        for (var j = 0; j < skuAttrList.length; j++) {
            var index = 1;

            if (j == 0) {
                index = modFoat(i / (loopCount / skuAttrList[0].length)) - 1;
            }
            else if (j == skuAttrList.length - 1) {
                index = i % skuAttrList[j].length;
                if (index == 0) {
                    index = skuAttrList[j].length - 1;
                }
                else {
                    index = index - 1;
                }
            }
            else {
                var itemRepeatCount = 1;
                for (var x = j; x < skuAttrList.length; x++) {
                    itemRepeatCount = itemRepeatCount * skuAttrList[x].length;
                }
                var itemGroupCount = i % itemRepeatCount;
                if (itemGroupCount == 0) {
                    index = skuAttrList[j].length - 1;
                }
                else {
                    var lineCount = itemRepeatCount / skuAttrList[j].length;
                    index = modFoat(itemGroupCount / lineCount) - 1;
                }
            }

            skuItmesStr += "<td width='88px'>" + skuAttrList[j][index].attrValue + "<input type='hidden' name='SKUAttrIdList' value='" + skuAttrList[j][index].attrId + "'/><input type='hidden' name='SKUAttrValueIdList' value='" + skuAttrList[j][index].attrValueId + "'/><input type='hidden' name='SKUAttrInputValueList' value='" + skuAttrList[j][index].inputValue + "'/></td>";
        }
        skuItmesStr += "</tr></table></td></tr>";
    }
    $("#skuTable tr[name=skuItemTr]").remove();
    $("#addSkuBut").before(skuItmesStr);
}

//构建属性选择表格
function buildAttrTable(result) {
    if (result.length < 1) {
        return "";
    }

    var optionList = "";
    var tempInputAttrValueId = 0;
    for (var i = 0; i < result.length; i++) {
        optionList += "<tr><td width='80px;' align='right'>";
        optionList += result[i].name;
        optionList += "：</td><td>";
        optionList += "<input name='AttrIdList' type='checkbox' style='display:none;' value='" + result[i].attrid + "'/>";
        for (var j = 0; j < result[i].attrvaluelist.length; j++) {
            optionList += "<input name='AttrValueIdList' type='checkbox' onclick='selectAttrValue(this)' isinput='" + result[i].attrvaluelist[j].isinput + "' value='" + result[i].attrvaluelist[j].attrvalueid + "' />" + result[i].attrvaluelist[j].attrvalue + "&nbsp;&nbsp;";
            if (result[i].attrvaluelist[j].isinput == 1) {
                tempInputAttrValueId = result[i].attrvaluelist[j].attrvalueid;
            }
        }
        optionList += "<input name='AttrInputValueList' type='text' attrIdSign='" + result[i].attrid + "' attrValueIdSign='" + tempInputAttrValueId + "' class='inputattrvalue input' size='20' style='display:none;' value=''/>";
    }
    optionList += "</td></tr>";
    return optionList;
}

//属性值选择方法
function selectAttrValue(obj) {
    var radioObj = $(obj);
    var isInput = radioObj.attr("isinput");

    if (radioObj.attr("checked") == true) {
        radioObj.parent().find("input[type=checkbox]").attr("checked", false);
        radioObj.parent().find("input[name=AttrIdList]").attr("checked", true);
        radioObj.attr("checked", true);
        if (isInput == "1") {//手动输入时
            radioObj.parent().find("input[type=text]").show();
        }
        else {//选择此属性时
            radioObj.parent().find("input[type=text]").val("").hide();
        }
    }
    else {
        radioObj.parent().find("input[type=checkbox]").attr("checked", false);
        radioObj.parent().find("input[type=text]").val("").hide();
    }

    //页面为编辑商品页面时
    if (productPageType == 1) {

        var changeproductattributeurl = "";
        if (radioObj.attr("checked") == false) {
            changeproductattributeurl = "/MallAdmin/product/DelProductAttribute?pid=" + pid + "&attrId=" + radioObj.parent().find("input[name=AttrIdList]").val() + "&t=" + new Date();
        }
        else {
            if (isInput == "0") {
                changeproductattributeurl = "/MallAdmin/product/UpdateProductAttribute?type=0" + "&pid=" + pid + "&attrId=" + radioObj.parent().find("input[name=AttrIdList]").val() + "&attrValueId=" + radioObj.val() + "&t=" + new Date();
            }
        }
        if (changeproductattributeurl != "") {
            $.jBox.tip("正在更新...", 'loading');
            $.get(changeproductattributeurl, function (data, textStatus) {
                if (data != "0") {
                    $.jBox.tip('更新成功！', 'success');
                } else {
                    $.jBox.error('更新失败，请联系管理员！', '更新失败');
                }
            });
        }

    }
}

//重写分类弹出层中的SetSelectedCategory方法
function SetSelectedCategory(selectedCateId, selectedCateName) {
    var cateObj = $(openCategorySelectLayerBut).parent().find(".CateId");
    cateObj.val(selectedCateId);
    $(openCategorySelectLayerBut).val(selectedCateName);
    $(openCategorySelectLayerBut).parent().find(".CategoryName").val(selectedCateName);
    $.jBox.close('CategorySelectLayer');
    $("#categoryTable").find("tr:not('.keepTr')").remove();
    if (selectedCateId != "-1") {
        $.get("/MallAdmin/Category/AANDVJSONList?cateId=" + selectedCateId + "&time=" + new Date(), function (data) {
            var result = eval(data);

            //页面为添加普通商品时生成属性选择表格
            if (productPageType == 0) {
                var optionList = buildAttrTable(result);
                $("#categoryTable").prepend(optionList);
            }
            else if (productPageType == 3) {//页面为添加sku页面时生成sku选择表格
                $("#skuTable").find("tr:not('.keepTr')").remove();
                var list = "";
                var tempInputAttrValueId = 0;
                for (var i = 0; i < result.length; i++) {
                    list += "<tr name='attrTr'><td width='80px;' align='right'>" + result[i].name + "：</td><td><p>";
                    for (var j = 0; j < result[i].attrvaluelist.length; j++) {
                        if (result[i].attrvaluelist[j].isinput == "0") {
                            list += "<label><input type='checkbox' name='attrValue' attrId='" + result[i].attrid + "' attrValue='" + result[i].attrvaluelist[j].attrvalue + "' onclick='setSKU(this)' value='" + result[i].attrvaluelist[j].attrvalueid + "' />" + result[i].attrvaluelist[j].attrvalue + "</label>" + "&nbsp;&nbsp;";
                        }
                        else {
                            tempInputAttrValueId = result[i].attrvaluelist[j].attrvalueid;
                        }
                    }
                    list += "手动输入<input type='text' name='skuInputValue' attrId='" + result[i].attrid + "' attrValueId='" + tempInputAttrValueId + "' class='input' size='20' onblur='setSKU(this)' value=''/>";
                    list += "<br /></p></td></tr>";
                }
                $("#addSkuBut").before(list);
            }
        });
    }
    else {
        alert("请先选择正确的分类");
    }
}

//重写店铺弹出层中的SetSelectedStore方法
function SetSelectedStore(item) {
    var selectedStoreId = $(item).attr("storeid");
    var storeObj = $(openStoreSelectLayerBut).parent().find(".StoreId");
    storeObj.val(selectedStoreId);
    var getstoreName = $(item).text();
    $(openStoreSelectLayerBut).val(getstoreName);
    $(openStoreSelectLayerBut).parent().find(".StoreName").val(getstoreName);
    $.jBox.close('StoreSelectLayer');

    $("#StoreCid").html("").attr("disabled", "disabled");
    $.get("/MallAdmin/Store/StoreClassSelectList?storeId=" + selectedStoreId + "&time=" + new Date(), function (data) {
        var result = eval("(" + data + ")");
        var list = "";
        list += "<option value='-1' slected='selected'>选择店铺分类</option>";
        for (var i = 0; i < result.length; i++) {
            list = list + "<option value='" + result[i].storecid + "'>" + result[i].name + "</option>";
        }
        $("#StoreCid").html(list).attr("disabled","");
    });

    $("#StoreSTid").html("").attr("disabled", "disabled");
    $.get("/MallAdmin/Store/StoreShipTemplateSelectList?storeId=" + selectedStoreId + "&time=" + new Date(), function (data) {
        var result = eval("(" + data + ")");
        var list = "";
        list += "<option value='-1' slected='selected'>选择配送模板</option>";
        for (var i = 0; i < result.length; i++) {
            list = list + "<option value='" + result[i].storestid + "'>" + result[i].title + "</option>";
        }
        $("#StoreSTid").html(list).attr("disabled", "");
    });
}

$(function () {

    //提交按钮
    $(".submit").click(function () {
        $("form:first").submit();
        return false;
    })

    //选项卡
    $(".addTag li").click(function () {
        $(".addTag li").removeClass("hot");
        $(this).addClass("hot");
        $(".addTable").hide().eq($(this).index()).show(0);

    })

    //初始化店铺分类选择框
    if ($("#StoreId").val() > 0) {
        $("#StoreCid").html("").attr("disabled", "disabled");
        $.get("/MallAdmin/Store/StoreClassSelectList?storeId=" + $("#StoreId").val() + "&time=" + new Date(), function (data) {
            var result = eval("(" + data + ")");
            var list = "";
            list += "<option value='-1' slected='selected'>选择店铺分类</option>";
            for (var i = 0; i < result.length; i++) {
                list = list + "<option value='" + result[i].storecid + "'>" + result[i].name + "</option>";
            }
            $("#StoreCid").html(list).attr("disabled", "");
            $("#StoreCid").find("option[value=" + $("#hiddenStoreCid").val() + "]").attr("selected", true);
        });
    }

    //当页面不是列表且选择了分类时设置商品属性
    if (productPageType != 2 && $("#CateId").val() > 0) {
        $("#categoryTable").find("tr:not('.keepTr')").remove();
        $.get("/MallAdmin/Category/AANDVJSONList?cateId=" + $("#CateId").val() + "&time=" + new Date(), function (data) {
            result = eval(data);
            var optionList = buildAttrTable(result);
            $("#categoryTable").prepend(optionList);

            //选中对应属性
            if (productAttributeList != undefined && productAttributeList != null) {
                for (var i = 0; i < productAttributeList.length; i++) {
                    $("#categoryTable").find("input[name=AttrIdList][value=" + productAttributeList[i].attrid + "]").attr("checked", true);
                    $("#categoryTable").find("input[name=AttrValueIdList][value=" + productAttributeList[i].attrvalueid + "]").attr("checked", true);
                    if (productAttributeList[i].inputvalue != "") {
                        $("#categoryTable").find("input[name=AttrInputValueList][attrIdSign=" + productAttributeList[i].attrid + "]").show().val(productAttributeList[i].inputvalue);
                    }
                }

                //当页面为编辑商品页面时
                if (productPageType == 1) {
                    $(".inputattrvalue").focus(function () {
                        var inputattrvalueobj = $(this);
                        tempinputattrvalue = inputattrvalueobj.val();
                    });

                    $(".inputattrvalue").blur(function () {
                        var inputattrvalueobj = $(this);
                        if (inputattrvalueobj.val() != tempinputattrvalue) {
                            $.jBox.tip("正在更新...", 'loading');
                            $.get("/MallAdmin/product/UpdateProductAttribute?type=1" + "&pid=" + pid + "&attrId=" + inputattrvalueobj.parent().find("input[name=AttrIdList]").val() + "&attrValueId=" + inputattrvalueobj.attr("attrValueIdSign") + "&inputValue=" + encodeURIComponent(inputattrvalueobj.val()) + "&t=" + new Date(), function (data, textStatus) {
                                if (data != "0") {
                                    $.jBox.tip('更新成功！', 'success');
                                } else {
                                    inputattrvalueobj.val(tempinputattrvalue);
                                    $.jBox.error('更新失败，请联系管理员！', '更新失败');
                                }
                            });
                        }
                    });
                }


            }

        });
    }

    //当页面为列表时
    if (productPageType == 2) {
        //本店售价
        var shoppriceinputtempvalue = 0;
        $(".shoppriceinput").focus(function () {
            var shoppriceinputobj = $(this);
            shoppriceinputtempvalue = shoppriceinputobj.val();
            shoppriceinputobj.val("");
            shoppriceinputobj.attr("class", "selectedsortinput");
        });
        $(".shoppriceinput").blur(function () {
            var shoppriceinputobj = $(this);
            if (shoppriceinputobj.val() == "") {
                sortinputobj.val(sortinputtempvalue)
            }
            else {
                var reg = /^\d+(\.\d{1,2})?$/;
                if (!reg.test(shoppriceinputobj.val())) {
                    shoppriceinputobj.val(shoppriceinputtempvalue).attr("class", "selectedsortinput");
                    alert("只能输入正数且最多两位小数！")
                    return;
                }
                else {
                    if (shoppriceinputtempvalue != shoppriceinputobj.val()) {
                        $.jBox.tip("正在更新...", 'loading');
                        $.get(shoppriceinputobj.attr("url") + "&shopprice=" + shoppriceinputobj.val() + "&t=" + new Date(), function (data, textStatus) {
                            if (data != "0") {
                                $.jBox.tip('更新成功！', 'success');
                            } else {
                                shoppriceinputobj.val(shoppriceinputtempvalue);
                                $.jBox.error('更新失败，请联系管理员！', '更新失败');
                            }
                        });
                    }
                }
            }
            shoppriceinputobj.attr("class", "unselectedsortinput");
        });

        //库存
        var stockinputtempvalue = 0;
        $(".stockinput").focus(function () {
            var stockinputobj = $(this);
            stockinputtempvalue = stockinputobj.val();
            stockinputobj.val("");
            stockinputobj.attr("class", "selectedsortinput");
        });
        $(".stockinput").blur(function () {
            var stockinputobj = $(this);
            if (stockinputobj.val() == "") {
                sortinputobj.val(sortinputtempvalue)
            }
            else {
                var reg = /^\d+$/;
                if (!reg.test(stockinputobj.val())) {
                    stockinputobj.val(stockinputtempvalue).attr("class", "selectedsortinput");
                    alert("只能输入数字！")
                    return;
                }
                else {
                    if (stockinputtempvalue != stockinputobj.val()) {
                        $.jBox.tip("正在更新...", 'loading');
                        $.get(stockinputobj.attr("url") + "&StockNumber=" + stockinputobj.val() + "&t=" + new Date(), function (data, textStatus) {
                            if (data != "0") {
                                $.jBox.tip('更新成功！', 'success');
                            } else {
                                stockinputobj.val(stockinputtempvalue);
                                $.jBox.error('更新失败，请联系管理员！', '更新失败');
                            }
                        });
                    }
                }
            }
            stockinputobj.attr("class", "unselectedsortinput");
        });

        /*精品*/
        $(".isbestspan").click(function () {
            var isbestspanobj = $(this);
            var isbest = isbestspanobj.attr("isbest");
            var alertmessage = "";
            var isbesturl = "";
            if (isbest == "0") {
                alertmessage = "您确认要设置此商品为精品吗？";
                isbesturl = isbestspanobj.attr("url") + "&state=1" + "&t=" + new Date();
            }
            else if (isbest == "1") {
                alertmessage = "您确认要取消此商品为精品吗？";
                isbesturl = isbestspanobj.attr("url") + "&state=0" + "&t=" + new Date();
            }
            $.jBox.confirm(alertmessage, "提示", function (v, h, f) {
                if (v == 'ok') {
                    $.jBox.tip("正在设置...", 'loading');
                    $.get(isbesturl, function (data, textStatus) {
                        if (data != "0") {
                            if (isbest == "0") {
                                isbestspanobj.html("是").attr("isbest", "1");
                            }
                            else if (isbest == "1") {
                                isbestspanobj.html("否").attr("isbest", "0");
                            }
                            $.jBox.tip('设置成功！', 'success');
                        } else {
                            $.jBox.error('设置失败，请联系管理员！', '设置失败');
                        }
                    });
                }
                else if (v == 'cancel') {
                    // 取消
                }

                return true; //close
            });
        });

        /*热销*/
        $(".ishotspan").click(function () {
            var ishotspanobj = $(this);
            var ishot = ishotspanobj.attr("ishot");
            var alertmessage = "";
            var ishoturl = "";
            if (ishot == "0") {
                alertmessage = "您确认要设置此商品为热销吗？";
                ishoturl = ishotspanobj.attr("url") + "&state=1" + "&t=" + new Date();
            }
            else if (ishot == "1") {
                alertmessage = "您确认要取消此商品为热销吗？";
                ishoturl = ishotspanobj.attr("url") + "&state=0" + "&t=" + new Date();
            }
            $.jBox.confirm(alertmessage, "提示", function (v, h, f) {
                if (v == 'ok') {
                    $.jBox.tip("正在设置...", 'loading');
                    $.get(ishoturl, function (data, textStatus) {
                        if (data != "0") {
                            if (ishot == "0") {
                                ishotspanobj.html("是").attr("ishot", "1");
                            }
                            else if (ishot == "1") {
                                ishotspanobj.html("否").attr("ishot", "0");
                            }
                            $.jBox.tip('设置成功！', 'success');
                        } else {
                            $.jBox.error('设置失败，请联系管理员！', '设置失败');
                        }
                    });
                }
                else if (v == 'cancel') {
                    // 取消
                }

                return true; //close
            });
        });

        /*新品*/
        $(".isnewspan").click(function () {
            var isnewspanobj = $(this);
            var isnew = isnewspanobj.attr("isnew");
            var alertmessage = "";
            var isnewurl = "";
            if (isnew == "0") {
                alertmessage = "您确认要设置此商品为新品吗？";
                isnewurl = isnewspanobj.attr("url") + "&state=1" + "&t=" + new Date();
            }
            else if (isnew == "1") {
                alertmessage = "您确认要取消此商品为新品吗？";
                isnewurl = isnewspanobj.attr("url") + "&state=0" + "&t=" + new Date();
            }
            $.jBox.confirm(alertmessage, "提示", function (v, h, f) {
                if (v == 'ok') {
                    $.jBox.tip("正在设置...", 'loading');
                    $.get(isnewurl, function (data, textStatus) {
                        if (data != "0") {
                            if (isnew == "0") {
                                isnewspanobj.html("是").attr("isnew", "1");
                            }
                            else if (isnew == "1") {
                                isnewspanobj.html("否").attr("isnew", "0");
                            }
                            $.jBox.tip('设置成功！', 'success');
                        } else {
                            $.jBox.error('设置失败，请联系管理员！', '设置失败');
                        }
                    });
                }
                else if (v == 'cancel') {
                    // 取消
                }

                return true; //close
            });
        });

        /*下架*/
        $(".outsaletag").click(function () {
            var outsaletagobj = $(this);
            var outsaleurl = outsaletagobj.attr("url") + "&t=" + new Date();
            $.jBox.confirm("您确认要下架此商品吗？", "提示", function (v, h, f) {
                if (v == 'ok') {
                    $.jBox.tip("正在设置...", 'loading');
                    $.get(outsaleurl, function (data, textStatus) {
                        if (data != "0") {
                            outsaletagobj.parents("tr").remove();
                            $.jBox.tip('下架成功！', 'success');
                        } else {
                            $.jBox.error('下架失败，请联系管理员！', '下架失败');
                        }
                    });
                }
                else if (v == 'cancel') {
                    // 取消
                }

                return true; //close
            });

            return false;
        });

        /*上架*/
        $(".onsaletag").click(function () {
            var onsaletagobj = $(this);
            var onsaleurl = onsaletagobj.attr("url") + "&t=" + new Date();
            $.jBox.confirm("您确认要上架此商品吗？", "提示", function (v, h, f) {
                if (v == 'ok') {
                    $.jBox.tip("正在上架...", 'loading');
                    $.get(onsaleurl, function (data, textStatus) {
                        if (data != "0") {
                            onsaletagobj.parents("tr").remove();
                            $.jBox.tip('上架成功！', 'success');
                        } else {
                            $.jBox.error('上架失败，请联系管理员！', '上架失败');
                        }
                    });
                }
                else if (v == 'cancel') {
                    // 取消
                }

                return true; //close
            });

            return false;
        });

    }
});
