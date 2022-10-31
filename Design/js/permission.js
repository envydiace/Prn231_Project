function Login() {
    var email = $("#email").val();
    var pass = $("#password").val();
    if (email == "" || pass == ""){
        
        $("#error-email").attr("hidden",email != "");
        $("#error-password").attr("hidden",pass != "");
    }
    else{
        $.ajax({
            type: "POST",
            url: 'http://localhost:5000/api/login',
            data: {
                email: email,
                password: pass
            },
            dataType: 'text',
            success: function(result) {
                localStorage['accessToken'] = result;
                window.location.href = 'index.html';
            },
            error: function(xhr, status, error, data) {
                switch (xhr.status) {
                    case 400:
                        alert("Email or password doesn't correct!");
                        window.location.href = 'signin.html';
                        break;
                    default:
                        alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText);
                        window.location.href = 'index.html';
                        break;
                }
            }
        });
    }
    
}

function Logout(){
    localStorage.removeItem("accessToken");
    window.location.href = 'index.html';
}