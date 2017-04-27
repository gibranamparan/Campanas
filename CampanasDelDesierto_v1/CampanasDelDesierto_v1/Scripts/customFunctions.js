$(document).ready(function () {
    $('.datablejs').DataTable();
});

/* When the user clicks on the button, 
toggle between hiding and showing the dropdown content */
function openActionsDropDown(actionBtn) {
    $(actionBtn).parent().children("#myDropdown").slideToggle();
}

/* When the user clicks on the button, 
toggle between hiding and showing the dropdown content */
function openActionsDropDown(actionBtn) {
    $(actionBtn).parent().children("#myDropdown").slideToggle();
}

// Close the dropdown menu if the user clicks outside of it
window.onclick = function (event) {
    if (!event.target.matches('.dropbtn')) {

        var dropdowns = document.getElementsByClassName("dropdown-content");
        var i;
        for (i = 0; i < dropdowns.length; i++) {
            var openDropdown = dropdowns[i];
            if (openDropdown.classList.contains('show')) {
                openDropdown.classList.remove('show');
            }
        }
    }
}

//Agrega funcion a JQuery para permitir solicitudes asincronas identificandose como usuario logeado
jQuery.postJSON = function (url, data, dataType, success, fail, always, antiForgeryToken) {
    if (dataType === void 0) { dataType = "json"; }
    if (typeof (data) === "object") { data = JSON.stringify(data); }
    var ajax = {
        url: url,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: dataType,
        data: data,
        success: success,
        fail: fail,
        complete: always
    };
    if (antiForgeryToken) {
        ajax.headers = {
            "__RequestVerificationToken": antiForgeryToken
        };
    };

    return jQuery.ajax(ajax);
};

/*Llamada asincrona para tomar del servidor el tipo de cambio de forma asincrona*/
function getTipoCambioAsync(success, error, complete , antiForgeryToken) {
    jQuery.postJSON("/MovimientoFinancieros/getCambioDolar", 0, 'JSON',
        success, error, complete, antiForgeryToken)
}