//店铺搜索
function storeSearch(keyword) {
    if (keyword == undefined || keyword == null || keyword.length < 1) {
        alert("请输入关键词");
    }
    else {
        window.location.href = "/Store/Search?keyword=" + encodeURIComponent(keyword);
    }
}