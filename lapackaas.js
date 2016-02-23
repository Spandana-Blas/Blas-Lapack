$(function () {


    // Declare a proxy to reference the hub.
    var blas = $.connection.bLAS;

    // Start the connection.
    $.connection.hub.start().done(function () {
        alert('Now connected, connection ID=' + $.connection.hub.id)
        $('#send').click(function () {
            var mat1JSON = $('#mat1').val();
            var mat2JSON = $('#mat2').val();
            

            // Call the matrix multiplication method on the hub.
            blas.server.blas1(mat1JSON, mat2JSON);
        });
        $('#matvec').click(function () {
            var mat3JSON = $('#mat3').val();
            var mat4JSON = $('#mat4').val();
            // Call the matrix multiplication method on the hub.
            blas.server.blas2(mat3JSON, mat4JSON);
        });
        $('#matmat').click(function () {
            var mat5JSON = $('#mat5').val();
            var mat6JSON = $('#mat6').val();
            // Call the matrix multiplication method on the hub.
            blas.server.blas3(mat5JSON, mat6JSON);
        });
        $('#lufact').click(function () {
            var mat7JSON = $('#mat7').val();
            var mat8JSON = $('#mat8').val();
            // Call the matrix multiplication method on the hub.
            blas.server.blas4(mat7JSON, mat8JSON);
        });
    });
    // Create a function that the hub can call to display the product.
    blas.client.displayBlas1 = function (product) {
        //Parse JSON using JSON.parse  http://www.w3schools.com/json/json_eval.asp
        var productObj = JSON.parse(product);
        
        document.getElementById("Product1").innerHTML = 'The dot product is ' + productObj;
    };
    blas.client.displayBlas2 = function (product) {
        //Parse JSON using JSON.parse  http://www.w3schools.com/json/json_eval.asp
        var productObj = JSON.parse(product);
        var len = productObj.data.length;
        var output = '';
        for ( i=0; i<len; i++)
        {
            output += '[' + productObj.data[i] + ']';
            if (i != len -1)
            {
                output += ',';
            }
        }
        document.getElementById("Product2").innerHTML = ' Resulting Vectror of Matrix vector Multiplication is: [' + output + '] ';
    };
    blas.client.displayBlas3 = function (product) {
        //Parse JSON using JSON.parse  http://www.w3schools.com/json/json_eval.asp
        var productObj = JSON.parse(product);
        var len = productObj.data.length;
        var output = '';
        for (i = 0; i < len; i++) {
            output += '[' + productObj.data[i] + ']';
            if (i != len - 1) {
                output += ',';
            }
        }
        document.getElementById("Product3").innerHTML = ' Resulting product of Matrix - Matrix Multiplication is: [' + output + '] ';
    };
    blas.client.displayBlas4 = function (product) {
        //Parse JSON using JSON.parse  http://www.w3schools.com/json/json_eval.asp
        var productObj = JSON.parse(product);
        var len = productObj.data.length;
        var output = '';
        for (i = 0; i < len; i++) {
            output += '[' + productObj.data[i] + ']';
            if (i != len - 1) {
                output += ',';
            }
        }
        document.getElementById("Product4").innerHTML = ' Resulting solution is : [' + productObj.data[0] + '] ';
    };

});
