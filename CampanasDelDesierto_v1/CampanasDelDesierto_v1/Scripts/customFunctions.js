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