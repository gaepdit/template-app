// Don't submit empty form fields.
// (Add this script to search forms that use GET for submit. It keeps clutter out of the resulting query string.)
document.addEventListener('DOMContentLoaded', function () {
    function disableEmptyInput(element) {
        if (element.value === '') {
            element.setAttribute('disabled', 'disabled');
        }
    }

    const searchButton = document.getElementById('SearchButton');
    if (searchButton) {
        searchButton.addEventListener('click', function () {
            document.querySelectorAll('input').forEach(disableEmptyInput);
            document.querySelectorAll('select').forEach(disableEmptyInput);
            return true;
        });
    }
});
