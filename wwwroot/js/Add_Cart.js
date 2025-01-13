function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    // Allow digits only
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}
function addqty(e) {
    var textinput = document.getElementById("qtyinp");
    let qty = parseInt(e.textinput.value);
    qty = qty + 1;
    e.textinput.value = String(qty);
}
function qtychanged(e, itemquantity) {
    if (e.target.value > itemquantity || e.target.value < 0) {
        e.target.value = itemquantity;
    }
}
function selectDeliveryAddress(e) {
    var delivery = document.getElementsByName("DeliveryId");
    let radiovalue = 0;
    delivery.forEach(e => {
        if (e.checked) {
            radiovalue = 1;
        }
    }
    )
    if (radiovalue == 0) {
        alert("please select delivery address")
    }
}
function onqtychange(e,itemid) {
    // Make an AJAX request
    $.ajax({
        url: '/Cart/UpdateCartItemQty', // Example API URL
        type: 'Post',  // Request type (GET, POST, etc.)
        data: {
            itemid: itemid,
            itemqty: e.target.value
        },
        dataType: 'json',  // Expected response type
        success: function (response) {
            // On success, update the DOM with the data
            console.log(response.data);
            $("#totalprice_lbl").text(response.total);
        },
        error: function (xhr, status, error) {
            // Handle any errors that occur during the request
            console.error("Error: " + error);
            alert("Something went wrong!");
        }

    });
}
function deleteItem(id) {
    $.ajax({
        url: '/Cart/Delete_Item', // Example API URL
        type: 'Post',  // Request type (GET, POST, etc.)
        data: {
            id: id,
        },
        dataType: 'json',  // Expected response type
        success: function (response) {
            // On success, update the DOM with the data
            console.log(response.data);
            $("#totalprice_lbl").text(response.total);
        },
        error: function (xhr, status, error) {
            // Handle any errors that occur during the request
            console.error("Error: " + error);
            alert("Something went wrong!");
        }

    });
}