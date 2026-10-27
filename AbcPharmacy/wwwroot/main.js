window.onload = function () {
    getMedicines("");
    getSales();
}

function searchClick() {
    var t = document.getElementById("txtSearch").value;
    getMedicines(t);
}

function resetClick() {
    document.getElementById("txtSearch").value = "";
    getMedicines("");
}

function getMedicines(nm) {
    var url = "/api/medicines";
    if (nm != null && nm != "")
        url = url + "?name=" + encodeURIComponent(nm);

    var xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            var data = JSON.parse(this.responseText);
            showGrid(data);
            fillCombo();
        }
    };
    xhttp.open("GET", url, true);
    xhttp.send();
}

function showGrid(data) {
    var tbl = document.getElementById("grd");
    while (tbl.rows.length > 1) {
        tbl.deleteRow(1);
    }

    document.getElementById("lblMsg").innerHTML = "";

    if (data.length == 0) {
        document.getElementById("lblMsg").innerHTML = "No data found";
        return;
    }

    for (var i = 0; i < data.length; i++) {
        var r = tbl.insertRow(-1);
        var d1 = new Date(data[i].expiryDate);
        var d2 = new Date();
        d2.setHours(0, 0, 0, 0);
        d1.setHours(0, 0, 0, 0);
        var days = (d1 - d2) / (1000 * 60 * 60 * 24);

        if (days < 30)
            r.style.backgroundColor = "#f5b5b5";
        else if (data[i].quantity < 10)
            r.style.backgroundColor = "#f7e07c";

        r.insertCell(0).innerHTML = data[i].fullName;
        r.insertCell(1).innerHTML = data[i].brand;
        r.insertCell(2).innerHTML = showDate(data[i].expiryDate);
        r.insertCell(3).innerHTML = data[i].quantity;
        r.insertCell(4).innerHTML = parseFloat(data[i].price).toFixed(2);
        r.insertCell(5).innerHTML = "<a href='javascript:void(0)' onclick='openNotes(" + data[i].id + ")'>view</a>";
    }
}

function fillCombo() {
    var xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            var data = JSON.parse(this.responseText);
            var cmb = document.getElementById("cmbMed");
            var old = cmb.value;
            cmb.innerHTML = "";
            var op = document.createElement("option");
            op.value = "0";
            op.text = "--Select--";
            cmb.appendChild(op);
            for (var i = 0; i < data.length; i++) {
                var o = document.createElement("option");
                o.value = data[i].id;
                o.text = data[i].fullName + "  (qty " + data[i].quantity + ")";
                cmb.appendChild(o);
            }
            if (old != "")
                cmb.value = old;
        }
    };
    xhttp.open("GET", "/api/medicines", true);
    xhttp.send();
}

function openNotes(id) {
    var xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            var m = JSON.parse(this.responseText);
            if (m.notes == null || m.notes == "")
                alert("No notes");
            else
                alert(m.fullName + "\n" + m.notes);
        }
    };
    xhttp.open("GET", "/api/medicines/" + id, true);
    xhttp.send();
}

function addMedicine() {
    var obj = new Object();
    obj.fullName = document.getElementById("txtName").value;
    obj.brand = document.getElementById("txtBrand").value;
    obj.expiryDate = document.getElementById("txtExpiry").value;
    obj.quantity = parseInt(document.getElementById("txtQty").value);
    obj.price = parseFloat(document.getElementById("txtPrice").value);
    obj.notes = document.getElementById("txtNotes").value;

    if (obj.fullName == "" || obj.brand == "" || obj.expiryDate == "") {
        document.getElementById("lblAdd").innerHTML = "Name / Brand / Expiry required";
        document.getElementById("lblAdd").style.color = "red";
        return;
    }

    var xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                document.getElementById("lblAdd").innerHTML = "Saved successfully";
                document.getElementById("lblAdd").style.color = "green";
                document.getElementById("txtName").value = "";
                document.getElementById("txtBrand").value = "";
                document.getElementById("txtExpiry").value = "";
                document.getElementById("txtQty").value = "";
                document.getElementById("txtPrice").value = "";
                document.getElementById("txtNotes").value = "";
                getMedicines("");
            }
            else {
                document.getElementById("lblAdd").innerHTML = this.responseText.replace(/"/g, "");
                document.getElementById("lblAdd").style.color = "red";
            }
        }
    };
    xhttp.open("POST", "/api/medicines", true);
    xhttp.setRequestHeader("Content-Type", "application/json");
    xhttp.send(JSON.stringify(obj));
}

function addSale() {
    var mid = document.getElementById("cmbMed").value;
    var q = document.getElementById("txtSaleQty").value;

    if (mid == "0" || mid == "") {
        document.getElementById("lblSale").innerHTML = "Select medicine";
        document.getElementById("lblSale").style.color = "red";
        return;
    }

    var xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                document.getElementById("lblSale").innerHTML = "Sale saved";
                document.getElementById("lblSale").style.color = "green";
                document.getElementById("txtSaleQty").value = "1";
                getMedicines(document.getElementById("txtSearch").value);
                getSales();
            }
            else {
                document.getElementById("lblSale").innerHTML = this.responseText.replace(/"/g, "");
                document.getElementById("lblSale").style.color = "red";
            }
        }
    };
    xhttp.open("POST", "/api/sales", true);
    xhttp.setRequestHeader("Content-Type", "application/json");
    xhttp.send(JSON.stringify({ medicineId: parseInt(mid), qtySold: parseInt(q) }));
}

function getSales() {
    var xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            var data = JSON.parse(this.responseText);
            var tbl = document.getElementById("grdSale");
            while (tbl.rows.length > 1) {
                tbl.deleteRow(1);
            }
            if (data.length == 0) {
                var r = tbl.insertRow(-1);
                var c = r.insertCell(0);
                c.colSpan = 4;
                c.innerHTML = "No sales";
                return;
            }
            for (var i = 0; i < data.length; i++) {
                var r = tbl.insertRow(-1);
                r.insertCell(0).innerHTML = showDate(data[i].soldOn) + " " + new Date(data[i].soldOn).toLocaleTimeString();
                r.insertCell(1).innerHTML = data[i].medicineName;
                r.insertCell(2).innerHTML = data[i].qtySold;
                r.insertCell(3).innerHTML = parseFloat(data[i].amount).toFixed(2);
            }
        }
    };
    xhttp.open("GET", "/api/sales", true);
    xhttp.send();
}

function showDate(val) {
    var d = new Date(val);
    var dd = d.getDate();
    var mm = d.getMonth() + 1;
    var yy = d.getFullYear();
    if (dd < 10) dd = "0" + dd;
    if (mm < 10) mm = "0" + mm;
    return dd + "/" + mm + "/" + yy;
}
