function getProduct(id) {
    $.ajax(
        {
            type: "GET",
            url: 'http://localhost:5000/api/product/get/' + id,
            success: function (result, status, xhr) {
                $("#product-name h2").html(result.productName);
                $("#product-price").html("$ "+result.unitPrice);
                // $("#quantityPerUnit").val(result.quantityPerUnit);
                // $("#unitsInStock").val(result.unitsInStock);
                // $("#select-category").val(result.categoryId);
                // $("#reorderLevel").val(result.reorderLevel);
                // $("#unitsOnOrder").val(result.unitsOnOrder);
                // $("#discontinued").prop('checked', result.discontinued);
            },
            error: function (xhr, status, error) {
                alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText)
            }
        }
    );
}

function addProductToCart(){
    addToCart(productId,1);
}