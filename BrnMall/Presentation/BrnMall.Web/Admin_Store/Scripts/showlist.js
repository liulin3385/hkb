var oldPageSize = 15;
var oldPageNumber = 1;
var delMessage = "您确定要删除吗？";
var batchDelMessage = "您确定要删除全部吗？";

//表单提交操作
function doPostBack(action) {
    var from = $("form:first");
    if (action != "") {
        from.attr("action", action);
    }
    from.get(0).submit();
};

/*页面变量初始化*/
$(function () {
    oldPageSize = $("#pageSize").val();
    oldPageNumber = $("#pageNumber").val();
});

//搜索按钮
$(function () {
    $(".submit").click(function () {
        doPostBack("");
        return false;
    });
});

//删除按钮
$(function () {
    $(".deleteOperate").click(function () {
        if (!confirm(delMessage)) {
            return false;
        }
    });
});

//表格全选
$(function () {
    $("#allSelect").click(function () {
        $("input[type='checkbox'][selectItem='true']").attr("checked", $(this).attr("checked"));
    });
});

//批量删除
$(function () {
    $(".batchDel").click(function () {
        if ($("input[type='checkbox'][selectItem='true']:checked").length > 0) {
            if (confirm(batchDelMessage)) {
                doPostBack($(this).attr("delUrl"));
            }
        }
        else {
            alert("没有选中任何一项!");
        }
    })
})

//表格排序
$(function () {
    $(".dataList table thead tr th[name='sortTitle']").click(function () {
        $("#sortColumn").val($(this).attr("column"));
        $("#sortDirection").val($(this).attr("direction"));
        doPostBack("");
    });
});

//页数按钮
$(function () {
    $(".dataListEdit .page .bt").click(function () {
        $("#pageNumber").val($(this).attr("page"));
        doPostBack("");
        return false;
    });
});

//排序提示
$(function () {
    var sortColumn = $("#sortColumn").val();
    if (sortColumn != "") {
        var sortDirection = $("#sortDirection").val();
        var flagHtml = "";
        if (sortDirection == "ASC") {
            flagHtml = "<b>↑</b>";
            sortDirection = "DESC";
        }
        else {
            flagHtml = "<b>↓</b>";
            sortDirection = "ASC";
        }
        var sortTh = $(".dataList th[name='sortTitle'][column='" + sortColumn + "']");
        sortTh.attr("direction", sortDirection);
        sortTh.append(flagHtml);

    }
});

//每页显示条数输入框
$(function () {
    $("#pageSize").keyup(function (e) {
        var regex = /^[0-9]*[1-9][0-9]*$/;
        var value = $(this).val();
        if (!regex.test(value)) {
            alert("只能输入数字!");
            $(this).val(oldPageSize);
        }
        if (e.keyCode == 13) {
            doPostBack("");
        }
    });
});

//跳转到指定页输入框
$(function () {
    $("#pageNumber").keyup(function (e) {
        if (e.keyCode == 13) {
            doPostBack("");
        }
        var regex = /^[0-9]*[1-9][0-9]*$/;
        var value = $(this).val();
        if (!regex.test(value)) {
            alert("只能输入数字!");
            $(this).val(oldPageNumber);
        }
        else {
            var totalPages = $(this).attr("totalPages");
            if (parseInt(value) > parseInt(totalPages)) {
                alert("跳转页数不能大于" + totalPages);
            }
        }
    });
});

/*排序*/
var sortinputtempvalue = 0;
$(function () {
    $(".sortinput").focus(function () {
        var sortinputobj = $(this);
        sortinputtempvalue = sortinputobj.val();
        sortinputobj.val("");
        sortinputobj.attr("class", "selectedsortinput");
    });
    $(".sortinput").blur(function () {
        var sortinputobj = $(this);
        if (sortinputobj.val() == "") {
            sortinputobj.val(sortinputtempvalue)
        }
        else {
            var reg = /^-?\d+$/;
            if (!reg.test(sortinputobj.val())) {
                sortinputobj.val(sortinputtempvalue).attr("class", "selectedsortinput");
                alert("只能输入数字！")
                return;
            }
            else {
                if (sortinputtempvalue != sortinputobj.val()) {
                    $.jBox.tip("正在更新...", 'loading');
                    $.get(sortinputobj.attr("url") + "&displayOrder=" + sortinputobj.val(), function (data, textStatus) {
                        if (data != "0") {
                            $.jBox.tip('更新成功！', 'success');
                        } else {
                            sortinputobj.val(sortinputtempvalue);
                            $.jBox.error('更新失败，请联系管理员！', '更新失败');
                        }
                    });
                }
            }
        }
        sortinputobj.attr("class", "unselectedsortinput");
    });
});

/*删除*/
$(function () {
    $(".ajaxdeleteOperate").click(function () {
        var ajaxdeleteobj = $(this);
        $.jBox.confirm(delMessage, "提示", function (v, h, f) {
            if (v == 'ok') {
                $.jBox.tip("正在删除...", 'loading');
                $.get(ajaxdeleteobj.attr("url"), function (data, textStatus) {
                    if (data != "0") {
                        ajaxdeleteobj.parents("tr").remove();
                        $.jBox.tip('删除成功！', 'success');
                    } else {
                        $.jBox.error('删除失败，请联系管理员！', '删除失败');
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
});