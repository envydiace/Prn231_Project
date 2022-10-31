function getAllCategory() {
    $.ajax(
        {
            type: "GET",
            url: 'http://localhost:5000/api/category/getall',
            success: function (result, status, xhr) {
                var select = $("#select-category");
                for (let ele of result) {
                    select.append($("<option>", {
                        value: ele.categoryId,
                        text: ele.categoryName
                    }));
                }
            },
            error: function (xhr, status, error) {
                alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText)
            }
        }
    );
}

function getAllCategorySelected(id) {
    $.ajax(
        {
            type: "GET",
            url: 'http://localhost:5000/api/category/getall',
            success: function (result, status, xhr) {
                var select = $("#select-category");
                for (let ele of result) {
                    select.append($("<option>", {
                        value: ele.categoryId,
                        text: ele.categoryName
                    }));
                }
                select.val(id);
            },
            error: function (xhr, status, error) {
                alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText)
            }
        }
    );
}

function getProducts() {
    var categoryId = typeof $("#select-category").val() == 'undefined' ? 0 : $("#select-category").val();
    var search = typeof $("#search").val() == 'undefined' ? "" : $("#search").val();
    var accessToken = localStorage.getItem("accessToken");

    $.ajax(
        {
            type: "GET",
            url: 'http://localhost:5000/api/product/GetAllFilter?categoryid=' + categoryId + '&search=' + search,
            beforeSend: function (xhr) {
                xhr.setRequestHeader('Authorization', 'Bearer ' + accessToken);
            },
            success: function (result, status, xhr) {
                var table = $("#orders");
                if ($(".table-row").length) {
                    $("tr").remove(".table-row");
                }
                for (let ele of result) {
                    table.append("<tr class='table-row' >" +
                        "<td><a href='order-detail.html?id='" + ele.productId + ">#" + ele.productId + "</a></td>" +
                        "<td>" + ele.productName + " </td>" +
                        "<td>" + ele.unitPrice + " </td>" +
                        "<td>pieces</td>" +
                        "<td>" + ele.unitsInStock + " </td>" +
                        "<td>" + ele.categoryName + " </td>" +
                        "<td>" + ele.discontinued + " </td>" +
                        "<td>" +
                        "<a href='edit-product.html?id=" + ele.productId + "'>Edit</a> &nbsp; | &nbsp; " +
                        "<a href='#' data-id=" + ele.productId + " onclick='deleteCategory(this);'>Delete</a>" +
                        "</td>" +
                        "</tr>");
                }
            },
            error: function (xhr, status, error) {
                switch (xhr.status) {
                    case 401:
                        alert("Unauthorized! Please Login!");
                        window.location.href = 'signin.html';
                        break;
                    case 403:
                        alert("Result: user don't have permission !");
                        window.location.href = 'signin.html';
                        break;
                    default:
                        alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText);
                        window.location.href = 'index.html';
                        break;
                }

            }
        }
    );
}

function deleteCategory(ctl) {
    var id = $(ctl).data("id");
    $.ajax(
        {
            type: "DELETE",
            url: 'http://localhost:5000/api/product/Delete/' + id,
            success: function (result, status, xhr) {
                alert("Delete success!");
                getAllCategory();
            },
            error: function (xhr, status, error) {
                alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText)
            }
        }
    );
}

function addProduct() {
    var accessToken = localStorage.getItem("accessToken");
    $.ajax({
        type: "POST",
        url: 'http://localhost:5000/api/product/Create',
        data: {
            productName: $("#productName").val(),
            categoryId: $("#select-category").val(),
            quantityPerUnit: $("#quantityPerUnit").val(),
            unitPrice: $("#unitPrice").val(),
            unitsInStock: $("#unitsInStock").val(),
            unitsOnOrder: 0,
            reorderLevel: $("#reorderLevel").val(),
            discontinued: $("#discontinued").prop('checked')
        },
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + accessToken);
        },
        dataType: 'json',
        success: function (result) {
            alert("Add Success!");
            location.href = 'product.html';
        },
        error: function (xhr, status, error, data) {
            alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText + " " + error.text);
        }
    });
}

function getProduct(id) {
    $.ajax(
        {
            type: "GET",
            url: 'http://localhost:5000/api/product/get/' + id,
            success: function (result, status, xhr) {
                getAllCategorySelected(result.categoryId);
                $("#productName").val(result.productName);
                $("#unitPrice").val(result.unitPrice);
                $("#quantityPerUnit").val(result.quantityPerUnit);
                $("#unitsInStock").val(result.unitsInStock);
                // $("#select-category").val(result.categoryId);
                $("#reorderLevel").val(result.reorderLevel);
                $("#unitsOnOrder").val(result.unitsOnOrder);
                $("#discontinued").prop('checked', result.discontinued);
            },
            error: function (xhr, status, error) {
                alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText)
            }
        }
    );
}

function updateProduct() {
    var accessToken = localStorage.getItem("accessToken");
    $.ajax({
        type: "PUT",
        url: 'http://localhost:5000/api/product/Update',
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', 'Bearer ' + accessToken);
        },
        data: {
            productId: productId,
            productName: $("#productName").val(),
            categoryId: $("#select-category").val(),
            quantityPerUnit: $("#quantityPerUnit").val(),
            unitPrice: $("#unitPrice").val(),
            unitsInStock: $("#unitsInStock").val(),
            unitsOnOrder: 0,
            reorderLevel: $("#reorderLevel").val(),
            discontinued: $("#discontinued").prop('checked')
        },
        dataType: 'json',
        success: function (result) {
            alert("Update Success!");
            location.href = 'product.html';
        },
        error: function (xhr, status, error, data) {
            alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText + " " + error.text);
        }
    });
}