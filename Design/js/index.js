var totalSize;
var listPage = [1, 2, 3, 4, 5, 6];
function getListPageIndex() {
    var divPages = $("#paging-new");
    $("div").remove("#pagination-new");
    var pageGen = "<div id='pagination-new' class='pagination'>" ;
    pageGen+="<a>&laquo;</a>";
    for(let index of listPage){
        pageGen+="<a>"+index+"</a>"
    }
    pageGen+="<a>&raquo;</a></div>";  
    divPages.append(pageGen);
}


function getAllCategory() {
    $.ajax(
        {
            type: "GET",
            url: 'http://localhost:5000/api/category/getall',
            success: function (result, status, xhr) {
                var list = $("#ul-category");
                list.append(" <a class='active' id='li-category-0' onclick='getProductsByCategory(0);'><li>All</li></a>");
                for (let ele of result) {
                    list.append("<a id='li-category-" + ele.categoryId + "' onclick='getProductsByCategory(" + ele.categoryId + ");'><li>" + ele.categoryName + "</li></a>");
                }
            },
            error: function (xhr, status, error) {
                alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText)
            }
        }
    );
}
function getProductsByCategory(id) {
    var list = $("#ul-category a.active");
    list.removeAttr('class');
    var item = $("#li-category-" + id);
    item.attr("class", "active");
    getProducts(id);
    getProductsBestSale(id);
}

function getProducts(categoryId) {
    var searchByCategoryId = categoryId != 0 ? "&filter=categoryId eq " + categoryId : ""
    $.ajax(
        {
            type: "GET",
            url: 'http://localhost:5000/odata/ProductOData?$count=true&$orderby=ProductId desc&$skip=0' + searchByCategoryId,
            success: function (result, status, xhr) {
                var table = $("#new-product");
                if ($(".div-new-product").length) {
                    $("div").remove(".div-new-product");
                }
                for (let ele of result.value) {
                    table.append("<div class='product div-new-product'>" +
                        "<a href='detail.html?id=" + ele.ProductId + "'><img src='img/1.jpg' width='100%' /></a>" +
                        "<div class='name'><a href='detail.html?id=" + ele.ProductId + "'>" + ele.ProductName + "</a></div>" +
                        "<div class='price'>$" + ele.UnitPrice + "</div>" +
                        "<div><a href=''>Buy now</a></div>" +
                        "</div>");
                }
                totalSize = result["@odata.count"];
            },
            error: function (xhr, status, error) {
                alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText);
            }
        }
    );
}

function getProductsBestSale(categoryId) {
    $.ajax(
        {
            type: "GET",
            url: 'http://localhost:5000/api/product/GetProductBestSale?categoryId=' + categoryId,
            success: function (result, status, xhr) {
                var table = $("#sale-product");
                if ($(".div-sale-product").length) {
                    $("div").remove(".div-sale-product");
                }
                for (let ele of result.values) {
                    table.append("<div class='product div-sale-product'>" +
                        "<a href='detail.html?id=" + ele.productId + "'><img src='img/1.jpg' width='100%' /></a>" +
                        "<div class='name'><a href='detail.html?id=" + ele.productId + "'>" + ele.productName + "</a></div>" +
                        "<div class='price'>$" + ele.unitPrice + "</div>" +
                        "<div><a href=''>Buy now</a></div>" +
                        "</div>");
                }
                console.log(result.total);
            },
            error: function (xhr, status, error) {
                alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText);
            }
        }
    );
}
