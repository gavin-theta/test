var keyboard = (function () {

    function getKeyCode(e) {
        let keynum = 0;
        if (window.event) { // IE
            keynum = e.keyCode;
        } else if (e.which) { // Netscape/Firefox/Opera
            keynum = e.which;
        }
        return keynum;
    }

    function navigateOnKeyPress(e) {
        let keynum = getKeyCode(e);
        var id = $(e.target).attr("id");
        var element = document.getElementById(id);
        var navUrl = element.getAttribute('data-nav') || "/";

        if (keynum === 13) {
            window.location = navUrl;
        }
    }

    function showSelectionOnKeyPress(e, func) {
        let keynum = getKeyCode(e);
        if (keynum === 13) {
            var id = $(e.target).attr("id");
            let element = document.getElementById(id);
            let check = element.getAttribute("data-check") || "";
            let records = parseInt(element.getAttribute("data-errorrecords") || "0");
            if (check.length > 0) {
                func(check, records);
            }
        }
    }

    function onKeyPress(e, func) {
        let keynum = getKeyCode(e);
        if (keynum === 13) {
            func();
        }
    }




    return {
        navigateOnKeyPress: navigateOnKeyPress,
        showSelectionOnKeyPress: showSelectionOnKeyPress,
        onKeyPress: onKeyPress,
         getKeyCode: getKeyCode
    }

})();