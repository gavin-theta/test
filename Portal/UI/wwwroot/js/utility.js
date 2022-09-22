var utility = (function () {

    const longDateOptions = {year: 'numeric', month: 'numeric', day: 'numeric', hour: 'numeric', minute: 'numeric', second: 'numeric', timeZoneName: 'short'};
    const longDateTimeFormater = new Intl.DateTimeFormat('en-NZ', longDateOptions);
    const shortDateFormater = new Intl.DateTimeFormat('en-NZ');

    function showError(title, message) {
        $('#generalErrorTitle').text(title);
        $('#generalErrorMessage').text(message);
        $('#generalErrorModalDialog').modal({
            show: true
        });
    }

    function parseAccessToken() {
        const token = $('input[name="__RequestVerificationToken"]').val();
        return token;
    }

    function setAccessToken(xhr) {
        const accessToken = $('input[name="__RequestVerificationToken"]').val();
        xhr.setRequestHeader('RequestVerificationToken', accessToken);
    }

    function clearPageLoader() {
        $('.main-loader img').fadeOut(500);
        $('.main-loader').fadeOut(1000);
        setTimeout(function () { $('body').removeClass('Home'); }, 1000);
    }

    function formatBytes(bytes, decimals = 2) {
        if (bytes === 0) return '';

        const k = 1024;
        const dm = decimals < 0 ? 0 : decimals;
        const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'];

        const i = Math.floor(Math.log(bytes) / Math.log(k));

        return parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + ' ' + sizes[i];
    }

    function uniqueArray(arr) {

        const result = [];
        const duplicatesIndices = [];

        // Loop through each item in the original array
        arr.forEach((current, index) => {

            if (duplicatesIndices.indexOf(index) > -1) return;

            result.push(current);

            // Loop through each other item on array after the current one
            for (let comparisonIndex = index + 1; comparisonIndex < arr.length; comparisonIndex++) {

                const comparison = arr[comparisonIndex];
                const currentKeys = Object.keys(current);
                const comparisonKeys = Object.keys(comparison);

                // Check number of keys in objects
                if (currentKeys.length !== comparisonKeys.length) {
                    continue;
                }

                // Check key names
                const currentKeysString = currentKeys.sort().join("").toLowerCase();
                const comparisonKeysString = comparisonKeys.sort().join("").toLowerCase();

                if (currentKeysString !== comparisonKeysString) {
                    continue;
                }

                // Check values
                let valuesEqual = true;
                for (let i = 0; i < currentKeys.length; i++) {
                    const key = currentKeys[i];
                    if (current[key] !== comparison[key]) {
                        valuesEqual = false;
                        break;
                    }
                }

                if (valuesEqual) {
                    duplicatesIndices.push(comparisonIndex);
                }
            }

        });

        return result;
    }

    function getDateFormatted(dateString, formatter) {
        let dateFormatted = "";
        try {
            dateString = dateString.toUpperCase();
            if (!dateString.endsWith("Z")) {
                dateString += "Z";
            }
            let date = new Date(dateString);
            if (date.getFullYear() < 1900) {
                dateFormatted = "";
            } else {
                dateFormatted = formatter.format(date).split("/").join("-");
            }
        } catch (error) {
            dateFormatted = "";
        }

        return dateFormatted;
    }

    function getFormattedShortDate(dateString) {
        return getDateFormatted(dateString, shortDateFormater);
    }

    function getFormattedLongDate(dateString) {
        return getDateFormatted(dateString, longDateTimeFormater);
    }

    return {       
        getAccessToken: parseAccessToken,
        setAccessToken: setAccessToken,
        clearPageLoader: clearPageLoader,
        formatBytes: formatBytes,
        uniqueArray: uniqueArray,
        formatDate: getFormattedShortDate,
        formatLongDate: getFormattedLongDate,
        showErrorDialog: showError,
    };

})();