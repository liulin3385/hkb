var provinceId = -1; //省id
var cityId = -1; //市id
var countyId = -1; //县或区id

//绑定省列表
function bindProvinceList(provinceSelectObj, selectProvinceId) {
    Ajax.get("/Tool/ProvinceList", true, function (data) {
        var provinceList = eval("(" + data + ")");
        if (provinceList.content.length > 0) {
            var optionStr = "<option value='-1'>请选择</option>";
            for (var i = 0; i < provinceList.content.length; i++) {
                optionStr = optionStr + "<option value='" + provinceList.content[i].id + "'>" + provinceList.content[i].name + "</option>";
            }
            provinceSelectObj.innerHTML = optionStr;
            if (selectProvinceId == undefined)
                selectProvinceId = -1;
            setSelectedOptions(provinceSelectObj, selectProvinceId);
        }
        else {
            alert("加载省列表时出错！")
        }
    })
}

//绑定市列表
function bindCityList(provinceId, citySelectObj, selectCityId) {
    Ajax.get("/Tool/CityList?provinceId=" + provinceId, true, function (data) {
        var cityList = eval("(" + data + ")");
        if (cityList.content.length > 0) {
            var optionStr = "<option value='-1'>请选择</option>";
            for (var i = 0; i < cityList.content.length; i++) {
                optionStr = optionStr + "<option value='" + cityList.content[i].id + "'>" + cityList.content[i].name + "</option>";
            }
            citySelectObj.innerHTML = optionStr;
            if (selectCityId == undefined)
                selectCityId = -1;
            setSelectedOptions(citySelectObj, selectCityId);
        }
        else {
            alert("加载市列表时出错！")
        }
    })
}

//绑定县或区列表
function bindCountyList(cityId, countySelectObj, selectCountyId) {
    Ajax.get("/Tool/CountyList?cityId=" + cityId, true, function (data) {
        var countyList = eval("(" + data + ")");
        if (countyList.content.length > 0) {
            var optionStr = "<option value='-1'>请选择</option>";
            for (var i = 0; i < countyList.content.length; i++) {
                optionStr = optionStr + "<option value='" + countyList.content[i].id + "'>" + countyList.content[i].name + "</option>";
            }
            countySelectObj.innerHTML = optionStr;
            if (selectCountyId == undefined)
                selectCountyId = -1;
            setSelectedOptions(countySelectObj, selectCountyId);
        }
        else {
            alert("加载县或区列表时出错！")
        }
    })
}
