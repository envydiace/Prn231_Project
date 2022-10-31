function getCart() {
    $.ajax(
        {
            type: "GET",
            url: 'http://localhost:5000/api/Cart/GetCart',
            success: function (result, status, xhr) {
                console.log("cart: ", result);
                var content = $("#cart-content");
                $("div").remove(".cart-item")
                var totalAmount = 0;
                for (let ele of result) {
                    content.append("<div class='cart-item'>" +
                        "<div class='cart-item-infor'>" +
                        "<div class='cart-item-img'> <img src='img/1.jpg' /></div>" +
                        "<div class='cart-item-name'>"+
                        "<a href='detail.html?id="+ele.productID+"'>"+ele.productName+"</a></div>" +
                        "<div class='cart-item-price'>"+ele.unitPrice+" $</div>" +
                        "<div class='cart-item-button'> <a class='cart-item-function-button' >Remove</a></div></div>" +
                        "<div class='cart-item-function'>" +
                        "<a class='cart-item-function-button' onclick='changeProductCartAmount("+ele.productID+",-1);'>-</a>" +
                        "<a class='cart-item-function-button' onclick='changeProductCartAmount("+ele.productID+",1);'>+</a>" +
                        "<input id='cart-item-quantity-"+ele.productID+"' type='text' value='"+ele.quantity+"' disabled />" +
                        "</div></div>"
                    );
                    totalAmount+= ele.unitPrice * ele.quantity;
                    $("div").remove("#cart-summary-content");
                    $("#cart-summary").append(" <div id='cart-summary-content'>Total amount: <span style='color:red'>"+totalAmount+" $</span></div>");
                }

            },
            error: function (xhr, status, error) {
                alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText)
            }
        }
    );
}

function addToCart(productID, quantity) {
    const cartItem = {
        "productID": productID,
        "quantity": quantity
    };
    let body = JSON.stringify(cartItem);
    $.ajax({
        type: "POST",
        url: 'http://localhost:5000/api/Cart/AddToCart',
        data: body,
        contentType: "application/json",
        dataType: 'json',
        success: function (result) {
            console.log(result);
        },
        error: function (xhr, status, error, data) {
            alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText + " " + error.text);
        }
    });
}

function changeProductCartAmount(id,quantity){
    cartItemQuantity = $("#cart-item-quantity-"+id);
    var newQuantity =  Number(cartItemQuantity.val())+quantity;
    cartItemQuantity.val(newQuantity);
}