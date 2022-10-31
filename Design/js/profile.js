function getCustomerProfile() {
    var accessToken = localStorage.getItem("accessToken");
    $.ajax(
        {
            type: "GET",
            url: 'http://localhost:5000/api/Customer/GetCustomer',
            beforeSend: function (xhr) {
                xhr.setRequestHeader('Authorization', 'Bearer ' + accessToken);
            },
            success: function (result, status, xhr) {
                $("#name").html("Welcome, "+result.contactName);
                $("div").remove(".content-main");
                $("div").remove(".path");
                var div = $("#content-right");
                // var div = $("#profile-content");
                
                div.append("<div class='path'>Personal information</b></div>"+
                "<div class='content-main'>"+
                "<div id='profile-content'>"+
                "<div class='profile-content-col'>" +
                    "<div>Company name: <br/>"+result.companyName+"</div>" +
                    "<div>Contact name: <br/>"+result.contactName+"</div>" +
                    "<div><input type='submit' value='Edit info'/></div>" +
                    "</div>"+
                    "<div class='profile-content-col'>" +
                    "<div>Company title: <br/>"+result.contactTitle+"</div>" +
                    "<div>Address: <br/>"+result.address+"</div>" +
                    "</div>"+
                    "<div class='profile-content-col'>" +
                    "<div>Email: <br/>"+result.email+"</div>" +
                    "</div></div></div>");
            },
            error: function (xhr, status, error) {
                // alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText)
                window.location.href = 'signin.html';
            }
        }
    );
}

function getCustomerOrder() {
    var accessToken = localStorage.getItem("accessToken");
    $.ajax(
        {
            type: "GET",
            url: 'http://localhost:5000/api/Order/GetCustomerOrder',
            beforeSend: function (xhr) {
                xhr.setRequestHeader('Authorization', 'Bearer ' + accessToken);
            },
            success: function (result, status, xhr) {
                $("div").remove(".content-main");
                $("div").remove(".path");
                var div = $("#content-right");
                var append = "<div class='path'>LIST ORDERS</b></div>"+
                "<div class='content-main'>"+
                    "<div id='profile-content-order'>";
                    for(let order of result){
                        append+= "<div>"+
                        "<div class='profile-order-title'>"+
                            "<div class='profile-order-title-left'>"+
                                "<div>Order creation date: "+order.orderDate+"</div>"+
                                "<div>Order: <a href='#'>#"+order.orderId+"</a></div>"+
                           "</div>"+
                            "<div class='profile-order-title-right'><span>Pending</span></div>"+
                        "</div>";
                        for (let orderDetail of order.orderDetails ){
                            append+="<div class='profile-order-content'>"+
                            "<div class='profile-order-content-col1'>"+
                               "<a href='detail.html'><img src='img/2.jpg' width='100%'/></a>"+
                            "</div>"+
                            "<div class='profile-order-content-col2'>"+orderDetail.productName+"</div>"+
                            "<div class='profile-order-content-col3'>Quantity: "+orderDetail.quantity+"</div>"+
                            "<div class='profile-order-content-col4'>"+orderDetail.unitPrice*orderDetail.quantity+" $</div>"+
                       "</div>";
                        }
                        append+="</div>";
                    }
                    append+="</div>"+
                "</div>";
                div.append(append);
            },
            error: function (xhr, status, error) {
                // alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText)
                window.location.href = 'signin.html';
            }
        }
    );
}