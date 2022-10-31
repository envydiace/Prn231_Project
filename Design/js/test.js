function getCategories(){
    $.ajax(
        {
            type: 'GET',
            url: 'http://localhost:5000/api/category/getall',
            success: function (result){
                console.log(result);
                $("tr").remove(".table-row") ;
                for(let category of result){
                    $("#my-table").append("<tr class='table-row' ><td>"+category.categoryId+"</td><td>"+category.categoryName+"</td></tr>")
                }
            },
            error: function (xhr, status, error){
                alert("Result: " + status + " " + error + " " + xhr.status + " " + xhr.statusText)
            }
        }
    )
}