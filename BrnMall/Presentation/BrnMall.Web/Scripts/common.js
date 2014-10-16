var uid = -1; //用户id
var isGuestSC = 0; //是否允许游客使用购物车(0代表不可以，1代表可以)
var scSubmitType = 0; //购物车的提交方式(0代表跳转到提示页面，1代表跳转到列表页面，2代表ajax提交)

//商城搜索
function mallSearch(keyword) {
    if (keyword == undefined || keyword == null || keyword.length < 1) {
        alert("请输入关键词");
    }
    else {
        window.location.href = "/Catalog/Search?keyword=" + encodeURIComponent(keyword);
    }
}

//获得购物车快照
function getCartSnap() {
    Ajax.get("/Cart/Snap", false, function (data) {
        getCartSnapResponse(data);
    })
}

//处理获得购物车快照的反馈信息
function getCartSnapResponse(data) {
    var cartSnap = document.getElementById("cartSnap");
    try {
        var result = eval("(" + data + ")");
        cartSnap.innerHTML = result.content;
    }
    catch (ex) {
        cartSnap.innerHTML = data;
    }
    cartSnap.style.display = "";
}

//添加商品到收藏夹
function addProductToFavorite(pid) {
    if (pid < 1) {
        alert("请选择商品");
    }
    else if (uid < 1) {
        alert("请先登录");
    }
    else {
        Ajax.get("/UCenter/AddProductToFavorite?pid=" + pid, false, addProductToFavoriteResponse)
    }
}

//处理添加商品到收藏夹的反馈信息
function addProductToFavoriteResponse(data) {
    var result = eval("(" + data + ")");
    alert(result.content);
}

//添加店铺到收藏夹
function addStoreToFavorite(storeId) {
    if (storeId < 1) {
        alert("请选择店铺");
    }
    else if (uid < 1) {
        alert("请先登录");
    }
    else {
        Ajax.get("/UCenter/AddStoreToFavorite?storeId=" + storeId, false, addStoreToFavoriteResponse)
    }
}

//处理添加店铺到收藏夹的反馈信息
function addStoreToFavoriteResponse(data) {
    var result = eval("(" + data + ")");
    alert(result.content);
}

//添加商品到购物车
function addProductToCart(pid, buyCount) {
    if (pid < 1) {
        alert("请选择商品");
    }
    else if (isGuestSC == 0 && uid < 1) {
        alert("请先登录");
    }
    else if (buyCount < 1) {
        alert("请填写购买数量");
    }
    else if (scSubmitType != 2) {
        window.location.href = "/Cart/AddProduct?pid=" + pid + "&buyCount=" + buyCount;
    }
    else {
        Ajax.get("/Cart/AddProduct?pid=" + pid + "&buyCount=" + buyCount, false, addProductToCartResponse)
    }
}

//处理添加商品到购物车的反馈信息
function addProductToCartResponse(data) {
    var result = eval("(" + data + ")");
    alert(result.content);
}

//添加套装到购物车
function addSuitToCart(pmId, buyCount) {
    if (pmId < 1) {
        alert("请选择套装");
    }
    else if (isGuestSC == 0 && uid < 1) {
        alert("请先登录");
    }
    else if (buyCount < 1) {
        alert("请填写购买数量");
    }
    else if (scSubmitType != 2) {
        window.location.href = "/Cart/AddSuit?pmId=" + pmId + "&buyCount=" + buyCount;
    }
    else {
        Ajax.get("/Cart/AddSuit?pmId=" + pmId + "&buyCount=" + buyCount, false, addSuitToCartResponse)
    }
}

//处理添加套装到购物车的反馈信息
function addSuitToCartResponse(data) {
    var result = eval("(" + data + ")");
    if (result.state != "stockout") {
        alert(result.content);
    }
    else {
        alert("商品库存不足");
    }
}