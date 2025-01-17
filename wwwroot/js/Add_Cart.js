function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    // Allow digits only
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        evt.preventDefault();
    }
    return true;
}
//function addqty(e) {
//    var textinput = document.getElementById("qtyinp");
//    let qty = parseInt(textinput.value);
//    qty = qty + 1;
//    textinput.value = String(qty);
//}
//function reduceQty(e) {
//    var textinput = document.getElementById("qtyinp");
//    let qty = parseInt(e.textinput.value);
//    qty = qty - 1;
//    e.textinput.value = String(qty);
//}
function qtychangedValidation(e, itemquantity) {
    if (e.target.value > itemquantity || e.target.value <= 0) {
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
            console.log(response);
            $("#totalprice_lbl").text(response.total);
            if (response.cartItems) {
                let htmlTxt = ``; 
                response.cartItems.forEach(item => {
                    htmlTxt += `  <div class="itembox">
                            <div class="imagediv">
                                if (${item.ItemDetails.ImageData} != null)
                                {
                                    <img src="data:image/png;base64,@Convert.ToBase64String(${item.ItemDetails.ImageData})" alt="${i.ItemName}" class="img-food" />
                                }
                                else
                                {
                                    <img src="" class="img-food" alt="item.ItemName" />
                                }
                            </div>
                            <p>₹@item.ItemDetails.ItemPrice</p>
                          
                                            <p id="qty"> <input type="number" value="1" id="qtyinp" onchange="qtychanged(event, ${item.ItemDetails.ItemQuantity}),onqtychange(event, ${item.ItemId})" onkeydown="isNumberKey(event)" class="form-control" pattern="^[0-9]+" required />&nbsp; <b>Qty</b></p>
                                            <p> Item Catagory: ${item.ItemDetails.ItemType}</p>
                            <a onclick="deleteItem(${item.CartId})" class="cursor-pointer">
                                <i class="fa-solid fa-trash text-dark text-hover-danger"></i>
                            </a>
                        </div>`;
                });
                $('#getItems').html(htmlTxt);
            }
        },
        error: function (xhr, status, error) {
            // Handle any errors that occur during the request
            console.error("Error: " + error);
            alert("Something went wrong!");
        }

    });
  
}
function AddDeliveryAddress() {
    let deliveryaddress = $('#deliveryaddress_txt').val();
    let mobileNo = $('#mobileNo_txt').val();
    let pin = $('#pin_txt').val();
    if (deliveryaddress && mobileNo && pin) {
        //$.ajax({
        //    url: '/Cart/AddDeliveryAddress', // Example API URL
        //    type: 'Post',  // Request type (GET, POST, etc.)
        //    data: {
        //        DeliveryAddress: deliveryaddress,
        //        Pin: pin,
        //        MobileNo: mobileNo,
        //    },
        //    dataType: 'json',  // Expected response type
        //    success: function (response) {
        //        // On success, update the DOM with the data
        //        console.log(response.data);
        //    },
        //    error: function (xhr, status, error) {
        //        // Handle any errors that occur during the request
        //        console.error("Error: " + error);
        //        alert("Something went wrong!");
        //    }

        //});

        axios.post('/Cart/AddDeliveryAddress', {

            Address: deliveryaddress,
            Pincode: pin,
            MobileNo: mobileNo
        }, {
            headers: {
                'Content-Type': 'application/json'
            }
        
            })
            .then(function (response) {
                console.log(response);
                $('#DeliveryAddressModal').modal('hide')
                loadData();                                     // load latest delivery address
                $('deliveryaddress_txt').text('');
                $('pin_txt').text('');
                $('mobileNo_txt').text('');
                
            })
            .catch(function (error) {
                console.log(error);
            });
    }

}



                    