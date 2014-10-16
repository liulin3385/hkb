/*分类选择层开始*/
var openCategorySelectLayerBut = null;
var categorySelectList = "<div class='selectBoxProgressBar'><p><img src='/Admin_Store/Content/Images/progressbar.gif'/></p></div>";

function categoryTree(obj, layer) {
    var state = $(obj).attr("class");
    if (state == "open") {
        $(obj).parent().parent().nextAll().each(function (index) {
            var flag = parseInt($(this).attr("layer")) - layer;
            if (flag == 1) {
                $(this).show();
            }
            else if (flag == 0) {
                return false;
            }
        })
        $(obj).removeClass("open").addClass("close");
    }
    else if (state == "close") {
        $(obj).parent().parent().nextAll().each(function (index) {
            if (parseInt($(this).attr("layer")) > layer) {
                $(this).hide();
                $(this).find("th span").each(function (i) {
                    if ($(this).attr("class") != "") {
                        $(this).removeClass("close").addClass("open");
                    }
                })
            }
            else {
                return false;
            }
        })
        $(obj).removeClass("close").addClass("open");
    }
}

function SetSelectedCategory(selectedCateId, selectedCateName) {
    var cateObj = $(openCategorySelectLayerBut).parent().find(".CateId");
    cateObj.val(selectedCateId);
    $(openCategorySelectLayerBut).val(selectedCateName);
    $(openCategorySelectLayerBut).parent().find(".CategoryName").val(selectedCateName);
    $.jBox.close('CategorySelectLayer');
}

function AjaxCategorySelectList() {
    $.jBox.setContent(categorySelectList);
    $.get("/Admin_Mall/Cache/Category/selectlist.js?t=" + new Date(), function (data) {
        $.jBox.setContent(data);
    })
}

function openCategorySelectLayer(openLayerBut) {
    $.jBox('html:categorySelectList', {
        id: 'CategorySelectLayer',
        width: 750,
        buttons: { '关闭': true },
        title: "选择类别"
    });
    openCategorySelectLayerBut = openLayerBut;
    AjaxCategorySelectList();
}
/*分类选择层开始*/

/*品牌选择层开始*/
var oldSelectListSearchBrand = "";
var openBrandSelectLayerBut = null;
var brandSelectList = "<div class='selectBoxProgressBar'><p><img src='/Admin_Store/Content/Images/progressbar.gif'/></p></div>";

function SetSelectedBrand(item) {
    var brandObj = $(openBrandSelectLayerBut).parent().find(".BrandId");
    brandObj.val($(item).attr("brandid"));
    var getbrandName = $(item).text();
    $(openBrandSelectLayerBut).val(getbrandName);
    $(openBrandSelectLayerBut).parent().find(".BrandName").val(getbrandName);
    $.jBox.close('BrandSelectLayer');
}

function AjaxBrandSelectList(brandName, pageNumber) {
    $.jBox.setContent(brandSelectList);
    $.get("/StoreAdmin/Brand/SelectList?t=" + new Date(), {
        'brandName': brandName,
        'pageNumber': pageNumber
    }, function (data) {
        var listObj = eval(data);
        var list = "<div id='selectBrandBox'><table width='100%' ><tr><td>品牌名称：<input type='text' id='selectListSearchBrand'  name='selectListSearchBrand' style='width:120px;height:18px;'> <input type='image' onclick='SearchBrandSelectList()' src='/Admin_Store/Content/Images/s.jpg' class='searchBut'></td></tr><tr><td><div id='selectBrandBoxCon'><ul>";
        for (var i = 0; i < listObj.items.length; i++) {
            list += "<li><a onclick='SetSelectedBrand(this)' brandid='" + listObj.items[i].id + "'>" + listObj.items[i].name + "</a></li>";
        }
        list += "<div class='clear'></div></ul><div class='clear'></div></div></td></tr><tr><td><div class='page' style='position:static;'>";
        for (var j = 1; j <= listObj.count; j++) {
            if (j != listObj.page) {
                list += "<a href='javascript:;' class='bt' onclick='GoBrandSelectListPage(this)' pageNumber='" + j + "'>" + j + "</a>";
            }
            else {
                list += "<a href='javascript:;' onclick='GoBrandSelectListPage(this)' pageNumber='" + j + "'>" + j + "</a>";
            }
        }
        $.jBox.setContent(list + "</div></td></tr></table></div>");
        $("#selectListSearchBrand").val(oldSelectListSearchBrand);
    })
}

function SearchBrandSelectList() {
    oldSelectListSearchBrand = $("#selectListSearchBrand").val();
    AjaxBrandSelectList(oldSelectListSearchBrand);
}

function GoBrandSelectListPage(pageObj) {
    oldSelectListSearchBrand = $("#selectListSearchBrand").val();
    AjaxBrandSelectList(oldSelectListSearchBrand, $(pageObj).attr("pageNumber"));
}

function openBrandSelectLayer(openLayerBut) {
    $.jBox('html:brandSelectList', {
        id: 'BrandSelectLayer',
        width: 750,
        buttons: { '关闭': true },
        title: "选择品牌"
    });
    openBrandSelectLayerBut = openLayerBut;
    AjaxBrandSelectList();

}
/*品牌选择层结束*/

/*商品选择层开始*/
var oldSelectListSearchProduct = "";
var openProductSelectLayerBut = null;
var productSelectList = "<div class='selectBoxProgressBar'><p><img src='/Admin_Store/Content/Images/progressbar.gif'/></p></div>";

function SetSelectedProduct(item) {
    var productObj = $(openProductSelectLayerBut).parent().find(".Pid");
    productObj.val($(item).attr("pid"));
    var getproductName = $(item).text();
    $(openProductSelectLayerBut).val(getproductName);
    $(openProductSelectLayerBut).parent().find(".ProductName").val(getproductName);
    $.jBox.close('ProductSelectLayer');
}

function AjaxProductSelectList(productName, pageNumber) {
    $.jBox.setContent(productSelectList);
    $.get("/StoreAdmin/Product/ProductSelectList?t=" + new Date(), {
        'ProductName': productName,
        'pageNumber': pageNumber
    }, function (data) {
        var listObj = eval(data);
        var list = "<div id='selectProductBox'><table width='100%' ><tr><td>商品名称：<input type='text' id='selectListSearchProduct'  name='selectListSearchProduct' style='width:120px;height:18px;'> <input type='image' onclick='SearchProductSelectList()' src='/Admin_Store/Content/Images/s.jpg' class='searchBut'></td></tr><tr><td><div id='selectProductBoxCon'><ul>";
        for (var i = 0; i < listObj.items.length; i++) {
            list += "<li><a onclick='SetSelectedProduct(this)' pid='" + listObj.items[i].id + "'>" + listObj.items[i].name + "</a></li>";
        }
        list += "<div class='clear'></div></ul><div class='clear'></div></div></td></tr><tr><td><div class='page' style='position:static;'>";
        for (var j = 1; j <= listObj.count; j++) {
            if (j != listObj.page) {
                list += "<a href='javascript:;' class='bt' onclick='GoProductSelectListPage(this)' pageNumber='" + j + "'>" + j + "</a>";
            }
            else {
                list += "<a href='javascript:;' onclick='GoProductSelectListPage(this)' pageNumber='" + j + "'>" + j + "</a>";
            }
        }
        $.jBox.setContent(list + "</div></td></tr></table></div>");
        $("#selectListSearchProduct").val(oldSelectListSearchProduct);
    })
}

function SearchProductSelectList() {
    oldSelectListSearchProduct = $("#selectListSearchProduct").val();
    AjaxProductSelectList(storeId, oldSelectListSearchProduct);
}

function GoProductSelectListPage(pageObj) {
    oldSelectListSearchProduct = $("#selectListSearchProduct").val();
    AjaxProductSelectList(storeId, oldSelectListSearchProduct, $(pageObj).attr("pageNumber"));
}

function openProductSelectLayer(openLayerBut) {
    $.jBox('html:productSelectList', {
        id: 'ProductSelectLayer',
        width: 750,
        buttons: { '关闭': true },
        title: "选择商品"
    });
    openProductSelectLayerBut = openLayerBut;
    AjaxProductSelectList();

}
/*商品选择层结束*/

/*配送公司选择层开始*/
var openShipCompanySelectLayerBut = null;
var shipCompanySelectList = "<div class='selectBoxProgressBar'><p><img src='/Admin_Store/Content/Images/progressbar.gif'/></p></div>";

function SetSelectedShipCompany(item) {
    var shipCompanyObj = $(openShipCompanySelectLayerBut).parent().find(".ShipCoId");
    shipCompanyObj.val($(item).attr("shipCoId"));
    var getshipCoName = $(item).text();
    $(openShipCompanySelectLayerBut).val(getshipCoName);
    $(openShipCompanySelectLayerBut).parent().find(".ShipCoName").val(getshipCoName);
    $.jBox.close('ShipCompanySelectLayer');
}

function AjaxShipCompanySelectList(pageNumber) {
    $.jBox.setContent(shipCompanySelectList);
    $.get("/StoreAdmin/ShipCompany/SelectList?t=" + new Date(), {
        'pageNumber': pageNumber
    }, function (data) {
        var listObj = eval(data);
        var list = "<div id='selectShipCompanyBox'><table width='100%' ><tr><td><div id='selectShipCompanyBoxCon'><ul>";
        for (var i = 0; i < listObj.items.length; i++) {
            list += "<li><a onclick='SetSelectedShipCompany(this)' shipCoId='" + listObj.items[i].id + "'>" + listObj.items[i].name + "</a></li>";
        }
        list += "<div class='clear'></div></ul><div class='clear'></div></div></td></tr><tr><td><div class='page' style='position:static;'>";
        for (var j = 1; j <= listObj.count; j++) {
            if (j != listObj.page) {
                list += "<a href='javascript:;' class='bt' onclick='GoShipCompanySelectListPage(this)' pageNumber='" + j + "'>" + j + "</a>";
            }
            else {
                list += "<a href='javascript:;' onclick='GoShipCompanySelectListPage(this)' pageNumber='" + j + "'>" + j + "</a>";
            }
        }
        $.jBox.setContent(list + "</div></td></tr></table></div>");
    })
}

function GoShipCompanySelectListPage(pageObj) {
    AjaxShipCompanySelectList($(pageObj).attr("pageNumber"));
}

function openShipCompanySelectLayer(openLayerBut) {
    $.jBox('html:shipCompanySelectList', {
        id: 'ShipCompanySelectLayer',
        width: 750,
        buttons: { '关闭': true },
        title: "选择配送公司"
    });
    openShipCompanySelectLayerBut = openLayerBut;
    AjaxShipCompanySelectList();

}
/*配送公司选择层结束*/